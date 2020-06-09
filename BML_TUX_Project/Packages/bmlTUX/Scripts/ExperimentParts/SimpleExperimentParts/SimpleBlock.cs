using System.Data;

namespace bmlTUX.Scripts.ExperimentParts.SimpleExperimentParts {
    
    /// <summary>
    /// This class is the simplest block possible.
    /// It is used as a default when no custom block is specified.
    /// </summary>
    public class SimpleBlock : Block {
        public SimpleBlock(ExperimentRunner runner,
                           DataTable trialTable, 
                           DataRow dataRow,
                           int index) :
            base(runner, 
                 trialTable, 
                 dataRow,
                 index) {
        }
        
    }
}