using System.Collections.Generic;
using bmlTUX.Scripts.UI.RuntimeUI.RunnerWindowUI;

namespace bmlTUX.Scripts.UI.RuntimeUI.UIUtilities {
    public class BlockOrderValidationResult : InputValidator {
        
        public BlockOrderValidationResult(BlockOrderData blockOrderData, BlockOrderPanel blockOrderPanel) {
            Valid = true;
            if (!blockOrderData.SelectionRequired) return;
            Errors = new List<string>();
            if (blockOrderPanel.BlockChosen) return;
            Errors.Add($"Need to select block order value");
            Valid = false;
        }

        public List<string> Errors { get; }
        public bool         Valid  { get; }
    }
}