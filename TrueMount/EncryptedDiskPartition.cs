using System;
using System.Collections.Generic;
using System.Management;

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
        public ManagementObject DiskDrive { get; set; }
        public ManagementObject DiskPartition { get; set; }

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
