using System;
using System.Data;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableValueAddingStrategies {

    /// <summary>
    /// Defines strategy for adding a dependent variable to the trial table with default values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParticipantVariableValuesAdderStrategy<T> {

        public DataTable AddValuesToCopyOf(DataTable table, ParticipantVariable<T> participantVariable) {

            AddVariableColumn(participantVariable, table);

            DataTable newTable = table.Copy();

            if (table.Rows.Count == 0) {
                throw new ArgumentException("Can't add participant variable values to empty trialTable");
            }

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[participantVariable.Name] = participantVariable.Value;
            }

            return newTable;
        }


        //TODO should probably be extension method
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