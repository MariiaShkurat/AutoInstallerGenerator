using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoInstallerGenerator
{
    public class InstallerSettings
    {
        public string ProjectName { get; set; }
        public string WatchPath { get; set; }
        public string OutputInstallerPath { get; set; }
        public string Localization { get; set; } = "en-us";
        public bool RequiresSerialCode { get; set; }
        public bool CreateShortcut { get; set; }
    }
}
