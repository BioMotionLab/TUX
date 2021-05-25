using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.UI.RuntimeUI;
using UnityEngine;

namespace bmlTUX.Scripts.Settings {
    [CreateAssetMenu(menuName = MenuNames.AssetCreationMenu + "New Experiment Settings File")]
    public class ExperimentSettings : ScriptableObject {
        
        public ColumnNamesSettings ColumnNames = default;
        
        [Space]
        
        public ControlSettings ControlSettings = default;
        
        [Space]
        public ExperimentGui GuiPrefab;
        public GuiSettings GuiSettings = default;
        
    }
}