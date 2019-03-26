using System.Data;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.ValueAddingStrategies {

    public static class AddVariableColumnToDataTable
    {

        public static void AddColumnForVariable(this DataTable table, Variable variable, int index = -1) {
            DataColumn column = new DataColumn {
                                                   DataType = variable.Type,
                                                   ColumnName = variable.Name,
                                                   ReadOnly = false,
                                                   Unique = false
                                               };
            table.Columns.Add(column);
            if (index >= 0) {
                column.SetOrdinal(index);
            }
        }
    }
}
