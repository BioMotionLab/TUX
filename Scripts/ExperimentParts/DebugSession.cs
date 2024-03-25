namespace bmlTUX {
    public class DebugSession : Session {
        public DebugSession(string debugFolder, string debugFileName) {
            OutputFile = OutputFile.DebugFile(debugFolder, debugFileName);
            SelectedDesignFilePath = "";
            BlockOrderChosenIndex  = 0;
        }
    }
}