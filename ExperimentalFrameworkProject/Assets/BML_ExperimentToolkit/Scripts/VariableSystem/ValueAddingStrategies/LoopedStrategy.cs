using System.Data;
using BML_Utilities;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.ValueAddingStrategies {

    public class LoopedStrategy<T> : IndependentValuesStrategy<T> {

        public override DataTable AddValuesToTable(DataTable table, IndependentVariable<T> castIndependentVariable) {

            table.AddColumnForVariable(castIndependentVariable);
            if (table.Rows.Count == 0) return AddValuesToEmptyTable(table, castIndependentVariable);
            DataTable newTable = table.Clone();


            LoopingList<T> loopValues = new LoopingList<T>();
            loopValues.AddRange(castIndependentVariable.Values);
            int lowestCommonMultiple =
                LowestCommonFunctions.LowestCommonMultiple(table.Rows.Count, castIndependentVariable.Values.Count);

            //Make the required number of copies of the trialTable.
            int numberOfTableCopies = lowestCommonMultiple / table.Rows.Count;
            for (int i = 0; i < numberOfTableCopies; i++) {
                foreach (DataRow tableRow in table.Rows) {
                    newTable.ImportRow(tableRow);
                }
            }

            T value = loopValues.FirstElement;
            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[castIndependentVariable.Name] = value;
                value = loopValues.NextElement;
            }
            
            return newTable;
        }

    }
}