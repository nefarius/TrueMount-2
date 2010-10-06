using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Reflection;

namespace TrueMount
{
    class AutoUpdater
    {
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
        public delegate void OnDownloadProgressChangedEventHandler(int downloadProgress);
        public event OnDownloadProgressChangedEventHandler OnDownloadProgressChanged;
        private string changes = string.Empty;
        public string ChangeLog
        {
            get
            {
                String temp = string.Empty;
                foreach (string part in changes.Split('|'))
                    temp += part + Environment.NewLine;
                return temp;
            }
            set
            {
                changes = value;
            }
        }

        public bool DownloadNewVersion()
        {
            // without the url to the archive this can't do anything usefull
            if (string.IsNullOrEmpty(this.zipUrl))
                return false;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(this.zipUrl);
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

            // extract every file in zip archiv
            foreach (ZipEntry zipEntry in zipFile)
            {
                // we don't need directories, they will be created
                if (!zipEntry.IsFile)
                    continue;

                Stream zipStream = zipFile.GetInputStream(zipEntry);
                // full file path + name (in the AppData update folder)
                string fileName = Path.Combine(Configuration.UpdateSavePath, zipEntry.Name);
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

            return true;
        }

        public bool NewVersionAvailable
        {
            get
            {
                // get the running version
                Version curVersion = Assembly.GetExecutingAssembly().GetName().Version;
                // compare the versions
                if (curVersion.CompareTo(NewVersion) < 0)
                    return true;
                return false;
            }
        }

        public bool DownloadVersionInfo()
        {
            XmlTextReader xmlReader = null;
            try
            {
                xmlReader = new XmlTextReader(Configuration.UpdateVersionFileURL);
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
            catch { return false; }
            // always close the file/stream
            finally { if (xmlReader != null) xmlReader.Close(); }

            return true;
        }
    }
}
