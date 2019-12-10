namespace bmlTUX.Scripts.Managers {
    /// <summary>
    /// Outputs files based on events that it listens to.
    /// </summary>
    public class OutputManager {
        
        readonly OutputFile file;

        public OutputManager(OutputFile file) {
            this.file = file;
            Enable();
        }
        
        void Enable() {
            ExperimentEvents.OnOutputUpdated += OutputToFile;
        }
        
        public void Disable() {
            ExperimentEvents.OnOutputUpdated -= OutputToFile;
        }

        void OutputToFile(Outputtable output) {
            //file.OutputToFile(output);
        }
    }

// ReSharper disable once IdentifierTypo
}
