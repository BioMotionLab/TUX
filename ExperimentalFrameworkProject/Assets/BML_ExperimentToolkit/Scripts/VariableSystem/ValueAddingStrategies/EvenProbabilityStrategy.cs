using System.Data;
using BML_Utilities;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.ValueAddingStrategies {
    public class EvenProbabilityStrategy<T> : IndependentValuesStrategy<T> {
        public override DataTable AddValuesToTable(DataTable table, IndependentVariable<T> castIndependentVariable) {

            table.AddColumnForVariable(castIndependentVariable);

            if (table.Rows.Count == 0) return AddValuesToEmptyTable(table, castIndependentVariable);

            DataTable newTable = table.Copy();

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[castIndependentVariable.Name] = castIndependentVariable.Values.RandomItem();
            }


            return newTable;
        }
    }
}