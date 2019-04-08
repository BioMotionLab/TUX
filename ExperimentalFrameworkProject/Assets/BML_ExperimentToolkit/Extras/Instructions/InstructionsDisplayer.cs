using UnityEngine;
using UnityEngine.UI;

namespace BML_ExperimentToolkit.Extras.Instructions {
    public class InstructionsDisplayer : MonoBehaviour {

        public Text       TextBox;
        public GameObject Container;

        void Start() {
            HideInstructions();
        }

        public void ShowInstructions(string instructions) {
            TextBox.text = instructions;
            Container.SetActive(true);
        }

        public void HideInstructions() {
            TextBox.text = string.Empty;
            Container.SetActive(false);
        }
    }
}