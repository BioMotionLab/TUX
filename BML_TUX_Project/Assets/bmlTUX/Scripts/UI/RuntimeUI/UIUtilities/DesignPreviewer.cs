using System.Data;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.VariableSystem;

namespace bmlTUX.Scripts.UI.RuntimeUI.UIUtilities {
    public class DesignPreviewer {
        public readonly ExperimentDesignFile DesignFile;
        public int SelectedBlockOrderIndex;

        public readonly ExperimentDesign ExperimentDesign;
        public DataTable PreviewTable;
        public int LastDisplayedOrderIndex = -1 ;
        public readonly BlockOrderData BlockOrderData;

        public bool SelectedBlockOrderChanged => SelectedBlockOrderIndex != LastDisplayedOrderIndex;

        public DesignPreviewer(ExperimentDesignFile designFile) {
            DesignFile = designFile;
            ExperimentDesign = ExperimentDesign.CreateFrom(designFile);
            BlockOrderData = new BlockOrderData(ExperimentDesign);
        }

        public bool DesignFileLinked() {
            bool linked = DesignFile != null;
            return linked;
        }

        public DataTable GetPreview(int blockOrderIndex) {
            
            if (!DesignFileLinked()) return null;
            
            if (SelectedBlockOrderChanged || PreviewTable == null) {
                PreviewTable = ExperimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
                LastDisplayedOrderIndex = SelectedBlockOrderIndex;
            }
            
            return PreviewTable;
        }

        public void ReRandomizeTable() {
            PreviewTable = ExperimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
        }


    }
}