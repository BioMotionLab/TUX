using System.Data;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.ValueAddingStrategies {

    public abstract class IndependentValuesStrategy<T> {

        public abstract DataTable AddValuesToTable(DataTable table, IndependentVariable<T> independentVariable);

        public DataTable AddValuesToEmptyTable(DataTable table, IndependentVariable<T> castIndependentVariable) {

            DataTable newTable = table.Clone();
            
            //Debug.Log("Adding rows to empty trialTable in variable creation");
            foreach (T value in castIndependentVariable.Values) {
                var newRow = newTable.NewRow();
                newRow[castIndependentVariable.Name] = value;
                newTable.Rows.Add(newRow);
            }

            return newTable;

        }

    }
}