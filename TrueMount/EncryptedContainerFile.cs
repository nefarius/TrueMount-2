using System;

namespace TrueMount
{
    [Serializable()]
    class EncryptedContainerFile : EncryptedMedia
    {
        public string FileName { get; set; }

        public EncryptedContainerFile(string fileName)
            : base()
        {
            this.FileName = fileName;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(EncryptedContainerFile))
            {
                EncryptedContainerFile ecf = (EncryptedContainerFile)obj;
                if (ecf.FileName.Equals(this.FileName))
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
            return this.FileName;
        }
    }
}
