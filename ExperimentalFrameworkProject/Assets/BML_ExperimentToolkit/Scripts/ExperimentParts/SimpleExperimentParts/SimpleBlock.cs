﻿using System;
using System.Data;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts {
    
    /// <summary>
    /// This class is the simplest block possible.
    /// It is used as a default when no custom block is specified.
    /// </summary>
    public class SimpleBlock : Block {
        public SimpleBlock(ExperimentRunner runner,
                           DataTable trialTable, 
                           string identity, 
                           Type customTrialType) :
            base(runner, 
                 trialTable, 
                 identity, 
                 customTrialType) {
        }
        
    }
}