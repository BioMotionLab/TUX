using System;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Settings;
using UnityEngine;
using UnityEngine.Serialization;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "New Experiment Design File")]
    public class ExperimentDesignFile : ScriptableObject {

        [Header("Randomization and Repetition settings:")]
       
        
        [FormerlySerializedAs("RepeatTrials")]
        [FormerlySerializedAs("RepeatTrialsInBlock")]
        [Range(1,1000)]
        public int  TrialRepetitions = 1;
        
        [FormerlySerializedAs("RepeatExperiment")]
        [FormerlySerializedAs("RepeatAllBlocks")]
        [Range(1,100)]
        public int ExperimentRepetitions = 1;
        
        
        [FormerlySerializedAs("TrialRandomizationMode")]
        public TrialRandomizationMode TrialRandomization = TrialRandomizationMode.InOrder;
        [FormerlySerializedAs("TrialPermutationType")]
        public TrialPermutationType TrialPermutationType = TrialPermutationType.DifferentPermutations;

        [FormerlySerializedAs("BlockRandomizationMode")]
        public BlockRandomizationMode BlockRandomization = BlockRandomizationMode.InOrder;
        public BlockPartialRandomizationSubType BlockPartialRandomizationSubType = BlockPartialRandomizationSubType.DifferentPermutations;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNamesSettings ColumnNamesSettings;
        public ControlSettings ControlSettings;
        public GuiSettings GuiSettings;
        
        public void Validate() {
            
            if (ColumnNamesSettings == null) {
                throw new NullReferenceException(
                                                 "Design file does not have column name settings defined. " + 
                                                 "Please drag column name settings into the proper place in the design file");
            }

            if (ControlSettings == null) {
                throw new NullReferenceException(
                                                 "Design file does not have Control Settings defined. " +
                                                 "Please drag control settings into the proper place in the design file");
            }
        }

        public Variables Variables => Factory.Variables;
        
        public bool HasBlocks {
            get {
                List<IndependentVariable> blockIVs = Variables.BlockVariables;
                return blockIVs.Count > 0;
            }
        }

        [SerializeField]
        public TrialTableGenerationMode TrialTableGeneration = TrialTableGenerationMode.OnTheFly;
        
        [SerializeField]
        public List<BlockOrderDefinition> BlockOrderConfigurations = new List<BlockOrderDefinition>();


    }
    

}