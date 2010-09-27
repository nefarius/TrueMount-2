using System;
using System.Collections.Generic;

namespace TrueMount
{
    [Serializable()]
    class EncryptedDiskPartition : EncryptedMedia
    {
        public string DiskCaption { get; set; }
        public uint DiskSignature { get; set; }
        public uint PartitionIndex { get; set; }

        public EncryptedDiskPartition(string caption, uint signature, uint partitionNr)
            : base()
        {
            this.DiskCaption = caption;
            this.DiskSignature = signature;
            this.PartitionIndex = partitionNr;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(EncryptedDiskPartition))
            {
                EncryptedDiskPartition ep = (EncryptedDiskPartition)obj;

                if (DiskCaption == ep.DiskCaption &&
                    DiskSignature == ep.DiskSignature &&
                    PartitionIndex == ep.PartitionIndex)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.DiskCaption + ", Partition: " + this.PartitionIndex.ToString();
        }
    }
}
