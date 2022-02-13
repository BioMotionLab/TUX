using bmlTUX.Scripts.UI.RuntimeUI.UIUtilities;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace bmlTUX.UI.EditorUI {
    public class DesignPreviewEditorWindow : EditorWindow {
        
        [FormerlySerializedAs("ConfigurationFile")]
        public IExperimentDesignFile DesignFile;
        DesignPreviewer                  previewer;
        Vector2 scrollPos;

        void OnGUI() {
            if (previewer != null) {
                DesignPreviewEditorDisplay previewDisplay = new DesignPreviewEditorDisplay(previewer);
                
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, 
                                                            false, false, 
                                                            GUILayout.ExpandHeight(true));
                
                previewDisplay.ShowEditorPreview();
                
                EditorGUILayout.EndScrollView();
            }
            else {
                EditorGUILayout.HelpBox("Nothing to preview. Make sure a design file is selected and it has no errors.", MessageType.Error);
            }

        }

        public static void ShowWindow(IExperimentDesignFile configFile) {
            
            DesignPreviewEditorWindow editorWindow = (DesignPreviewEditorWindow) GetWindow(typeof(DesignPreviewEditorWindow), false, "Design Previewer");
            editorWindow.previewer = new DesignPreviewer(configFile);
            editorWindow.Show();
        }
        
    }
}