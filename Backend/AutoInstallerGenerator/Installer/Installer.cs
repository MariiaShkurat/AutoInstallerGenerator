using AutoInstallerGenerator.FilesTree;
using AutoInstallerGenerator.InstallerLocalization;

namespace AutoInstallerGenerator.InstallerConfiguration
{
    public class Installer
    {
        public string Name { get; set; }
        public ILocalization Localization { get; set; }
        public bool CreateShortcutCheckbox { get; set; }
        public bool RequiresSerialCode { get; set; }
        public string OutputDirectory { get; set; }

        private IList<FileNode> files = new List<FileNode>();
        public IList<FileNode> Files
        {
            set => files = value;
            get => files;
        }

        public Installer(string name, ILocalization localization, bool createShortcutCheckbox, bool requiresSerialCode, string outputDirectory)
        {
            Name = name;
            Localization = localization;
            CreateShortcutCheckbox = createShortcutCheckbox;
            RequiresSerialCode = requiresSerialCode;
            OutputDirectory = outputDirectory;
        }

        public void AddFile(FileNode file)
        {
            Files.Add(file);
        }
    }
}
