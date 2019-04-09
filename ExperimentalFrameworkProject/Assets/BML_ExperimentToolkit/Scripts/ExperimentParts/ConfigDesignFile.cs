using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEngine;
using MenuNames = BML_Utilities.MenuNames;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Variable Config")]
    public class ConfigDesignFile : ScriptableObject {

        public bool ShuffleTrialOrder;
        public bool ShuffleDifferentlyForEachBlock;
        public int  RepeatTrialBlock;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNames ColumnNames;

       
    }
}