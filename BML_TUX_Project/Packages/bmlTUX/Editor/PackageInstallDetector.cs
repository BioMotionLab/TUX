using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using bmlTUX.Scripts.ExperimentParts;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Editor {
    public class PackageInstallDetector : AssetPostprocessor {

        const string PackageName = "com.biomotionlab.tux";
        const string InstallDataFileName = "installData.json";

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var inPackages = importedAssets.Any(path => path.StartsWith("Packages/")) ||
                             deletedAssets.Any(path => path.StartsWith("Packages/")) ||
                             movedAssets.Any(path => path.StartsWith("Packages/")) ||
                             movedFromAssetPaths.Any(path => path.StartsWith("Packages/"));
 
            if (inPackages)
            {
                InitializeOnLoad();
            }
        }
   
        [InitializeOnLoadMethod]
        static void InitializeOnLoad() {

            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
                Thread.Sleep(100);
 
            if (listRequest.Error != null)
            {
                Debug.Log("Error: " + listRequest.Error.message);
                return;
            }
            var packages = listRequest.Result;
            foreach (var package in packages)
            {
                if (package.name == PackageName) {
                    ThisPackageLoaded(new Version(package.version));
                }
            }
        
        }

        static string ProjectFolder => Path.GetDirectoryName(Path.Combine( Application.dataPath, "../" ));
        static string InstallDataFilePath => Path.Combine(FileLocationSettings.BaseTuxDocumentsFolderPath, ProjectFolder, InstallDataFileName);
    
        static void ThisPackageLoaded(Version packageVersion) {

            InstallData installData;
            if (File.Exists(InstallDataFilePath)) {
                installData = LoadInstallData();
            }
            else {
                installData = new InstallData();
            }

            Version savedVersion = new Version(installData.versionText);

            if (packageVersion.CompareTo(savedVersion) > 0) {
                HandleNewVersionOfPackageLoaded(packageVersion);
            }

        }

        static InstallData LoadInstallData() {
            string loadedText = File.ReadAllText(InstallDataFilePath);
            InstallData installData = JsonUtility.FromJson<InstallData>(loadedText);
            return installData;
        }

        static void SaveInstallData(InstallData installData) {
            string textToSave = JsonUtility.ToJson(installData);
            File.WriteAllText(InstallDataFilePath, textToSave);
        }

        static void HandleNewVersionOfPackageLoaded(Version packageVersion) {
            SaveInstallData(
                new InstallData {
                    versionText = packageVersion.ToString()
                });


            string header = $"Thank you for installing {PackageName} version {packageVersion}";
            Debug.Log(header);

            StringBuilder body = new StringBuilder();

            body.AppendLine(
                "We hope you find our tool useful, and we would love to hear about what you're working on as well as any feedback you might have");

            body.AppendLine("");
        
            body.AppendLine("Please remember to cite our work. Commercial-use requires a license.");
        
            body.AppendLine("");

            if (packageVersion.CompareTo(new Version("1.0.9")) > 0) {
                body.AppendLine("WARNING: Updating from an older version? Read this first:");
                body.AppendLine();
                body.AppendLine("Version 1.0.10+");
                body.AppendLine(
                    "- Improved manual block order workflow. This unfortunately means old configurations must be deleted and recreated.");
                body.AppendLine(
                    "- Improved settings customization workflow. Single settings file created with the helper tool alongside your design file. Existing projects must create one from Create menu, and drag into design file");
            }

            EditorUtility.DisplayDialog(header, body.ToString(), "Ok");
        }

        [Serializable]
        public class InstallData {
            public string versionText = "0.0.0";
        }
    }
}

