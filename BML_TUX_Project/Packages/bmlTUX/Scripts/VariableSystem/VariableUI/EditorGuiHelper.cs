using System.IO;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public static class EditorGuiHelper {
        public static Texture2D MakeTex(Color col )
        {
            Color[] pix = new Color[1 * 1];
            for(int i = 0; i < pix.Length; ++i ) {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(1, 1 );
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public static GUIStyle MakeBackgroundStyle(Color backgroundColor) {
            GUIStyle variableStyle = new GUIStyle(GUI.skin.box) {
                normal = {
                    background = MakeTex(backgroundColor)
                }
            };
            return variableStyle;
        }
        

        public static Color AlternateColor {
            get {
                Color alternateColor = Color.grey;
                alternateColor.a = .5f;
                return alternateColor;
            }
        }

        public static GUIStyle AlternateColorBox => MakeBackgroundStyle(AlternateColor);
        public static Color IndependentVarColor { get {
            Color alternateColor = Color.cyan;
            alternateColor.a = .05f;
            return alternateColor;
        }  }

        public static Color DependentVarColor { get {
            Color alternateColor = Color.magenta;
            alternateColor.a = .05f;
            return alternateColor;
        }  }
        public static Color ParticipantVarColor { get {
            Color alternateColor = Color.yellow;
            alternateColor.a = .05f;
            return alternateColor;
        } }

        public static Color RedColor { get {
            Color alternateColor = Color.red;
            alternateColor.a = .1f;
            return alternateColor;
        } }


        public static string GetUniqueName(string fullPath) {
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while(File.Exists(newFullPath)) {
                count++;
                string tempFileName = $"{fileNameOnly}({count})";
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return newFullPath;
        }
        
    }
    
    
}