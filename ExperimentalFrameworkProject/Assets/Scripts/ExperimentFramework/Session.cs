using System.IO;
using UnityEngine;

public class Session {

    readonly string debugFolder = Application.dataPath + "/Debug";
    const string debugFileName = "debugFile";

    string outputFullPath;
    /// <summary>
    /// stores the output path
    /// </summary>
    public string OutputFullPath {
        get {
            outputFullPath = Path.Combine(OutputFolder, OutputFileName);
            return outputFullPath;
        }
    }

    string outputFileName = "";
    public string OutputFileName {
        get {
            if (DebugMode) {
                return debugFileName;
            }
            else {
                return outputFileName;
            }
        }
        set { outputFileName = value; }
    }

    string outputFolder = "";
    public string OutputFolder {
        get {
            if (DebugMode) {
                return debugFolder;
            }
            else {
                return outputFolder;
            }
        }
        set { outputFolder = value; }
    }

    bool debugMode;
    public bool DebugMode {
        get { return debugMode; }
        set {
            debugMode = value;
            if (debugMode) {
                BlockChosen = true;
            }
        }
    } 

    bool blockChosen = false;

    public bool BlockChosen {
        get { return blockChosen; }
        set {

            if (value == blockChosen) return;

            blockChosen = value;
            if (blockChosen) {
                Debug.Log($"Block order chosen: {OrderChosenIndex}");
                ExperimentEvents.BlockOrderSelected(OrderChosenIndex);
            }
        }
    }

    public int OrderChosenIndex = 0;
    public string ParticipantId = "Unamed";
}