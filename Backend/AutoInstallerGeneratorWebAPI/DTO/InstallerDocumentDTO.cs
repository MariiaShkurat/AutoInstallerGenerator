namespace AutoInstallerGeneratorWebAPI.DTO
{
    public class InstallerDocumentDTO
    {
        public IFormFile File { get; set; } = default!;
        public string ProjectName { get; set; } = "";
        public string Culture { get; set; } = "en";
        public string Log { get; set; } = "";
    }
}
