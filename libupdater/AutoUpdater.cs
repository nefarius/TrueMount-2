using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace TrueMount
{
    public class AutoUpdater
    {
        // in newVersion variable we will store the
        // version info from xml file
        private Version newVersion = null;
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

        public bool DownloadNewVersion(String targetLocation)
        {
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
                    while ((byteSize = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, byteSize);
                        byteDownloaded += byteSize;
                        percentDone = (byteDownloaded * 100) / (int)httpResponse.ContentLength;
                        this.OnDownloadProgressChanged.Invoke(percentDone);
                    }

                    responseStream.Close();
                    zipFile = new ZipFile(memoryStream);
                }
            }

            foreach (ZipEntry zipEntry in zipFile)
            {
                if (!zipEntry.IsFile)
                    continue;

                Stream zipStream = zipFile.GetInputStream(zipEntry);
                string fileName = Path.Combine(targetLocation, zipEntry.Name);
                string dirName = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

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
                Version curVersion =
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // compare the versions
                if (curVersion.CompareTo(newVersion) < 0)
                    return true;
                return false;
            }
        }

        public bool DownloadVersionInfo(String updateFileURL)
        {
            XmlTextReader xmlReader = null;
            try
            {
                xmlReader = new XmlTextReader(updateFileURL);
                // simply (and easily) skip the junk at the beginning
                xmlReader.MoveToContent();
                // internal - as the XmlTextReader moves only
                // forward, we save current xml element name
                // in elementName variable. When we parse a
                // text node, we refer to elementName to check
                // what was the node name
                string elementName = string.Empty;
                // we check if the xml starts with a proper
                // "ourfancyapp" element node
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
                                        newVersion = new Version(xmlReader.Value);
                                        break;
                                    case "url":
                                        zipUrl = xmlReader.Value;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch { return false; }
            finally { if (xmlReader != null) xmlReader.Close(); }

            return true;
        }
    }
}
