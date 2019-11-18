using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace bmlTUX.Scripts.UI.RuntimeUI.SessionSetupWindowUI {
    public class ErrorDisplayPanel : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI ErrorText = default;

        public void Display(string errorString) {
            ErrorText.text = errorString;
            gameObject.SetActive(true);
        }
    
        [PublicAPI]
        public void HideErrorPanel() {
            gameObject.SetActive(false);
        }
    }
}
