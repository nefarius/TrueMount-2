﻿using System;

namespace TrueMount
{
    [Serializable()]
    class UsbKeyDevice
    {
        public string Caption { get; set; }
        public uint Signature { get; set; }
        public uint PartitionIndex { get; set; }
        public bool IsActive { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(System.DBNull))
            {
                UsbKeyDevice kd = (UsbKeyDevice)obj;
                if (Caption == kd.Caption &&
                    Signature == kd.Signature &&
                    PartitionIndex == kd.PartitionIndex)
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
            return this.Caption;
        }
    }
}
