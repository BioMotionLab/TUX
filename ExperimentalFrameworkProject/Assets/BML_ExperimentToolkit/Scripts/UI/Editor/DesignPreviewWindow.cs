using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEditor;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {
    public class DesignPreviewWindow : EditorWindow {
        
        public VariableConfigurationFile ConfigurationFile;
        DesignPreviewer                  previewer;
        
        void OnGUI() {
            previewer.ShowPreview();
        }

        public static void ShowWindow(VariableConfigurationFile configFile) {
            
            DesignPreviewWindow window = (DesignPreviewWindow) GetWindow(typeof(DesignPreviewWindow), false, "Design Previewer");
            window.previewer = new DesignPreviewer(configFile);
            window.Show();
        }
        
    }
}