namespace bmlTUX {
    public class DebugSession : Session {
        public DebugSession(FileLocationSettings fileLocationSettings) :base(fileLocationSettings){
            OutputFile = OutputFile.DebugFile(fileLocationSettings.DebugFolder, fileLocationSettings.DebugFileName);
            SelectedDesignFilePath = "";
            BlockOrderChosenIndex  = 0;
        }
    }
}