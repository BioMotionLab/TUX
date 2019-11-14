using System;
using System.Collections.Generic;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Settings;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {
    [CreateAssetMenu(menuName = MenuNames.AssetCreationMenu + "Experiment Design File")]
    public class ExperimentDesignFile : ScriptableObject {

       
        [SerializeField]
        [Min(1)]
        public int TrialRepetitions = 1;
        
        [Min(1)]
        public int ExperimentRepetitions = 1;
        
        public TrialRandomizationMode TrialRandomization = TrialRandomizationMode.InOrder;
        public TrialPermutationType TrialPermutationType = TrialPermutationType.DifferentPermutations;

        public BlockRandomizationMode BlockRandomization = BlockRandomizationMode.InOrder;
        public BlockPartialRandomizationSubType BlockPartialRandomizationSubType = BlockPartialRandomizationSubType.DifferentPermutations;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        [SerializeField]
        public ColumnNamesSettings ColumnNamesSettings;
        [SerializeField]
        public ControlSettings ControlSettings;
        [SerializeField]
        public GuiSettings GuiSettings;
        [SerializeField]
        public FileLocationSettings FileLocationSettings;
        
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