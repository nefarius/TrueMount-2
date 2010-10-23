using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueMount
{
    [Serializable()]
    class EncryptedMedia
    {
        public bool IsActive { get; set; }
        public bool OpenExplorer { get; set; }
        public string PasswordFile { get; set; }
        public List<String> KeyFiles { get; set; }
        public string DriveLetter { get; set; }
        public bool Readonly { get; set; }
        public bool Removable { get; set; }
        public bool Timestamp { get; set; }
        public bool System { get; set; }
        public bool TriggerDismount { get; set; }

        public string KeyFilesArgumentLine
        {
            get
            {
                String args = null;
                if (this.KeyFiles.Count > 0)
                {
                    foreach (string item in this.KeyFiles)
                        args += "/k \"" + item + "\" ";
                    return args.Trim();
                }
                else
                    return args;
            }
        }

        public string MountOptions
        {
            get
            {
                String m_opts = null;
                if (Readonly)
                    m_opts += "/m ro ";
                if (Removable)
                    m_opts += "/m rm ";
                if (Timestamp)
                    m_opts += "/m ts ";
                if (System)
                    m_opts += "/m sm ";

                if (!string.IsNullOrEmpty(m_opts))
                    return m_opts.Trim();
                return m_opts;
            }
        }

        public EncryptedMedia()
        {
            this.KeyFiles = new List<string>();
            this.PasswordFile = String.Empty;
        }
    }
}
