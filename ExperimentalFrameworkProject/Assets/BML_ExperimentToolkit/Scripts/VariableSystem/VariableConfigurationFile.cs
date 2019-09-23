using System;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Variable Configuration")]
    public class VariableConfigurationFile : ScriptableObject {

        
        [Header("Randomization and Repetition settings:")]
        public RandomizationMode RandomizationMode;
        
        [Range(1,50)]
        public int  RepeatTrialsInBlock = 1;
        
        [Range(1,20)]
        public int RepeatAllBlocks = 1;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNamesSettings ColumnNamesSettings;
        public ControlSettings ControlSettings;
        public GuiSettings GuiSettings;
        
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

        public Variables Variables => Factory.Variables;
        
        
        [SerializeField]
        public TrialTableGenerationMode TrialTableGeneration = TrialTableGenerationMode.OnTheFly;
        
        [SerializeField]
        public List<BlockOrderDefinition> BlockOrderConfigurations = new List<BlockOrderDefinition>();


    }
    

}