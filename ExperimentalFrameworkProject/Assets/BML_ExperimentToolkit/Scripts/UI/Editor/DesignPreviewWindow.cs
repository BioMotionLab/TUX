using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {
    public class DesignPreviewWindow : EditorWindow {

        [SerializeField]
        int OrderIndex = default;
        
        public VariableConfigurationFile ConfigurationFile;
        DesignPreviewer                  previewer;

        void OnGUI() {

            if (previewer == null) {
                previewer = new DesignPreviewer(ConfigurationFile);
            }

            previewer.ShowPreview();

        }

        public static void ShowWindow(VariableConfigurationFile target) {
            DesignPreviewWindow window = (DesignPreviewWindow) GetWindow(typeof(DesignPreviewWindow), false, "Design Previewer");
            window.ConfigurationFile = target;
            window.Show();
            
        }
        
    }
}