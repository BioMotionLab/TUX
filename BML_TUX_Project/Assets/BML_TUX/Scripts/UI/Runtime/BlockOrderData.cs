using BML_TUX.Scripts.ExperimentParts;

namespace BML_TUX.Scripts.UI.Runtime {
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