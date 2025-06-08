using System.Diagnostics;
using System.Reflection;
using AutoInstallerGenerator.InstallerConfiguration;
using AutoInstallerGenerator.InstallerLocalization;

namespace AutoInstallerGenerator.Builders
{
    public class InstallerDirector
    {
        private IBuilder installerBuilder;
        private Installer installerInfo;

        public InstallerDirector(IBuilder installerBuilder, Installer installerInfo)
        {
            this.installerBuilder = installerBuilder;
            this.installerInfo = installerInfo;
        }

        public (bool Success, string BuildLog, string MsiPath) Run(string wixResourcesPath)
        {
            installerBuilder.ChangeProductName(installerInfo.Name);
            installerBuilder.ReplaceComponentsWithFiles(installerInfo.Files.ToList());
            if (installerInfo.CreateShortcutCheckbox)
                installerBuilder.AddInstallShortcutControl();
            if (installerInfo.RequiresSerialCode)
                installerBuilder.AddLicenseKeyCheckDialog();
            string wixInstaller = installerBuilder.GetResult();
            File.WriteAllText(Path.Combine(wixResourcesPath, "Product_Build.wxs"), wixInstaller);
            
            return this.BuildWiXProject(wixResourcesPath);
        }

        public (bool Success, string BuildLog, string MsiPath) BuildWiXProject(string wixResourcesPath)
        {
            var buildLog = String.Empty;

            string candlePath = Path.Combine(wixResourcesPath, "candle.exe");
            string lightPath = Path.Combine(wixResourcesPath, "light.exe");
            string sourcePath = Path.Combine(wixResourcesPath, "Product_Build.wxs");
            string outputObjPath = Path.Combine(installerInfo.OutputDirectory, "intermediateFile.wixobj");
            string outputMsiPath = Path.Combine(installerInfo.OutputDirectory, $"{installerInfo.Name}.msi");
            string outputPdbPath = Path.ChangeExtension(outputMsiPath, ".wixpdb");

            int? candleExit = null;
            int? lightExit = null;

            string serialCodeValidationTargetDir = wixResourcesPath;

            var candleStartInfo = new ProcessStartInfo
            {
                FileName = candlePath,
                Arguments = $"-ext WixUIExtension -dSerialCodeValidation.TargetDir=\"{serialCodeValidationTargetDir}\" -o \"{outputObjPath}\" \"{sourcePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = candleStartInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                buildLog += $"Candle Exit Code: {process.ExitCode}. ";
                buildLog += $"Candle Output: {output}. ";

                candleExit = process.ExitCode;
                if (!string.IsNullOrEmpty(error)) buildLog += $"Candle Error: {error}. ";
            }

            var lightStartInfo = new ProcessStartInfo
            {
                FileName = lightPath,
                Arguments = $"-ext WixUIExtension -loc \"{installerInfo.Localization.GetWxlPath(wixResourcesPath)}\" \"{outputObjPath}\" -cultures:\"{installerInfo.Localization.CultureCode}\" -o \"{outputMsiPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = lightStartInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                buildLog += $"Light Exit Code: {process.ExitCode}. ";
                buildLog += $"Light Output: {output}. ";

                lightExit = process.ExitCode;
                if (!string.IsNullOrEmpty(error)) buildLog += $"Light Error: {error}. ";
            }

            try
            {
                if (File.Exists(outputObjPath))
                    File.Delete(outputObjPath);
                if (File.Exists(outputPdbPath))
                    File.Delete(outputPdbPath);
            }
            catch (Exception ex)
            {
                buildLog += $"Cleanup Warning: {ex.Message}. ";
            }

            bool ok = candleExit == 0 && lightExit == 0;
            string msi = Path.Combine(installerInfo.OutputDirectory, $"{installerInfo.Name}.msi");
            return (ok, buildLog, msi);
        }

    }
}
