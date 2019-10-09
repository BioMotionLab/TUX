using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BML_Utilities.PackageBuilder {
    
    
    /// <summary>
    /// Tool for automatically building unitypackages from folders in your project.
    /// </summary>
    [CreateAssetMenu]
    public class PackageBuilder : ScriptableObject {

        [SerializeField]
        public List<Object> Folders;

        [SerializeField]
        public List<string> FolderPaths = new List<string>();

        public string ExportPath;
        public string PackageName = "unamed_package";
        public string Version     = "0.0.0";

        string PrimaryDirectory => FolderPaths[0];
        string VersionFilePath => PrimaryDirectory + "//version_history.json";

        public string VersionOfLastBuild {
            get {
                if (!File.Exists(VersionFilePath)) return string.Empty;
                OldVersionData oldVersionData = LoadOldVersionData();
                PackageSaveData prevVersion = oldVersionData.OldPackages[0];
                return prevVersion.Version;
            }
        }

        /// <summary>
        /// Exports the specified folders as a unitypackage
        /// </summary>
        public void ExportPackage() {
            
            string fullPath = ExportPath + "//" + PackageName + "_" + Version + ".unitypackage";
            Debug.Log($"Building Package: {fullPath}");
            Directory.CreateDirectory(ExportPath);
            CreateVersionFile();
            
            AssetDatabase.ExportPackage(FolderPaths.ToArray(),fullPath ,ExportPackageOptions.Recurse);
            Debug.Log($"Finished Building Package: {fullPath}");
        }

        OldVersionData LoadOldVersionData() {
            string loadedJson = File.ReadAllText(VersionFilePath);
            OldVersionData oldVersionData = JsonUtility.FromJson<OldVersionData>(loadedJson);
            return oldVersionData;
        }
        
        
        void CreateVersionFile() {
            OldVersionData oldVersionData = File.Exists(VersionFilePath) ? 
                LoadOldVersionData() : 
                new OldVersionData();

            
            PackageSaveData saveVersionData = new PackageSaveData(PackageName, Version, ExportPath, FolderPaths);
            
            oldVersionData.OldPackages.Insert(0, saveVersionData);
            string json = JsonUtility.ToJson(oldVersionData);
            File.WriteAllText(VersionFilePath, json);
        }


        public bool IsOldVersion(string versionStringValue) {
            if (!File.Exists(VersionFilePath)) return false;
            
            OldVersionData oldVersionData = LoadOldVersionData();
            foreach (PackageSaveData oldPackage in oldVersionData.OldPackages) {
                if (oldPackage.Version == versionStringValue) {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public class PackageSaveData {
        
        public List<string> FolderPaths = new List<string>();

        public string ExportPath;
        public string PackageName = "unamed_package";
        public string Version;

        public PackageSaveData(string packageName, string version, string exportPath, List<string> folderPaths) {
            FolderPaths = folderPaths;
            ExportPath = exportPath;
            Version = version;
            PackageName = packageName;
        }
    }
    
    [Serializable]
    public class OldVersionData {
            
        [SerializeField]
        public List<PackageSaveData> OldPackages = new List<PackageSaveData>();

            
            
    }
}