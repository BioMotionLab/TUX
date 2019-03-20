using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class ConfigDesignFile : ScriptableObject {

        public bool ShuffleTrialOrder;
        public bool ShuffleDifferentlyForEachBlock;
        public int  RepeatTrialBlock;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNames ColumnNames;

       
    }
}