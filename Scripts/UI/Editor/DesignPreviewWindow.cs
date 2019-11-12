using bmlTUX.Scripts.UI.Runtime;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine.Serialization;

namespace BML_TUX.Scripts.UI.Editor {
    public class DesignPreviewWindow : EditorWindow {
        
        [FormerlySerializedAs("ConfigurationFile")]
        public ExperimentDesignFile DesignFile;
        DesignPreviewer                  previewer;
        
        void OnGUI() {
            if (previewer != null) {
                previewer.ShowEditorPreview();
            }
            else {
                EditorGUILayout.HelpBox("Nothing to preview. Make sure a design file is selected and it has no errors.", MessageType.Error);
            }

        }

        public static void ShowWindow(ExperimentDesignFile configFile) {
            
            DesignPreviewWindow window = (DesignPreviewWindow) GetWindow(typeof(DesignPreviewWindow), false, "Design Previewer");
            window.previewer = new DesignPreviewer(configFile);
            window.Show();
        }
        
    }
}