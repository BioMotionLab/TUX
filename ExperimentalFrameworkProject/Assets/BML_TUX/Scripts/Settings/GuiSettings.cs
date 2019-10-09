using BML_ExperimentToolkit.Scripts.UI.Runtime;
using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.Settings {
    [CreateAssetMenu(menuName = MenuNames.BmlSettingsMenu + "GUI Settings")]
    public class GuiSettings : ScriptableObject {
        
        public ExperimentGui GuiPrefab;
    }
}
