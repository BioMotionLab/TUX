using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class ConfigDesignFile : ScriptableObject {
        public bool ShuffleTrialOrder;
        public int  NumberOfTimesToRepeatTrials = 1;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNames ColumnNames;

        public void PrintTrials() {
            throw new System.NotImplementedException();
        }
    }
}