using System.Data;

namespace bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies {

    /// <summary>
    /// Definition for strategies for adding variables to tables to create trial structure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IndependentVariableValuesAdderStrategy<T> {

        /// <summary>
        /// Adds a variable's values to a data table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="independentVariable"></param>
        /// <returns></returns>
        public abstract DataTable AddVariableValuesToTable(DataTable table, IndependentVariable<T> independentVariable);

        /// <summary>
        /// Adds values of this variable to an empty table. Just one trial per value.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="independentVariable"></param>
        /// <returns></returns>
        public DataTable AddValuesToEmptyTable(DataTable table, IndependentVariable<T> independentVariable) {

            DataTable newTable = table.Clone();
            
            //Debug.Log("Adding rows to empty trialTable in variable creation");
            foreach (T value in independentVariable.Values) {
                DataRow newRow = newTable.NewRow();
                newRow[independentVariable.Name] = value;
                newTable.Rows.Add(newRow);
            }

            return newTable;

        }

    }
}