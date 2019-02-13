using System.IO;
using UnityEngine;

/// <summary>
/// Outputs files based on events that it listens to.
/// </summary>
public class OutputManager {

    readonly string outputPath;


    /// <summary>
    /// Creates an OutputFile manager instance to output strings as a file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="overwrite"></param>
    /// <param name="extension"></param>
    public OutputManager(string path, bool overwrite = false, string extension = FileExtensions.CSV) {
        outputPath = path;

        Debug.Log($"path before extension {outputPath}");
        if (!Path.HasExtension(outputPath)) {
            Debug.Log($"path no extension: {outputPath}");
            outputPath += extension;
            Debug.Log($"path after extension add: {outputPath}");
        }

        if (File.Exists(outputPath) && !overwrite) {
            Debug.LogError("OutputFile file already exists, please choose new name");
        }
        Enable();
    }

    /// <summary>
    /// Must be called when instance created
    /// </summary>
    void Enable() {
        ExperimentEvents.OnOutputUpdated += OutputFile;
    }

    /// <summary>
    /// *Important!*
    /// Must be called when instance no longer needed to prevent memory leak
    /// </summary>
    public void Disable() {
        ExperimentEvents.OnOutputUpdated -= OutputFile;
    }

    /// <summary>
    /// Outputs the Outputtable as a file
    /// </summary>
    /// <param name="output"></param>
    void OutputFile(Outputtable output) {
        Debug.Log($"Writing to file {outputPath}");
        Debug.Log("***********");
        Debug.Log("");
        string folder = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(folder))
            if (folder != null)
                Directory.CreateDirectory(folder);

        using (StreamWriter streamWriter = new StreamWriter(outputPath)) {
            streamWriter.Write(output.AsString);
        }

    }

}

// ReSharper disable once IdentifierTypo
/// <summary>
/// Defines a class that can be output as a string.
/// </summary>
public interface Outputtable {
    string AsString { get; }
}

/// <summary>
/// Defines supported file extensions.
/// </summary>
public static class FileExtensions {
    // ReSharper disable once InconsistentNaming
    public const string CSV = ".csv";
}



