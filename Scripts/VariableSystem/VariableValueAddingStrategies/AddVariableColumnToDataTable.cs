using System;
using System.Data;
using bmlTUX.Scripts.Utilities;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies {

    public static class CreateColumnForVariableInDataTableExtension
    {

        /// <summary>
        /// Creates a column in the datatable based on the variable's name and type.
        /// You can manually specify the position of the column.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="variable"></param>
        /// <param name="index">The optional index to place the column</param>
        public static void CreateColumnForVariable(this DataTable table, Variable variable, int index = -1) {
            DataColumn column = new DataColumn {
                                                   DataType = variable.Type,
                                                   ColumnName = variable.Name,
                                                   ReadOnly = false,
                                                   Unique = false
                                               };
            if (column.ColumnName == UnnamedColumn.Name) {
                Debug.LogError($"{TuxLog.Prefix} There remains an unnamed variable. Please fix.");
                throw new InvalidOperationException("Can't add unnamed variable to table");
            }
            table.Columns.Add(column);
            if (index >= 0) {
                column.SetOrdinal(index);
            }
        }
    }
}
