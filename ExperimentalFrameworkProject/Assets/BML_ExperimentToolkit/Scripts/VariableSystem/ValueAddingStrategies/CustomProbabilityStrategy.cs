using System.Collections.Generic;
using System.Data;
using BML_Utilities;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.ValueAddingStrategies {
    public class CustomProbabilityStrategy<T> : IndependentValuesStrategy<T> {

        public override DataTable AddValuesToTable(DataTable table, IndependentVariable<T> castIndependentVariable) {

            table.AddColumnForVariable(castIndependentVariable);
            if (table.Rows.Count == 0) return AddValuesToEmptyTable(table, castIndependentVariable);

            DataTable newTable = table.Copy();

            List<T> distribution = new List<T>();
            for (int i = 0; i < castIndependentVariable.Probabilities.Count; i++) {
                float prob = castIndependentVariable.Probabilities[i];
                T val = castIndependentVariable.Values[i];
                int number = (int)(prob * 1000);
                for (int j = 0; j < number; j++) {
                    distribution.Add(val);
                }
            }

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[castIndependentVariable.Name] = distribution.RandomItem();
            }
            
            return newTable;
        }
    }
}