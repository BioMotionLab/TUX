    
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using bmlTUX.Scripts.ExperimentParts;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
 
public class PackageInstallDetector : AssetPostprocessor {

    const string PackageName = "bmlTUX";
    const string installDataFileName = "installData.json";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
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
    private static void InitializeOnLoad() {

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
            if (package.source == PackageSource.Registry) {
                if (package.name == PackageName) {
                    HandlePackageLoad(new Version(package.version));
                }
            }
        }
        
        
        
    }

    static string ProjectFolder => Path.GetDirectoryName(Path.Combine( Application.dataPath, "../" ));
    static string InstallDataFilePath => Path.Combine(FileLocationSettings.BaseTuxDocumentsFolderPath, ProjectFolder, installDataFileName);
    
    static void HandlePackageLoad(Version packageVersion) {

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
        body.AppendLine(header);

        body.AppendLine(
            "We hope you find our tool useful, and would love to hear about what you're working on as well as any feedback you might have.\nPlease email at adambebko@gmail.com");

        body.AppendLine("Please remember to cite our work. For commercial use, a commercial license must be obtained directly by contacting us.");
        
        if (packageVersion.CompareTo(new Version("1.0.9")) > 0) {
            body.AppendLine("Warning: version 1.0.10 and up contains breaking changes to older projects:\n \n" +
                            "To make custom block orders more stable, the way they are internally stored has changed. " +
                            "This unfortunately means all custom block orders must be deleted and recreated.");
        }
        
        EditorUtility.DisplayDialog(header, body.ToString(), "Ok");
    }

    [Serializable]
    public class InstallData {
        public string versionText = "0.0.0";
    }
}

