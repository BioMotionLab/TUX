using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.ValueAddingStrategies {
    public class BalancedStrategy<T> : IndependentValuesStrategy<T> {


        public override DataTable AddValuesToTable(DataTable table, IndependentVariable<T> castIndependentVariable) {

            table.AddColumnForVariable(castIndependentVariable);

            if (table.Rows.Count == 0) return AddValuesToEmptyTable(table, castIndependentVariable);

            DataTable newTable = table.Clone();


            //Debug.Log("Adding rows to NON empty trialTable in variable creation");
            foreach (DataRow tableRow in table.Rows) {
                foreach (T value in castIndependentVariable.Values) {
                    newTable.ImportRow(tableRow);
                    var newRow = newTable.Rows[newTable.Rows.Count - 1];
                    newRow[castIndependentVariable.Name] = value;
                }
            }


            return newTable;
        }

    }
}