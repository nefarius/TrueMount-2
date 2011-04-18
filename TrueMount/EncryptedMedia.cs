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
        public bool NextFreeLetter { get; set; }
        public bool RandomFreeLetter { get; set; }
        public bool Readonly { get; set; }
        public bool Removable { get; set; }
        public bool Timestamp { get; set; }
        public bool System { get; set; }
        public bool TriggerDismount { get; set; }
        public bool FetchUserPassword { get; set; }
        public string DynamicDriveLetter
        {
            get
            {
                if (NextFreeLetter)
                    return SystemDevices.FreeDriveLetters.FirstOrDefault();
                if (RandomFreeLetter)
                    return SystemDevices.RandomFreeDriveLetter;
                if (!string.IsNullOrEmpty(DriveLetter))
                    return DriveLetter;

                return string.Empty;
            }
        }
        public string DriveLetterCurrent { get; set; }
        public string DriveletterMasked
        {
            get
            {
                if (NextFreeLetter || RandomFreeLetter)
                    return "?";
                if (!string.IsNullOrEmpty(DriveLetter))
                    return DriveLetter;

                return "?";
            }
        }

        public string KeyFilesArgumentLine
        {
            get
            {
                String args = string.Empty;
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
                String mOpts = string.Empty;
                if (Readonly)
                    mOpts += "/m ro ";
                if (Removable)
                    mOpts += "/m rm ";
                if (Timestamp)
                    mOpts += "/m ts ";
                if (System)
                    mOpts += "/m sm ";

                if (!string.IsNullOrEmpty(mOpts))
                    return mOpts.Trim();
                return mOpts;
            }
        }

        public EncryptedMedia()
        {
            this.KeyFiles = new List<string>();
            this.PasswordFile = String.Empty;
        }
    }
}
