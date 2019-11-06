using System.Data;

namespace BML_TUX.Scripts.VariableSystem.VariableValueAddingStrategies {


    /// <summary>
    /// A strategy for adding balanced variables to a table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BalancedAdderStrategy<T> : IndependentVariableValuesAdderStrategy<T> {

        public override DataTable AddVariableValuesToTable(DataTable table, IndependentVariable<T> independentVariable) {

            table.CreateColumnForVariable(independentVariable);

            if (table.Rows.Count == 0) return AddValuesToEmptyTable(table, independentVariable);

            DataTable newTable = table.Clone();
            
            foreach (DataRow tableRow in table.Rows) {
                foreach (T value in independentVariable.Values) {
                    newTable.ImportRow(tableRow);
                    DataRow newRow = newTable.Rows[newTable.Rows.Count - 1];
                    newRow[independentVariable.Name] = value;
                }
            }

            return newTable;
        }
    }
}