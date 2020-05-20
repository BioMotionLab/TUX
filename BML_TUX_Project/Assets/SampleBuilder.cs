using System.IO;
using UnityEngine;

[CreateAssetMenu]
public class SampleBuilder : ScriptableObject
{
    
    [SerializeField] public string baseFolderName;

    public void BuildSamples() {

        string baseFolder = Path.Combine(Application.dataPath, baseFolderName);
        string sourceSampleFolder =  Path.Combine(baseFolder, "Samples");
        string destinationSampleFolder = Path.Combine(baseFolder, "Samples~");
        if (!Directory.Exists(sourceSampleFolder)) Debug.LogError("Samples folder not found", this);

        Debug.Log($"source:{sourceSampleFolder}");
        Debug.Log($"destination:{destinationSampleFolder}");
        
        if (Directory.Exists(destinationSampleFolder)) {
            Debug.LogError("Samples~ folder exists, deleting old version.", this);
            Directory.Delete(destinationSampleFolder);
        }
        
        DirectoryCopy(sourceSampleFolder, destinationSampleFolder);
        Debug.Log("Built Samples", this);
    }
    
 
    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
    {
        Debug.Log(sourceDirName);
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs) {
            foreach (DirectoryInfo subdir in dirs) {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
    
}