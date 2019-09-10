using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEditor;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {
    public class DesignPreviewWindow : EditorWindow {
        
        public VariableConfigurationFile ConfigurationFile;
        DesignPreviewer                  previewer;

        void OnEnable() {
            previewer = new DesignPreviewer(ConfigurationFile);
        }

        void OnGUI() {
            
            previewer.ShowPreview();
        }

        public static void ShowWindow() {
            DesignPreviewWindow window = (DesignPreviewWindow) GetWindow(typeof(DesignPreviewWindow), false, "Design Previewer");
            window.Show();
        }
    }
}