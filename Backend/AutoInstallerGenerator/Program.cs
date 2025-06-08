using System.Text.Json;

namespace AutoInstallerGenerator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--config")
            {
                string configPath = args[1];

                if (!File.Exists(configPath))
                {
                    Console.WriteLine($"Config file not found: {configPath}");
                    return;
                }

                try
                {
                    await InstallerAutoRunner.RunFromConfig(configPath);
                    Console.WriteLine("Installer build completed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading config or building installer: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Usage: dotnet AutoInstallerGenerator.dll --config <path-to-installer.settings.json>");
            }
        }
    }
}
