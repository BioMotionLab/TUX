using UnityEngine;

namespace BML_Utilities {
    public class GuiLayoutRect {
        
        readonly float lineHeight;
        Rect           mainRect;
        float          startingY;

        public GuiLayoutRect(float lineHeight) {
            this.lineHeight = lineHeight;
        }

        public Rect NewSetup(Rect position) {
            startingY = mainRect.y;
            mainRect = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);
            return mainRect;
        }
        public Rect NextLine {
            get {
                AddLine();
                return mainRect;
            }
        }

        public float FinalHeight => mainRect.y - startingY;
        public Rect  CurrentLine => mainRect;

        public void AddHeight(float height) {
            mainRect.y += height;
            AddLine();
        }

        void AddLine() {
            mainRect.y += lineHeight;
        }
    }
}