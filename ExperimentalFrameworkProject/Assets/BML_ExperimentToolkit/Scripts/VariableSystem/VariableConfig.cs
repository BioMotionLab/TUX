using System;
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

        public void Validate() {
            
            if (ColumnNamesSettings == null) {
                throw new NullReferenceException(
                                                 "Configuration file does not have column name settings defined. " + 
                                                 "Please drag column name settings into the proper place in the config file");
            }

            if (ControlSettings == null) {
                throw new NullReferenceException(
                                                 "Configuration file does not have Control Settings defined. " +
                                                 "Please drag control settings into the proper place in the config file");
            }
        }
    }
}