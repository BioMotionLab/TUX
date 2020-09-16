using System.Collections.Generic;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Settings;
using bmlTUX.Scripts.Utilities;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {
    [CreateAssetMenu(menuName = MenuNames.AssetCreationMenu + "Experiment Design File")]
    public class ExperimentDesignFile2 : ScriptableObject, IExperimentDesignFile {
        
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
        public VariableFactory2 Factory = new VariableFactory2();

        [SerializeField]
        public ColumnNamesSettings ColumnNamesSettings;
        [SerializeField]
        public ControlSettings ControlSettings;
        [SerializeField]
        public GuiSettings GuiSettings;
        [SerializeField]
        public FileLocationSettings FileLocationSettings;

        bool valid = true;

        [SerializeField] [HideInInspector] public bool ShowAdvancedEditor = false;
        [SerializeField][HideInInspector] public bool BlockOrderIsValid;

        [SerializeField]
        public TrialTableGenerationMode TrialTableGeneration = TrialTableGenerationMode.OnTheFly;
        
        [SerializeField]
        public List<BlockOrderDefinition> BlockOrderConfigurations = new List<BlockOrderDefinition>();

        public Variables Variables => Factory.Variables;

        void OnValidate() {
            Validate();
            CheckBlockOrderValidity();
        }

        public void Validate() {
            bool wasValid = valid;
            valid = true;
            if (ColumnNamesSettings == null) MissingReference(nameof(ColumnNamesSettings));
            if (ControlSettings == null) MissingReference(nameof(ControlSettings));
            if (FileLocationSettings == null) MissingReference(nameof(FileLocationSettings));
            if (GuiSettings == null) MissingReference(nameof(GuiSettings));
            if (!wasValid && valid) Debug.Log(TuxLog.Good($"{nameof(ExperimentDesignFile2)} Fixed."), this);
        }

        void MissingReference(string missingReference) {
            TuxLog.LogError($"{nameof(ExperimentDesignFile2)} does not have {missingReference} defined. " + 
                         $"Please drag {missingReference} into the proper place in the design file", this);
            valid = false;
        }

        public bool HasBlocks => BlockNumber > 0;

        public int BlockNumber {
            get {
                List<IndependentVariable> blockIVs = Variables.BlockVariables;
                return blockIVs.Count;
            }
        }

       

        public void CheckBlockOrderValidity() {
            if (BlockOrderConfigurations.Count == 0) return;
            bool AllValid = true;
            foreach (BlockOrderDefinition blockOrderDefinition in BlockOrderConfigurations) {
                if (blockOrderDefinition == null) continue;
                if (blockOrderDefinition == null || !blockOrderDefinition.Initialized) continue;
                if (!blockOrderDefinition.IsValid) {
                    AllValid = false;
                }
            }

            BlockOrderIsValid = AllValid;
        }

        public BlockRandomizationMode GetBlockRandomization => BlockRandomization;
        public ColumnNamesSettings GetColumnNamesSettings => ColumnNamesSettings;
        public List<BlockOrderDefinition>  GetBlockOrderConfigurations => BlockOrderConfigurations;
        public int GetExperimentRepetitions => ExperimentRepetitions;
        public TrialRandomizationMode GetTrialRandomization => TrialRandomization;
        public TrialPermutationType GetTrialPermutationType => TrialPermutationType;
        public int GetTrialRepetitions => TrialRepetitions;
        public BlockPartialRandomizationSubType GetBlockPartialRandomizationSubType => BlockPartialRandomizationSubType;
        public ControlSettings GetControlSettings => ControlSettings;
        public TrialTableGenerationMode GetTrialTableGeneration => TrialTableGeneration;
        public Variables GetVariables => Variables;
        public GuiSettings GetGuiSettings => GuiSettings;
        public IVariableFactory GetFactory => Factory;
        public FileLocationSettings GetFileLocationSettings => FileLocationSettings;
        public string GetName => name;
        public bool GetHasBlocks => HasBlocks;
        public bool GetBlockOrderIsValid => BlockOrderIsValid;

        public void GetValidate() {
            Validate();
        }

        public int GetBlockNumber => BlockNumber;
    }
    

}