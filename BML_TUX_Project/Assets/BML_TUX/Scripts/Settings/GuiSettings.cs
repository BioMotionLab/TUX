using BML_TUX.Scripts.UI.Runtime;
using UnityEngine;

namespace BML_TUX.Scripts.Settings {
    [CreateAssetMenu(menuName = MenuNames.BmlSettingsMenu + "GUI Settings")]
    public class GuiSettings : ScriptableObject {
        
        public ExperimentGui GuiPrefab;
    }
}
