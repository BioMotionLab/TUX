using System.Collections.Generic;
using System.Data;
using BML_Utilities.Extensions;

namespace BML_TUX.Scripts.VariableSystem.VariableValueAddingStrategies {

    /// <inheritdoc />
    /// <summary>
    /// Strategy for adding values to trial table with custom probabilities defined by the variable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomProbabilityAdderStrategy<T> : IndependentVariableValuesAdderStrategy<T> {

        readonly List<T> distribution = new List<T>();

        public override DataTable AddVariableValuesToTable(DataTable table, IndependentVariable<T> independentVariable) {

            table.CreateColumnForVariable(independentVariable);

            if (table.Rows.Count == 0) return AddValuesToEmptyTable(table, independentVariable);

            DataTable newTable = table.Copy();

            CreateDistribution(independentVariable);

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[independentVariable.Name] = distribution.RandomItem();
            }
            
            return newTable;
        }

        void CreateDistribution(IndependentVariable<T> independentVariable) {
            for (int i = 0; i < independentVariable.Probabilities.Count; i++) {
                float prob = independentVariable.Probabilities[i];
                T val = independentVariable.Values[i];
                int number = (int) (prob * 1000);
                for (int j = 0; j < number; j++) {
                    distribution.Add(val);
                }
            }
        }
    }
}