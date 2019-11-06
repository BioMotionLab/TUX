using System.Data;

namespace BML_TUX.Scripts.ExperimentParts.SimpleExperimentParts {
    
    /// <summary>
    /// This class is the simplest block possible.
    /// It is used as a default when no custom block is specified.
    /// </summary>
    public class SimpleBlock : Block {
        public SimpleBlock(ExperimentRunner runner,
                           DataTable trialTable, 
                           DataRow dataRow) :
            base(runner, 
                 trialTable, 
                 dataRow) {
        }
        
    }
}