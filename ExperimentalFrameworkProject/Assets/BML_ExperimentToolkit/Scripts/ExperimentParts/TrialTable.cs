using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public class TrialTable {

        DataTable baseTrialTable;
        ColumnNames ColumnNames;
        public int Trials => baseTrialTable.Rows.Count;

        public TrialTable(List<Variable> allData, bool shuffleTrialOrder, int numberOfRepetitions, ColumnNames columnNames) {
            ColumnNames = columnNames;
            baseTrialTable = ExperimentDesign.SortAndAddIVs(allData);

            //Repeat all trials if specified
            if (numberOfRepetitions > 1) {
                DataTable repeatedTable = baseTrialTable.Clone();
                for (int i = 0; i < numberOfRepetitions; i++) {
                    foreach (DataRow row in baseTrialTable.Rows) {
                        repeatedTable.ImportRow(row);
                    }
                }

                baseTrialTable = repeatedTable;
            }

            //Shuffle trial order if needed
            if (shuffleTrialOrder) {
                baseTrialTable = baseTrialTable.Shuffle();
            }

            AddTotalTrialIndexColumnTo(baseTrialTable);
            AddTrialIndexColumnTo(baseTrialTable);
            AddBlockNumberColumnTo(baseTrialTable);
            AddSuccessColumnTo(baseTrialTable);
            AddAttemptsColumnTo(baseTrialTable);
            AddSkippedColumnTo(baseTrialTable);

        }

        public static implicit operator DataTable(TrialTable table) {
            return table.baseTrialTable;
        }

        public DataRowCollection Rows => baseTrialTable.Rows;

        void AddSkippedColumnTo(DataTable table) {
            DataColumn skippedColumn = new DataColumn {
                DataType = typeof(bool),
                ColumnName = ColumnNames.Skipped,
                Unique = false,
                ReadOnly = false,
            };
            table.Columns.Add(skippedColumn);
            foreach (DataRow row in table.Rows) {
                row[ColumnNames.Skipped] = false;
            }
        }

        void AddAttemptsColumnTo(DataTable table) {
            DataColumn attemptsColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = ColumnNames.Attempts,
                Unique = false,
                ReadOnly = false,
            };
            table.Columns.Add(attemptsColumn);
            foreach (DataRow row in table.Rows) {
                row[ColumnNames.Attempts] = 0;
            }
        }

        void AddSuccessColumnTo(DataTable table) {
            DataColumn successColumn = new DataColumn {
                DataType = typeof(bool),
                ColumnName = ColumnNames.Completed,
                Unique = false,
                ReadOnly = false,
            };
            table.Columns.Add(successColumn);
            foreach (DataRow row in table.Rows) {
                row[ColumnNames.Completed] = false;
            }
        }

        void AddTrialIndexColumnTo(DataTable table) {
            DataColumn trialIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = ColumnNames.TrialIndex,
                Unique = false,
                ReadOnly = false,
            };
            table.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in table.Rows) {
                row[ColumnNames.TrialIndex] = -1;
            }
        }

        void AddTotalTrialIndexColumnTo(DataTable table) {
            DataColumn trialIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = ColumnNames.TotalTrialIndex,
                Unique = false,
                ReadOnly = false,
            };
            table.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in table.Rows) {
                row[ColumnNames.TotalTrialIndex] = -1;
            }
        }

        void AddBlockNumberColumnTo(DataTable table) {
            DataColumn blockIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = ColumnNames.BlockIndex,
                Unique = false,
                ReadOnly = false,
            };
            table.Columns.Add(blockIndexColumn);
            blockIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in table.Rows) {
                row[ColumnNames.BlockIndex] = -1;
            }
        }

        public DataTable Copy() {
            return baseTrialTable.Copy();
        }

        public void AddBlockColumns(DataTable blockTable) {
            foreach (DataColumn blockTableColumn in blockTable.Columns) {
                baseTrialTable.AddColumnFromOtherTable(blockTableColumn, 0);
            }
        }
    }
}