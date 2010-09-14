using System;
using System.Collections.Generic;

namespace TrueMount
{
    class UsbKeyDevice
    {
        public string Caption { get; set; }
        public uint Signature { get; set; }
        public uint PartitionIndex { get; set; }
        public bool IsActive { get; set; }

        public override bool Equals(object obj)
        {
            UsbKeyDevice kd = (UsbKeyDevice)obj;
            if (Caption == kd.Caption &&
                Signature == kd.Signature &&
                PartitionIndex == kd.PartitionIndex)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
