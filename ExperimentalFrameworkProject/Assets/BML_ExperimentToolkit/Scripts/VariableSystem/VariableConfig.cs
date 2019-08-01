using System;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Settings;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {

    [CreateAssetMenu]
    public class VariableConfig : ScriptableObject {

        public bool ShuffleTrialOrder = false;
        public bool ShuffleDifferentlyForEachBlock = false;
        
        [Range(1,100)]
        public int  RepeatTrialsInBlock = 1;
        
        [Range(1,100)]
        public int RepeatAllBlocks = 1;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNamesSettings ColumnNamesSettings;
        public ControlSettings ControlSettings;
        
        [Header("Manual Block Order Config:")]
        [SerializeField]
        public List<OrderConfig> OrderConfigs = new List<OrderConfig>();

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