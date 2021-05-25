using bmlTUX.Scripts.Managers;

namespace bmlTUX.Scripts.ExperimentParts {
    public class DebugSession : Session {
        public DebugSession() {
            OutputFile = OutputFile.DebugFile();
            SelectedDesignFilePath = "";
            BlockOrderChosenIndex  = 0;
        }
    }
}