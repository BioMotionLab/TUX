namespace bmlTUX {
    public class DebugSession : Session {
        public DebugSession(FileLocationSettings fileLocationSettings) {
            OutputFile = OutputFile.DebugFile(fileLocationSettings.DebugFolder, fileLocationSettings.DebugFileName);
            SelectedDesignFilePath = "";
            BlockOrderChosenIndex  = 0;
        }
    }
}