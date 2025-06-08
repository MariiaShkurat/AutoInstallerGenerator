namespace AutoInstallerGenerator.InstallerLocalization
{
    public interface ILocalization
    {
        string Name { get; set; }
        string Code { get; set; }
        string CultureCode { get; set; }
        string GetWxlPath(string wixRoot);
    }

    public class UkrainianLocalization : ILocalization
    {
        public string Name { get; set; } = "Ukrainian";
        public string Code { get; set; } = "1058";
        public string CultureCode { get; set; } = "uk-ua";
        public string GetWxlPath(string wixRoot) =>
            Path.Combine(wixRoot, $"{CultureCode}.wxl");
    }

    public class EnglishLocalization : ILocalization
    {
        public string Name { get; set; } = "English";
        public string Code { get; set; } = "1033";
        public string CultureCode { get; set; } = "en-us";

        public string GetWxlPath(string wixRoot) =>
            Path.Combine(wixRoot, $"{CultureCode}.wxl");
    }
}
