using System.Xml.Linq;
using AutoInstallerGenerator.FilesTree;

namespace AutoInstallerGenerator.Builders
{
    public interface IBuilder
    {
        void ChangeProductName(string name);
        void ReplaceComponentsWithFiles(List<FileNode> files);
        void AddInstallShortcutControl();
        void AddLicenseKeyCheckDialog();
        string GetResult();
    }

    public class InstallerBuilder : IBuilder
    {
        private XDocument wixDocument;

        public InstallerBuilder(string filePath)
        {
            wixDocument = XDocument.Load(filePath);
        }

        public void ChangeProductName(string newName)
        {
            XElement productElement = wixDocument.Descendants().FirstOrDefault(e => e.Name.LocalName == "Product");
            XAttribute nameAttribute = productElement.Attribute("Name");
            nameAttribute.Value = newName;
        }

        public void ReplaceComponentsWithFiles(List<FileNode> files)
        {
            XNamespace wix = "http://schemas.microsoft.com/wix/2006/wi";
            XElement componentGroup = wixDocument.Descendants().Where(e => e.Name.LocalName == "ComponentGroup").FirstOrDefault(x => x.Attribute("Id")?.Value == "InstallComponents");
            if (componentGroup.Descendants("Component").Any())
                componentGroup.Descendants("Component").Remove();

            foreach (var file in files)
            {
                XElement componentElement = new XElement(wix + "Component", new XAttribute("Id", $"FileComponent_{files.IndexOf(file)}"),  // Add necessary attributes
                    new XElement(wix + "File", new XAttribute("Source", file.FullPath)));

                componentGroup.Add(componentElement);
            }
        }

        public void AddInstallShortcutControl()
        {
            XNamespace wix = "http://schemas.microsoft.com/wix/2006/wi";
            XElement parentControl = wixDocument.Descendants().FirstOrDefault(x => x.Attribute("Id")?.Value == "InstallDirAndOptionalShortcutDlg");

            if (parentControl.Descendants().FirstOrDefault(e => e.Attribute("Id")?.Value == "InstallShortcutCheckbox") == null)
            {
                XElement newInstallShortcutControl = new XElement(wix + "Control",
                    new XAttribute("Id", "InstallShortcutCheckbox"),
                    new XAttribute("Type", "CheckBox"),
                    new XAttribute("X", "20"),
                    new XAttribute("Y", "205"),
                    new XAttribute("Width", "200"),
                    new XAttribute("Height", "17"),
                    new XAttribute("Property", "INSTALLSHORTCUT"),
                    new XAttribute("CheckBoxValue", "1"),
                    new XAttribute("Text", "!(loc.DesktopShortcut)")
                );

                parentControl.Add(newInstallShortcutControl);
            }
        }

        public void AddLicenseKeyCheckDialog()
        {
            XNamespace wix = "http://schemas.microsoft.com/wix/2006/wi";
            XElement WorkflowUI = wixDocument.Descendants().Where(e => e.Name.LocalName == "UI").FirstOrDefault(x => x.Attribute("Id")?.Value == "CustomWorkflow");
            XElement licenseAgreementDlgNext = wixDocument.Descendants().FirstOrDefault(e => e.Attribute("Dialog")?.Value == "LicenseAgreementDlg" && e.Attribute("Control")?.Value == "Next");
            licenseAgreementDlgNext.SetAttributeValue("Value", "LicenseKeyDlg");
            XElement InstallDirectoriesDlgBack = wixDocument.Descendants().FirstOrDefault(e => e.Attribute("Dialog")?.Value == "InstallDirAndOptionalShortcutDlg" && e.Attribute("Control")?.Value == "Back");
            InstallDirectoriesDlgBack.SetAttributeValue("Value", "LicenseKeyDlg");

            XElement backPublishLicenseKeyDlg = new XElement(wix + "Publish",
                new XAttribute("Dialog", "LicenseKeyDlg"),
                new XAttribute("Control", "Back"),
                new XAttribute("Event", "NewDialog"),
                new XAttribute("Value", "LicenseAgreementDlg"),
                "LicenseAccepted = \"1\"");

            XElement nextPublishLicenseKeyDlg = new XElement(wix + "Publish",
                new XAttribute("Dialog", "LicenseKeyDlg"),
                new XAttribute("Control", "Next"),
                new XAttribute("Event", "NewDialog"),
                new XAttribute("Value", "InstallDirAndOptionalShortcutDlg"),
                new XAttribute("Order", "2"),
                "PIDACCEPTED = \"1\"");

            WorkflowUI.Add(backPublishLicenseKeyDlg, nextPublishLicenseKeyDlg);
        }

        public string GetResult()
        {
            return wixDocument.ToString();
        }
    }
}