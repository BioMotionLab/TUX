using System.Collections.Generic;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Settings;
using bmlTUX.Scripts.UI.RuntimeUI;
using bmlTUX.UI.RuntimeUI;
using UnityEngine;
using VariableSystem;

namespace bmlTUX.Scripts.VariableSystem {
    public interface IExperimentDesignFile {
        BlockRandomizationMode GetBlockRandomization { get; }
        ColumnNamesSettings GetColumnNamesSettings { get; }
        List<BlockOrderDefinition>  GetBlockOrderConfigurations { get; }
        int GetExperimentRepetitions { get; }
        TrialRandomizationMode GetTrialRandomization { get; }
        TrialPermutationType GetTrialPermutationType { get;  }
        int GetTrialRepetitions { get; }
        BlockPartialRandomizationSubType GetBlockPartialRandomizationSubType { get;  }
        ControlSettings GetControlSettings { get; }
        TrialTableGenerationMode GetTrialTableGeneration { get; }
        Variables GetVariables { get; }
        GuiSettings GetGuiSettings { get; }
        IVariableFactory GetFactory { get; }
        string GetName { get; }
        bool GetHasBlocks { get; }
        bool GetBlockOrderIsValid { get; }
        int GetBlockNumber { get; }
        ExperimentGui GetGuiPrefab { get;  }
        void GetValidate();
    }
}