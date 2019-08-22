using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public class BaseTrialTable {

        DataTable baseTrialTable;

        readonly Variables           variables;
        readonly ColumnNamesSettings columnNames;
        public   int                 NumberOfTrials => baseTrialTable.Rows.Count;

        public BaseTrialTable(BaseBlockTable baseBlockTable,
                              VariableConfig variableConfig) {

            variables = variableConfig.Variables;
            columnNames = variableConfig.ColumnNamesSettings;


            baseTrialTable = AddVariablesToTable();

            RepeatTrialsIfNeeded(variableConfig);

            //ShuffleRows trial order if needed
            ShuffleTrialsIfNeeded(variableConfig);

            AddMetaColumns(baseBlockTable);
        }

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

        void ShuffleTrialsIfNeeded(VariableConfig variableConfig) {
            if (variableConfig.ShuffleTrialOrder) {
                baseTrialTable = baseTrialTable.ShuffleRows();
            }
        }

        void RepeatTrialsIfNeeded(VariableConfig variableConfig) {
            if (variableConfig.RepeatTrialsInBlock > 1) {
                DataTable repeatedTable = baseTrialTable.Clone();
                for (int i = 0; i < variableConfig.RepeatTrialsInBlock; i++) {
                    foreach (DataRow row in baseTrialTable.Rows) {
                        repeatedTable.ImportRow(row);
                    }
                }

                baseTrialTable = repeatedTable;
            }
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
            //Debug.Log("Adding Block Columns to Trial Table");
            foreach (DataColumn blockTableColumn in blockTable.Columns) {
                //Debug.Log($"Adding Column: {blockTableColumn.ColumnName}");
                baseTrialTable = baseTrialTable.AddColumnFromOtherTable(blockTableColumn, 0);
            }
        }
    }

}