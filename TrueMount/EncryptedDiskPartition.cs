using System;
using System.Collections.Generic;

namespace TrueMount
{
    class EncryptedDiskPartition
    {
        public string DiskCaption { get; set; }
        public uint DiskSignature { get; set; }
        public uint PartitionIndex { get; set; }
        public bool IsActive { get; set; }
        public bool OpenExplorer { get; set; }
        public string PasswordFile { get; set; }
        public List<String> KeyFiles { get; set; }
        public string DriveLetter { get; set; }
        public bool Readonly { get; set; }
        public bool Removable { get; set; }
        public bool Timestamp { get; set; }
        public bool System { get; set; }

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

        public EncryptedDiskPartition()
        {
            this.KeyFiles = new List<string>();
        }

        public EncryptedDiskPartition(string caption, uint signature, uint partition_nr)
        {
            this.DiskCaption = caption;
            this.DiskSignature = signature;
            this.PartitionIndex = partition_nr;
            this.KeyFiles = new List<string>();
        }

        public override bool Equals(object obj)
        {
            EncryptedDiskPartition ep = (EncryptedDiskPartition)obj;

            if (DiskCaption == ep.DiskCaption &&
                DiskSignature == ep.DiskSignature &&
                PartitionIndex == ep.PartitionIndex)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
