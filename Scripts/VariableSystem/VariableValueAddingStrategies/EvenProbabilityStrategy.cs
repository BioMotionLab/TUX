using System.Data;
using bmlTUX.Scripts.Utilities.Extensions;

namespace bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies {

    /// <inheritdoc />
    /// <summary>
    /// Strategy for adding variable values to data table with even probability of each value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EvenProbabilityAdderStrategy<T> : IndependentVariableValuesAdderStrategy<T> {

        public override DataTable AddVariableValuesToTable(DataTable table, IndependentVariable<T> independentVariable) {

            table.CreateColumnForVariable(independentVariable);

            if (table.Rows.Count == 0) return AddValuesToEmptyTable(table, independentVariable);

            DataTable newTable = table.Copy();

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[independentVariable.Name] = independentVariable.Values.RandomItem();
            }

            return newTable;
        }
    }
}