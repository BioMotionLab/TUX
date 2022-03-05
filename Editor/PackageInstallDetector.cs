using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace bmlTUX.Editor {
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
            ThisPackageLoaded();
        }

        static Version GetPackageVersion()
        {
            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
                Thread.Sleep(100);
 
            if (listRequest.Error != null)
            {
                Debug.Log("Error: " + listRequest.Error.message);
                return null;
            }
            var packages = listRequest.Result;
            foreach (var package in packages)
            {
                if (package.name == PackageName)
                {
                    return new Version(package.version);
                }
            }

            return null;
        }

        static string ProjectFolder => Path.GetDirectoryName(Path.Combine( Application.dataPath, "../" ));
        static string InstallDataFilePath => Path.Combine(FileLocationSettings.BaseTuxDocumentsFolderPath, ProjectFolder, InstallDataFileName);
    
        static void ThisPackageLoaded()
        {
            var packageVersion = GetPackageVersion();
            if (packageVersion == null) return;
            InstallData installData;
            if (File.Exists(InstallDataFilePath)) {
                installData = LoadInstallData();
            }
            else {
                installData = new InstallData();
            }

            Version savedVersion = new Version(installData.versionText);

            if (packageVersion.CompareTo(savedVersion) > 0) {
                ShowVersionUpdateDialog();
                SaveInstallData(
                    new InstallData {
                        versionText = packageVersion.ToString()
                    });
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
        
        
        [MenuItem(MenuNames.BmlMainMenu + "About bmlTUX & What's New?")] 
        public static void ShowVersionUpdateDialog() {
            
            var packageVersion = GetPackageVersion();

            string header = $"Thank you for installing {PackageName} version {packageVersion}";
            Debug.Log(header);

            StringBuilder body = new StringBuilder();

            body.AppendLine(
                "We hope you find our tool useful, and we would love to hear about what you're working on as well as any feedback you might have");

            body.AppendLine("");
        
            body.AppendLine("Please remember to cite our work. Commercial-use requires a license.");
        
            body.AppendLine("");

            if (VersionIsLaterThan(packageVersion, "1.0.0"))
            {
                body.AppendLine("WARNING: Major version update 3.0.0. Breaking Changes");
                body.AppendLine();
                body.AppendLine("Version 3.0.0 Release");
                body.AppendLine();
                body.AppendLine("- Major pass on cleaning up primary namespaces. Almost everything is now in bmlTUX namespace");
                body.AppendLine("- Major feature: Support for continuous data recording and playback, see documentation for more details");
                body.AppendLine(
                    "- Can now start an experiment from code, bypassing the starting UI. This is especially useful for experiments on standalone VR headsets.");
                body.AppendLine("- Major pass on documentation fixes/clarifications");
                body.AppendLine("- Several quality of life improvements and bug fixes, including several contributions from the community. Special thanks to A-Ivan, and DerMilchmann");
                body.AppendLine("");
                body.AppendLine("Note on future maintenance: I have recently left BioMotionLab for a new job, and therefore will have less time to devote to this project. " +
                                "However, I'm happy with its current state and stability. I will still be providing bug fixes, and accepting community contributions on GitHub, " +
                                "but I will not be providing any major new features moving forward. If you have a new feature request that is important for your work, or any other questions, " +
                                "please reach out to myself (Adam Bebko) and/or Nikolaus Troje.");
            }

            EditorUtility.DisplayDialog(header, body.ToString(), "Ok");
        }

        static bool VersionIsLaterThan(Version currentVersion, string versionToCompare)
        {
            return currentVersion.CompareTo(new Version(versionToCompare)) > 0;
        }

        [Serializable]
        public class InstallData {
            public string versionText = "0.0.0";
        }
    }
}

