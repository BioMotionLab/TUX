using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace bmlTUX.Scripts.UI.RuntimeUI.RunnerWindowUI {
    public class BlockOrderPanel : MonoBehaviour
    {
        const string PleaseSelectText = "Choose...";
        
        [SerializeField]
        TMP_Dropdown BlockOrderSelector = default;
        
        [SerializeField]
        TextMeshProUGUI BlockOrderTitle = default;
        
        public int SelectedBlockOrder {
            get => BlockOrderSelector.value;
            set => BlockOrderSelector.value = value;
        }

        string SelectedText => BlockOrderSelector.options[BlockOrderSelector.value].text;

        public bool BlockChosen => SelectedText != PleaseSelectText;

        void Clear() {
            BlockOrderSelector.options.Clear();
        }

        void AddOptions(List<string> blockPermutations) {
            
            foreach (string order in blockPermutations) {
                BlockOrderSelector.options.Add(new TMP_Dropdown.OptionData() {text = order});
            }
        }

        public void SetTitle(string blockOrderText) {
            BlockOrderTitle.text = blockOrderText;
        }
        
        
        public void GetBlockOrderFromPopup(List<string> experimentDesignBlockPermutationsStrings) {
            List<string> blockPermutations = experimentDesignBlockPermutationsStrings;
            Clear();
            blockPermutations.Insert(0, PleaseSelectText);
            AddOptions(blockPermutations);
        }
        
        public void Activate() {
            gameObject.SetActive(true);
        }

        public void Deactivate() {
            gameObject.SetActive(false);
        }

    }
}
