using UnityEngine;

namespace BML_Utilities {
    public class GuiLayoutRect {
        
        readonly float lineHeight;
        public float LineHeight => lineHeight;
        Rect           mainRect;
        readonly float startingY;

        public GuiLayoutRect(float lineHeight, Rect position) {
            this.lineHeight = lineHeight;
            startingY = position.y;
            mainRect = new Rect(position.x, position.y, position.width, lineHeight);
        }

        public Rect NextLine {
            get {
                AddLine();
                return mainRect;
            }
        }

        public Rect NextLines(int numberOfLines) {
            Rect longRect = new Rect(mainRect.x, mainRect.y + lineHeight, mainRect.width, lineHeight*numberOfLines);
            for (int i = 0; i < numberOfLines; i++) {
                AddLine();
            }
            return longRect;
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