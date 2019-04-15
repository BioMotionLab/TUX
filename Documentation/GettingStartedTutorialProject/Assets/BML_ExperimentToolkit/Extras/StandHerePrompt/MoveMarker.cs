using UnityEngine;

namespace BML_ExperimentToolkit.Extras.StandHerePrompt {
    public abstract class MoveMarker : MonoBehaviour {


        public StandHerePromptSettings PromptSettings;
        public MeshRenderer      Renderer;
        public Transform         MainText;
        public Transform         Hmd;

        public abstract void RotateTextToBeReadable();

        void Update() {
            RotateTextToBeReadable();
        }

        public void SetMaterial(Material material) {
            Renderer.material = material;
        }
    }
}