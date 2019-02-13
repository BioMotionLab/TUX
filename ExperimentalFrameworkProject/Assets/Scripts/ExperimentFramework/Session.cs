using UnityEngine;

public class Session {

    readonly string debugPath = Application.dataPath + "/Debug/debugFile";

    /// <summary>
    /// stores the output path
    /// </summary>
    public string OutputPath;

    /// <summary>
    /// retrieves the output path or debug path
    /// </summary>
    public string GetFinalizedOutputPath => DebugMode ? debugPath : OutputPath;

    public bool DebugMode;
    public int OrderChosenIndex;
    public string ParticipantId;
}