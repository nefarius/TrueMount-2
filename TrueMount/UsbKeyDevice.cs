using System;
using System.Collections.Generic;

namespace TrueMount
{
    class UsbKeyDevice
    {
        public string Caption { get; set; }
        public uint Signature { get; set; }
        public uint PartitionIndex { get; set; }
        private string _PasswordFile = null;
        public string PasswordFile
        {
            get { return this._PasswordFile; }
            set { this._PasswordFile = value.Substring(2); }
        }
        public List<String> KeyFiles { get; set; }
        public bool IsActive { get; set; }

        public UsbKeyDevice()
        {
            KeyFiles = new List<string>();
        }

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
