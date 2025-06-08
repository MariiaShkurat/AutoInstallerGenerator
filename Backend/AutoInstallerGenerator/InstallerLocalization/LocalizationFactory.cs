namespace AutoInstallerGenerator.InstallerLocalization
{
    public static class LocalizationFactory
    {
        private static readonly Dictionary<string, ILocalization> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["en-us"] = new EnglishLocalization(),
            ["uk-ua"] = new UkrainianLocalization()
        };

        public static ILocalization Get(string code) =>
            _map.TryGetValue(code, out var loc) ? loc : _map["en-us"];
    }
}
