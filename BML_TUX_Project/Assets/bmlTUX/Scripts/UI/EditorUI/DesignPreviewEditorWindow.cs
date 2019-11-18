using bmlTUX.Scripts.UI.RuntimeUI.UIUtilities;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine.Serialization;

namespace bmlTUX.Scripts.UI.EditorUI {
    public class DesignPreviewEditorWindow : EditorWindow {
        
        [FormerlySerializedAs("ConfigurationFile")]
        public ExperimentDesignFile DesignFile;
        DesignPreviewer                  previewer;
        
        void OnGUI() {
            if (previewer != null) {
                DesignPreviewEditorDisplay previewDisplay = new DesignPreviewEditorDisplay(previewer);
                previewDisplay.ShowEditorPreview();
            }
            else {
                EditorGUILayout.HelpBox("Nothing to preview. Make sure a design file is selected and it has no errors.", MessageType.Error);
            }

        }

        public static void ShowWindow(ExperimentDesignFile configFile) {
            
            DesignPreviewEditorWindow editorWindow = (DesignPreviewEditorWindow) GetWindow(typeof(DesignPreviewEditorWindow), false, "Design Previewer");
            editorWindow.previewer = new DesignPreviewer(configFile);
            editorWindow.Show();
        }
        
    }
}