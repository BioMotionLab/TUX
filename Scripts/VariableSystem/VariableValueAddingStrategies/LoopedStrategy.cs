using System.Data;
using BML_Utilities;
using BML_Utilities.Extensions;

namespace bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies {

    public class LoopedAdderStrategy<T> : IndependentVariableValuesAdderStrategy<T> {

        readonly LoopingList<T> loopValues = new LoopingList<T>();
        

        public override DataTable AddVariableValuesToTable(DataTable table, IndependentVariable<T> independentVariable) {

            table.CreateColumnForVariable(independentVariable);
            if (table.Rows.Count == 0) return AddValuesToEmptyTable(table, independentVariable);
            

            loopValues.AddRange(independentVariable.Values);
            int numberOfCopies = CalculateNumberOfCopies(table, independentVariable);

            DataTable newTable = MakeRequiredNumberOfCopiesOf(table, numberOfCopies);

            AddValuesToTable(newTable, independentVariable);

            return newTable;
        }

        void AddValuesToTable(DataTable newTable, IndependentVariable<T> independentVariable) {
            T value = loopValues.FirstElement;
            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[independentVariable.Name] = value;
                value = loopValues.NextElement;
            }
        }

        static int CalculateNumberOfCopies(DataTable table, IndependentVariable<T> independentVariable) {
            int lowestCommonMultiple = LowestCommon.Multiple(table.Rows.Count, independentVariable.Values.Count);
            int numberOfCopies = lowestCommonMultiple / table.Rows.Count;
            return numberOfCopies;
        }


        DataTable MakeRequiredNumberOfCopiesOf(DataTable table, int numberOfTableCopies) {
            DataTable newTable = table.Clone();
            
            for (int i = 0; i < numberOfTableCopies; i++) {
                foreach (DataRow tableRow in table.Rows) {
                    newTable.ImportRow(tableRow);
                }
            }

            return newTable;
        }
    }
}