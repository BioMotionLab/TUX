using BML_ExperimentToolkit.Scripts.Settings;
using UnityEngine;
using MenuNames = BML_Utilities.MenuNames;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {

    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Create new Variable Config Asset")]
    public class VariableConfig : ScriptableObject {

        public bool ShuffleTrialOrder;
        public bool ShuffleDifferentlyForEachBlock;
        public int  RepeatTrialBlock;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNamesSettings ColumnNamesSettings;
        public ControlSettings ControlSettings;
    }
}