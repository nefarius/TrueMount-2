using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueMount
{
    class TrueCryptConfig
    {
        public string ExecutablePath { get; set; }
        public bool Background { get; set; }
        public bool Silent { get; set; }
        public bool Cache { get; set; }
        public bool Explorer { get; set; }
        public bool Beep { get; set; }
        public string CommandLineArguments
        {
            get
            {
                String cli_args = null;
                if (Background)
                    cli_args += "/q background ";
                if (Silent)
                    cli_args += "/s ";
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
            Background = true;
            Silent = true;
            Cache = true;
        }
    }
}
