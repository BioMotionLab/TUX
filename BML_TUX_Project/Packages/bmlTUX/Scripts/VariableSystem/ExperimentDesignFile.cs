using System.Collections.Generic;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Settings;
using bmlTUX.Scripts.Utilities;
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

        bool valid = true;
        
        void OnValidate() {
            Validate();
        }

        public void Validate() {
            bool wasValid = valid;
            valid = true;
            if (ColumnNamesSettings == null) MissingReference(nameof(ColumnNamesSettings));
            if (ControlSettings == null) MissingReference(nameof(ControlSettings));
            if (FileLocationSettings == null) MissingReference(nameof(FileLocationSettings));
            if (GuiSettings == null) MissingReference(nameof(GuiSettings));
            if (!wasValid && valid) Debug.Log(TuxLog.Good($"{nameof(ExperimentDesignFile)} Fixed."), this);
        }

        void MissingReference(string missingReference) {
            TuxLog.LogError($"{nameof(ExperimentDesignFile)} does not have {missingReference} defined. " + 
                         $"Please drag {missingReference} into the proper place in the design file", this);
            valid = false;
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