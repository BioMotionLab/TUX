using System;
using System.Data;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts {
    
    public class SimpleBlock : Block {
        public SimpleBlock(DataRow row, DataTable trialTable, string identity, Type CustomTrialType) :
            base(row, trialTable, identity, CustomTrialType) {
        }
    }
}