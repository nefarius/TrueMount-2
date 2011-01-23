using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueMount
{
    [Serializable()]
    class TrueCryptConfig
    {
        public string ExecutablePath { get; set; }
        public string LauncherPath { get; set; }
        public bool Cache { get; set; }
        public bool Explorer { get; set; }
        public bool Beep { get; set; }
        public bool ShowErrors { get; set; }
        public string CommandLineArguments
        {
            get
            {
                String cli_args = "/q ";
                if (Cache)
                    cli_args += "/c ";
                if (Explorer)
                    cli_args += "/e ";
                if (Beep)
                    cli_args += "/b ";
                return cli_args;
            }
        }

        public TrueCryptConfig()
        {
            Cache = true;
            ShowErrors = true;
        }
    }
}
