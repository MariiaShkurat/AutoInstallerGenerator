using System.Net.Http.Headers;
using System.Text.Json;
using AutoInstallerGenerator;
using AutoInstallerGenerator.Builders;
using AutoInstallerGenerator.FilesTree;
using AutoInstallerGenerator.InstallerConfiguration;
using AutoInstallerGenerator.InstallerLocalization;

public static class InstallerAutoRunner
{
    public static async Task RunFromConfig(string configPath)
    {
        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<InstallerSettings>(json);

        if (config == null)
            throw new Exception("Invalid configuration");

        string projectDir = Path.GetDirectoryName(configPath)!;

        if (string.IsNullOrWhiteSpace(config.WatchPath))
        {
            config.WatchPath = DetectPublishFolder(projectDir) ??
                throw new DirectoryNotFoundException("Publish folder is not found. Configure Watch path in configuration");

            Console.WriteLine($"WatchPath is not found in configuration -> using: {config.WatchPath}");
        }

        ILocalization localization = LocalizationFactory.Get(config.Localization);

        var installer = new Installer(config.ProjectName,
                                      localization,
                                      config.CreateShortcut,
                                      config.RequiresSerialCode,
                                      config.OutputInstallerPath);

        var files = Directory.GetFiles(config.WatchPath);
        foreach (var file in files)
        {
            installer.AddFile(new FileNode(Path.GetFileName(file), Path.GetDirectoryName(file)!, file));
        }

        string wixResourcesPath = Path.Combine(config.WatchPath, "WiXResources");
        var builder = new InstallerBuilder(Path.Combine(wixResourcesPath, "Product_Sample.wxs"));
        var director = new InstallerDirector(builder, installer);
        var (success, buildLog, msiPath) = director.Run(wixResourcesPath);

        if (success && File.Exists(msiPath))
            await PostInstallerAsync(apiUrl: "https://localhost:7103",
                                     msiPath: msiPath,
                                     buildLog: buildLog,
                                     project: installer.Name,
                                     culture: installer.Localization.CultureCode);
    }

    private static string? DetectPublishFolder(string projectDir)
    {
        var publishes = Directory.EnumerateDirectories( Path.Combine(projectDir, "bin"), "publish", SearchOption.AllDirectories);

        return publishes.Select(p => new DirectoryInfo(p))
                        .OrderByDescending(d => d.LastWriteTimeUtc)
                        .FirstOrDefault()?.FullName;
    }

    private static async Task PostInstallerAsync(string apiUrl, string msiPath, string buildLog, string project, string culture)
    {
        await using var msi = File.OpenRead(msiPath);

        using var form = new MultipartFormDataContent
        {
            { new StreamContent(msi)
            { Headers = { ContentType = new MediaTypeHeaderValue("application/octet-stream") } },
                "file", Path.GetFileName(msiPath) },

            { new StringContent(project), "projectName" },
            { new StringContent(culture), "culture" },
            { new StringContent(buildLog), "log" }
        };

        using var client = new HttpClient();
        var resp = await client.PostAsync($"{apiUrl.TrimEnd('/')}/api/installers", form);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"Installer uploaded -> {json}");
    }
}
