using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This experiment's implementation of outputting files
/// </summary>
public class CsvOutputManager : MonoBehaviour {

    //the dataFile object
    public OutputDataFile DataFile;

    readonly bool debugMode = false;
    readonly string debugPath = "C:/temp/debug.csv";
    

    /// <summary>
    /// Sets up the output file. Can specify debug mode which will output a debug file to the computer's temp folder.
    ///
    /// </summary>
    /// <param name="debugMode"></param>
    public void SetupFile(bool debugMode) {
        if (debugMode) {
            debugMode = true;
            return;
        }
        if (File.Exists(DataFile.FullPath)) {
            Debug.LogError("Output file already exists, please choose a new file name in OutputManager");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            return;
#else
            Application.Quit();
            return false;
#endif
        }
        else {
            
            return;
            
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// outputs a line to the DataFile
    /// </summary>
    /// <param name="outputString"></param>
    public void AddLine(string outputString) {
        StreamWriter streamWriter;
        if (debugMode) {
            streamWriter = new StreamWriter(debugPath, append: true);
        }
        else {
            streamWriter = new StreamWriter(DataFile.FullPath, append: true);
        }
         
        streamWriter.WriteLine(outputString);
        streamWriter.Close();
    }
}

