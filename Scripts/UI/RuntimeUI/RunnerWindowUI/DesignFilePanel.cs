using TMPro;
using UnityEngine;

namespace bmlTUX.UI.RuntimeUI {
    public class DesignFilePanel : MonoBehaviour
    {
        [SerializeField]
        public TMP_InputField DesignFilePathInput = default;

        public string DesignFilePath => DesignFilePathInput.text;

        public void Show() {
            gameObject.SetActive(true);
        }
    
    
    
    }
}
