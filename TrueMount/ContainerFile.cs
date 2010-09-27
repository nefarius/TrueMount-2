using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueMount
{
    [Serializable()]
    class ContainerFile
    {
        public string FileName { get; set; }
        public bool IsActive { get; set; }
        public bool OpenExplorer { get; set; }
        public string PasswordFile { get; set; }
        public List<String> KeyFiles { get; set; }
        public string DriveLetter { get; set; }
        public bool Readonly { get; set; }
        public bool Removable { get; set; }
        public bool Timestamp { get; set; }
        public bool System { get; set; }
    }
}
