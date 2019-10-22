using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine.Serialization;

namespace BML_TUX.Scripts.UI.Editor {
    public class DesignPreviewWindow : EditorWindow {
        
        [FormerlySerializedAs("ConfigurationFile")]
        public ExperimentDesignFile DesignFile;
        DesignPreviewer                  previewer;
        
        void OnGUI() {
            if (previewer != null) {
                previewer.ShowPreview();
            }
            else {
                EditorGUILayout.HelpBox("Nothing to preview. Click on the preview button of a design file", MessageType.Error);
            }

        }

        public static void ShowWindow(ExperimentDesignFile configFile) {
            
            DesignPreviewWindow window = (DesignPreviewWindow) GetWindow(typeof(DesignPreviewWindow), false, "Design Previewer");
            window.previewer = new DesignPreviewer(configFile);
            window.Show();
        }
        
    }
}