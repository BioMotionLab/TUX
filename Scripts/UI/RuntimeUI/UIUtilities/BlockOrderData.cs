namespace bmlTUX.Scripts.UI.RuntimeUI.UIUtilities {
    public class BlockOrderData {
        
        
        public readonly bool   SelectionRequired      = false;
        public readonly int    DefaultBlockOrderIndex = 0;
        public readonly string BlockOrderText;
        
        public BlockOrderData(ExperimentDesign experimentDesign) {
            
            if (experimentDesign.RandomizedBlocks) {
                SelectionRequired = false;
            }
            else if (experimentDesign.NumberOfBlocks > 1) {
                BlockOrderText = "Select a Block order";
                SelectionRequired = true;
            }
            else if (experimentDesign.NumberOfBlocks == 1) {
                BlockOrderText = "Only one Block value";
            }
            else {
                BlockOrderText = "No Block variables";
            }
        }

    }
}