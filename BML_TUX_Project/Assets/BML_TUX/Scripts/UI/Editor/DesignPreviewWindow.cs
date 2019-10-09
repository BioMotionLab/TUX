using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine.Serialization;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {
    public class DesignPreviewWindow : EditorWindow {
        
        [FormerlySerializedAs("ConfigurationFile")]
        public ExperimentDesignFile DesignFile;
        DesignPreviewer                  previewer;
        
        void OnGUI() {
            previewer.ShowPreview();
        }

        public static void ShowWindow(ExperimentDesignFile configFile) {
            
            DesignPreviewWindow window = (DesignPreviewWindow) GetWindow(typeof(DesignPreviewWindow), false, "Design Previewer");
            window.previewer = new DesignPreviewer(configFile);
            window.Show();
        }
        
    }
}