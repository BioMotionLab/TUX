using System.Data;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.VariableSystem;

namespace bmlTUX.Scripts.UI.Runtime {
    public class DesignPreviewer {
        public readonly ExperimentDesignFile designFile;
        public int SelectedBlockOrderIndex;

        public readonly ExperimentDesign experimentDesign;
        public DataTable previewTable;
        public int lastDisplayedOrderIndex = -1 ;
        public readonly BlockOrderData blockOrderData;

        public bool SelectedBlockOrderChanged => SelectedBlockOrderIndex != lastDisplayedOrderIndex;

        public DesignPreviewer(ExperimentDesignFile designFile) {
            this.designFile = designFile;
            experimentDesign = ExperimentDesign.CreateFrom(designFile);
            blockOrderData = new BlockOrderData(experimentDesign);
        }

        public bool DesignFileLinked() {
            bool linked = designFile != null;
            return linked;
        }

        public DataTable GetPreview(int blockOrderIndex) {
            
            if (!DesignFileLinked()) return null;
            
            if (SelectedBlockOrderChanged || previewTable == null) {
                previewTable = experimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
                lastDisplayedOrderIndex = SelectedBlockOrderIndex;
            }
            
            return previewTable;
        }

        public void ReRandomizeTable() {
            previewTable = experimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
        }


    }
}