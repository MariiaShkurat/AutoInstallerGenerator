namespace AutoInstallerGenerator.FilesTree
{
    public class FileNode
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public string FullPath { get; }
        public FileNode(string name, string directory, string fullPath)
        {
            Name = name;
            Directory = directory;
            FullPath = fullPath;
        }
    }
}
