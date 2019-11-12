using bmlTUX.Scripts.UI.Runtime;
using UnityEngine;

namespace bmlTUX.Scripts.Settings {
    [CreateAssetMenu(menuName = MenuNames.BmlSettingsMenu + "GUI Settings")]
    public class GuiSettings : ScriptableObject {
        
        public ExperimentGui GuiPrefab;
    }
}
