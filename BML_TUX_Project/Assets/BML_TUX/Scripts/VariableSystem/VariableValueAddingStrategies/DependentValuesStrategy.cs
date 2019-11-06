using System;
using System.Data;

namespace BML_TUX.Scripts.VariableSystem.VariableValueAddingStrategies {

    /// <summary>
    /// Defines strategy for adding a dependent variable to the trial table with default values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DependentVariableValuesAdderStrategy<T> {

        public DataTable AddValuesToCopyOf(DataTable table, DependentVariable<T> dependentVariable) {

            AddVariableColumn(dependentVariable, table);

            DataTable newTable = table.Copy();

            if (table.Rows.Count == 0) {
                throw new ArgumentException("Can't add dependent variable values to empty trialTable");
            }

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[dependentVariable.Name] = dependentVariable.DefaultValue;
            }

            return newTable;
        }

        protected void AddVariableColumn(Variable variable, DataTable newTable, int index = -1) {
            DataColumn column = new DataColumn {
                                                   DataType = variable.Type,
                                                   ColumnName = variable.Name,
                                                   ReadOnly = false,
                                                   Unique = false
                                               };
            newTable.Columns.Add(column);
            if (index >= 0) {
                column.SetOrdinal(index);
            }
        }
    }
}