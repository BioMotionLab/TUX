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
        
        
        void OnValidate() {
            Debug.LogWarning($"{name} is obsolete. Please Update. (You can click on this warning to select it)", this);
        }
        
        


    }
    

}