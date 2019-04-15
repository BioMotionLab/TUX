﻿using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public class TrialTable {

        public DataTable BaseTrialTable;
        readonly ColumnNames columnNames;
        public int NumberOfTrials => BaseTrialTable.Rows.Count;

        public TrialTable(List<Variable> allData,  ExperimentDesign design,           BlockTable  baseBlockTable, bool shuffleBaseTrialOrder,
                          int            numberOfRepetitions, ColumnNames columnNames) {
            this.columnNames = columnNames;
            BaseTrialTable = design.SortAndAddIVs(allData);

            //Repeat all trials if specified
            if (numberOfRepetitions > 1) {
                DataTable repeatedTable = BaseTrialTable.Clone();
                for (int i = 0; i < numberOfRepetitions; i++) {
                    foreach (DataRow row in BaseTrialTable.Rows) {
                        repeatedTable.ImportRow(row);
                    }
                }

                BaseTrialTable = repeatedTable;
            }

            //Shuffle trial order if needed
            if (shuffleBaseTrialOrder) {
                BaseTrialTable = BaseTrialTable.Shuffle();
            }

            AddBlockColumnsFrom(baseBlockTable);

            AddTotalTrialIndexColumnTo();
            AddTrialIndexColumnTo();
            AddBlockNumberColumnTo();
            AddSuccessColumnTo();
            AddAttemptsColumnTo();
            AddSkippedColumnTo();
            AddTrialTimeColumnTo();
            



        }

        public static implicit operator DataTable(TrialTable table) {
            return table.BaseTrialTable;
        }

        public DataRowCollection Rows => BaseTrialTable.Rows;

        void AddSkippedColumnTo() {
            DataColumn skippedColumn = new DataColumn {
                DataType = typeof(bool),
                ColumnName = columnNames.Skipped,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(skippedColumn);
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNames.Skipped] = false;
            }
        }

        void AddAttemptsColumnTo() {
            DataColumn attemptsColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = columnNames.Attempts,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(attemptsColumn);
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNames.Attempts] = 0;
            }
        }

        void AddSuccessColumnTo() {
            DataColumn successColumn = new DataColumn {
                DataType = typeof(bool),
                ColumnName = columnNames.Completed,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(successColumn);
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNames.Completed] = false;
            }
        }

        void AddTrialIndexColumnTo() {
            DataColumn trialIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = columnNames.TrialIndex,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNames.TrialIndex] = columnNames.DefaultMissingValue;
            }
        }

        void AddTotalTrialIndexColumnTo() {
            DataColumn trialIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = columnNames.TotalTrialIndex,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNames.TotalTrialIndex] = columnNames.DefaultMissingValue;
            }
        }

        void AddBlockNumberColumnTo() {
            DataColumn blockIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = columnNames.BlockIndex,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(blockIndexColumn);
            blockIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNames.BlockIndex] = columnNames.DefaultMissingValue;
            }
        }
        void AddTrialTimeColumnTo() {
            DataColumn trailTimeColumn = new DataColumn {
                                                             DataType = typeof(float),
                                                             ColumnName = columnNames.TrialTime,
                                                             Unique = false,
                                                             ReadOnly = false,
                                                         };
            BaseTrialTable.Columns.Add(trailTimeColumn);
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNames.TrialTime] = 0;
            }
        }

        public DataTable Copy() {
            return BaseTrialTable.Copy();
        }

        void AddBlockColumnsFrom(DataTable blockTable) {
            //Debug.Log("Adding Block Columns to Trial Table");
            foreach (DataColumn blockTableColumn in blockTable.Columns) {
                //Debug.Log($"Adding Column: {blockTableColumn.ColumnName}");
                BaseTrialTable = BaseTrialTable.AddColumnFromOtherTable(blockTableColumn, 0);
            }
        }
    }
}