using System.Data;
using bmlTUX.Extensions;
using bmlTUX.Scripts.Settings;
using bmlTUX.Scripts.VariableSystem;

namespace bmlTUX {
    public class BaseTrialTable {

        DataTable baseTrialTable;

        readonly Variables           variables;
        readonly ColumnNamesSettings columnNames;

        public BaseTrialTable(BaseBlockTable baseBlockTable,
                              IExperimentDesignFile iExperimentDesignFile) {

            variables = iExperimentDesignFile.GetVariables;
            columnNames = iExperimentDesignFile.GetColumnNamesSettings;
            
            baseTrialTable = AddVariablesToTable();
            
            AddMetaColumns(baseBlockTable);
        }

        public DataRowCollection Rows => baseTrialTable.Rows;

        void AddMetaColumns(BaseBlockTable baseBlockTable) {
            AddBlockColumnsFrom(baseBlockTable);
            AddTotalTrialIndexColumn();
            AddTrialIndexColumn();
            AddBlockNumberColumn();
            AddSuccessColumn();
            AddAttemptsColumn();
            AddSkippedColumn();
            AddTrialTimeColumn();
        }

        public static implicit operator DataTable(BaseTrialTable table) {
            return table.baseTrialTable;
        }

        DataTable AddVariablesToTable() {
            DataTable table = new DataTable();

            //Order matters.
            foreach (IndependentVariable independentVariable in variables.IndependentVariables.Looped) {
                table = independentVariable.AddValuesTo(table);
            }

            foreach (IndependentVariable independentVariable in variables.IndependentVariables.Balanced) {
                table = independentVariable.AddValuesTo(table);
            }

            foreach (IndependentVariable independentVariable in variables.IndependentVariables.Probability) {
                table = independentVariable.AddValuesTo(table);
            }

            foreach (DependentVariable dependentVariable in variables.DependentVariables) {
                table = dependentVariable.AddValuesTo(table);
            }

            return table;
        }

        void AddSkippedColumn() {
            DataColumn skippedColumn = new DataColumn {
                                                          DataType = typeof(bool),
                                                          ColumnName = columnNames.Skipped,
                                                          Unique = false,
                                                          ReadOnly = false,
                                                      };
            baseTrialTable.Columns.Add(skippedColumn);
            foreach (DataRow row in baseTrialTable.Rows) {
                row[columnNames.Skipped] = false;
            }
        }

        void AddAttemptsColumn() {
            DataColumn attemptsColumn = new DataColumn {
                                                           DataType = typeof(int),
                                                           ColumnName = columnNames.Attempts,
                                                           Unique = false,
                                                           ReadOnly = false,
                                                       };
            baseTrialTable.Columns.Add(attemptsColumn);
            foreach (DataRow row in baseTrialTable.Rows) {
                row[columnNames.Attempts] = 0;
            }
        }

        void AddSuccessColumn() {
            DataColumn successColumn = new DataColumn {
                                                          DataType = typeof(bool),
                                                          ColumnName = columnNames.Completed,
                                                          Unique = false,
                                                          ReadOnly = false,
                                                      };
            baseTrialTable.Columns.Add(successColumn);
            foreach (DataRow row in baseTrialTable.Rows) {
                row[columnNames.Completed] = false;
            }
        }

        void AddTrialIndexColumn() {
            DataColumn trialIndexColumn = new DataColumn {
                                                             DataType = typeof(int),
                                                             ColumnName = columnNames.TrialIndex,
                                                             Unique = false,
                                                             ReadOnly = false,
                                                         };
            baseTrialTable.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in baseTrialTable.Rows) {
                row[columnNames.TrialIndex] = columnNames.DefaultMissingValue;
            }
        }

        void AddTotalTrialIndexColumn() {
            DataColumn trialIndexColumn = new DataColumn {
                                                             DataType = typeof(int),
                                                             ColumnName = columnNames.TotalTrialIndex,
                                                             Unique = false,
                                                             ReadOnly = false,
                                                         };
            baseTrialTable.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in baseTrialTable.Rows) {
                row[columnNames.TotalTrialIndex] = columnNames.DefaultMissingValue;
            }
        }

        void AddBlockNumberColumn() {
            DataColumn blockIndexColumn = new DataColumn {
                                                             DataType = typeof(int),
                                                             ColumnName = columnNames.BlockIndex,
                                                             Unique = false,
                                                             ReadOnly = false,
                                                         };
            baseTrialTable.Columns.Add(blockIndexColumn);
            blockIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in baseTrialTable.Rows) {
                row[columnNames.BlockIndex] = columnNames.DefaultMissingValue;
            }
        }

        void AddTrialTimeColumn() {
            DataColumn trailTimeColumn = new DataColumn {
                                                            DataType = typeof(float),
                                                            ColumnName = columnNames.TrialTime,
                                                            Unique = false,
                                                            ReadOnly = false,
                                                        };
            baseTrialTable.Columns.Add(trailTimeColumn);
            foreach (DataRow row in baseTrialTable.Rows) {
                row[columnNames.TrialTime] = 0;
            }
        }

        public DataTable Copy() {
            return baseTrialTable.Copy();
        }

        public DataTable Clone() {
            return baseTrialTable.Clone();
        }

        void AddBlockColumnsFrom(DataTable blockTable) {
            foreach (DataColumn blockTableColumn in blockTable.Columns) {
                baseTrialTable = baseTrialTable.AddColumnFromOtherTable(blockTableColumn, 0);
            }
        }
    }

}