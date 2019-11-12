using bmlTUX.Scripts.Managers;

namespace bmlTUX.Scripts.ExperimentParts {
    public class DebugSession : Session {
        public DebugSession(FileLocationSettings fileLocations) : base(fileLocations) {
            OutputFile = OutputFile.DebugFile(fileLocations);
            SelectedDesignFilePath = "";
            BlockOrderChosenIndex  = 0;
        }
    }
}