using System.IO;
using UnityEngine;

/// <summary>
/// This experiment's implementation of outputting files
/// </summary>
public class OutputManager {

    string DebugPath = Application.dataPath + "/Debug/debugFile";
    readonly string outputPath;

    public OutputManager(string path, bool debugMode = false, string extension = FileExtensions.CSV) {
        this.outputPath = debugMode? DebugPath : path;

        Debug.Log($"path before extension {outputPath}");
        if (!Path.HasExtension(outputPath)) {
            Debug.Log($"path no extension: {outputPath}");
            outputPath += extension;
            Debug.Log($"path after extension add: {outputPath}");
        }
        
        if (File.Exists(outputPath) && !debugMode) {
            Debug.LogError("Output file already exists, please choose new name");
        }
        Enable();
    }


    void Enable() {
        ExperimentEvents.OnOutputUpdated += Output;
    }

    public void Disable() {
        ExperimentEvents.OnOutputUpdated -= Output;
    }

    void Output(Outputtable output) {
        Debug.Log($"Writing to file {outputPath}");

        string folder = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        using (StreamWriter streamWriter = new StreamWriter(outputPath)) {
            streamWriter.Write(output.AsString);
        } 
        
    }

}

public interface Outputtable {
    string AsString { get; }
}

public static class FileExtensions {
    public const string CSV = ".csv";
}



