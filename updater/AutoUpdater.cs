using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace updater
{
    class AutoUpdater
    {
        public delegate void OnDownloadProgressChangedEventHandler(int downloadProgress);
        public event OnDownloadProgressChangedEventHandler OnDownloadProgressChanged;

        // in newVersion variable we will store the
        // version info from xml file
        public Version NewVersion { get; set; }
        // and in this variable we will put the url we
        // would like to open so that the user can
        // download the new version
        // it can be a homepage or a direct
        // link to zip/exe file
        private string zipUrl = string.Empty;
        // provide the XmlTextReader with the URL of
        // our xml document
        private string changes = string.Empty;
#if !DEBUG
        private static string updateInfoURL = "https://raw.github.com/nefarius/TrueMount-2/master/TrueMount/TrueMountVersion.xml";
#else
        private static string updateInfoURL = "http://localhost/TrueMountVersion.xml";
#endif
        private string userAgent = string.Empty;
        private Version currentVersion = null;

        /// <summary>
        /// Creates a new AutoUpdater with a given current version.
        /// </summary>
        /// <param name="currentVersion">The current assembly version.</param>
        public AutoUpdater(Version currentVersion)
        {
            if (currentVersion == null)
                throw new ArgumentNullException();
            FetchVersion(currentVersion);
        }

        /// <summary>
        /// Creates new AutoUpdater with a given file name.
        /// </summary>
        /// <param name="currentAssembly">The full path to the assembly file.</param>
        public AutoUpdater(String currentAssembly)
        {
            if (string.IsNullOrEmpty(currentAssembly))
                throw new ArgumentNullException();
            AppDomain domain = AppDomain.CreateDomain("TrueMount");
            domain.Load(AssemblyName.GetAssemblyName(currentAssembly));
            FetchVersion(domain.GetAssemblies()[1].GetName().Version);
            AppDomain.Unload(domain);
        }

        /// <summary>
        /// Fetches the version of the parent assembly and builds User-Agent.
        /// </summary>
        /// <param name="currentVersion">The assembly version.</param>
        private void FetchVersion(Version currentVersion)
        {
            this.currentVersion = currentVersion;
            this.userAgent = "TrueMount/" + this.currentVersion +
                ";Updater/" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Returns the changes of the new version.
        /// </summary>
        public string ChangeLog
        {
            get
            {
                String temp = string.Empty;
                if (string.IsNullOrEmpty(changes))
                    return temp;

                foreach (string part in Regex.Split(changes, Environment.NewLine))
                {
                    if (!string.IsNullOrEmpty(part.Trim()))
                        temp += part.Trim() + Environment.NewLine;
                }
                return temp;
            }
        }

        /// <summary>
        /// Downloads and extracts an archive with the new version.
        /// </summary>
        /// <param name="downloadLocation">The target directory to store the downloaded data in.</param>
        /// <returns>Returns true on success.</returns>
        public bool DownloadNewVersion(String downloadLocation)
        {
            if (string.IsNullOrEmpty(downloadLocation))
                throw new ArgumentNullException();
            // without the url to the archive this can't do anything useful
            if (string.IsNullOrEmpty(this.zipUrl))
                return false;

            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(this.zipUrl);
                httpRequest.UserAgent = userAgent;
                byte[] buffer = new byte[4096];
                ZipFile zipFile = null;
                MemoryStream memoryStream = null;

                using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    using (Stream responseStream = httpResponse.GetResponseStream())
                    {
                        Int64 fileSize = httpResponse.ContentLength;
                        memoryStream = new MemoryStream();
                        //StreamUtils.Copy(responseStream, memoryStream, buffer);

                        int byteSize = 0, byteDownloaded = 0, percentDone = 0;
                        // read the binary stream till the end
                        while ((byteSize = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memoryStream.Write(buffer, 0, byteSize);
                            // calculate progress percentage
                            byteDownloaded += byteSize;
                            percentDone = (byteDownloaded * 100) / (int)httpResponse.ContentLength;
                            // invoke event to tell other forms the new download progress
                            this.OnDownloadProgressChanged.Invoke(percentDone);
                        }

                        // associate new zip compressed file with memory stream
                        zipFile = new ZipFile(memoryStream);
                    }
                }

                // extract every file in zip archive
                foreach (ZipEntry zipEntry in zipFile)
                {
                    // we don't need directories, they will be created
                    if (!zipEntry.IsFile)
                        continue;

                    Stream zipStream = zipFile.GetInputStream(zipEntry);
                    // full file path + name (in the AppData update folder)
                    string fileName = Path.Combine(downloadLocation, zipEntry.Name);
                    // directory name of the file
                    string dirName = Path.GetDirectoryName(fileName);
                    // if its directory does not exist, create it
                    if (!Directory.Exists(dirName))
                        Directory.CreateDirectory(dirName);

                    // create the file and write uncompressed data into it
                    using (FileStream streamWriter = File.Create(fileName))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if new version was detected.
        /// </summary>
        public bool NewVersionAvailable
        {
            get
            {
                if (!this.DownloadVersionInfo())
                    return false;

                // compare the versions
                if (this.currentVersion.CompareTo(NewVersion) < 0)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Downloads the version information from the update location.
        /// </summary>
        /// <returns>Returns true on success.</returns>
        public bool DownloadVersionInfo()
        {
            XmlTextReader xmlReader = null;

            var request = (HttpWebRequest)WebRequest.Create(updateInfoURL);
            request.UserAgent = userAgent;
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (xmlReader = new XmlTextReader(responseStream))
            {
                // simply (and easily) skip the junk at the beginning
                xmlReader.MoveToContent();
                // internal - as the XmlTextReader moves only
                // forward, we save current xml element name
                // in elementName variable. When we parse a
                // text node, we refer to elementName to check
                // what was the node name
                string elementName = string.Empty;
                // we check if the xml starts with a proper
                // "truemount" element node
                if ((xmlReader.NodeType == XmlNodeType.Element) &&
                    (xmlReader.Name == "truemount"))
                {
                    while (xmlReader.Read())
                    {
                        // when we find an element node,
                        // we remember its name
                        if (xmlReader.NodeType == XmlNodeType.Element)
                            elementName = xmlReader.Name;
                        else
                        {
                            // for text nodes...
                            if ((xmlReader.NodeType == XmlNodeType.Text) &&
                                (xmlReader.HasValue))
                            {
                                // we check what the name of the node was
                                switch (elementName)
                                {
                                    case "version":
                                        // thats why we keep the version info
                                        // in xxx.xxx.xxx.xxx format
                                        // the Version class does the
                                        // parsing for us
                                        NewVersion = new Version(xmlReader.Value);
                                        break;
                                    case "url":
                                        zipUrl = xmlReader.Value;
                                        break;
                                    case "changes":
                                        changes = xmlReader.Value;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Copies a folder recursively.
        /// </summary>
        /// <param name="sourceFolder">The source directory.</param>
        /// <param name="destFolder">The target directory.</param>
        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            foreach (string file in Directory.GetFiles(sourceFolder))
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, true);
            }

            foreach (string folder in Directory.GetDirectories(sourceFolder))
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        public static void WriteLog(Exception ex)
        {
            WriteLog(ex.Message);
            WriteLog(ex.StackTrace);
        }

        public static void WriteLog(String logLine)
        {
            string appDataDir = 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "TrueMount");
            string logFile = Path.Combine(appDataDir, "updater.log");
            using (StreamWriter sWriter = new StreamWriter(logFile, true))
            {
                sWriter.WriteLine(DateTime.Now.ToShortTimeString() +
                    " - " + logLine);
            }
        }
    }
}
