using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace bmlTUX.Extras.Instructions {
    public class InstructionsDisplay : MonoBehaviour {

        public Text       TextBox;
        public GameObject DisplayGameObject;

        void Start() {
            HideInstructions();
        }

        [PublicAPI]
        public void ShowInstructions(string instructions) {
            TextBox.text = instructions;
            DisplayGameObject.SetActive(true);
        }

        [PublicAPI]
        public void ShowInstructionsForDuration(string instructions, float duration) {
            ShowInstructions(instructions);
            StartCoroutine(HideAfterSeconds(duration));

        }

        [PublicAPI]
        public void HideInstructions() {
            TextBox.text = string.Empty;
            DisplayGameObject.SetActive(false);
        }
        
        IEnumerator HideAfterSeconds(float duration) {
            yield return new WaitForSeconds(duration);
            HideInstructions();
        }
        
    }
    
}