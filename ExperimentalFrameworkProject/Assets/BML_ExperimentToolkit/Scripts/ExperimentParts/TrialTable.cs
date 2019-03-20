﻿using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public class TrialTable {

        DataTable baseTrialTable;
        ColumnNames ColumnNames;
        public int Trials => baseTrialTable.Rows.Count;

        public TrialTable(List<Variable> allData,             BlockTable  baseBlockTable, bool shuffleBaseTrialOrder,
                          int            numberOfRepetitions, ColumnNames columnNames) {
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
            if (shuffleBaseTrialOrder) {
                baseTrialTable = baseTrialTable.Shuffle();
            }

            AddBlockColumnsFrom(baseBlockTable);

            AddTotalTrialIndexColumnTo();
            AddTrialIndexColumnTo();
            AddBlockNumberColumnTo();
            AddSuccessColumnTo();
            AddAttemptsColumnTo();
            AddSkippedColumnTo();
            
            

        }

        public static implicit operator DataTable(TrialTable table) {
            return table.baseTrialTable;
        }

        public DataRowCollection Rows => baseTrialTable.Rows;

        void AddSkippedColumnTo() {
            DataColumn skippedColumn = new DataColumn {
                DataType = typeof(bool),
                ColumnName = ColumnNames.Skipped,
                Unique = false,
                ReadOnly = false,
            };
            baseTrialTable.Columns.Add(skippedColumn);
            foreach (DataRow row in baseTrialTable.Rows) {
                row[ColumnNames.Skipped] = false;
            }
        }

        void AddAttemptsColumnTo() {
            DataColumn attemptsColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = ColumnNames.Attempts,
                Unique = false,
                ReadOnly = false,
            };
            baseTrialTable.Columns.Add(attemptsColumn);
            foreach (DataRow row in baseTrialTable.Rows) {
                row[ColumnNames.Attempts] = 0;
            }
        }

        void AddSuccessColumnTo() {
            DataColumn successColumn = new DataColumn {
                DataType = typeof(bool),
                ColumnName = ColumnNames.Completed,
                Unique = false,
                ReadOnly = false,
            };
            baseTrialTable.Columns.Add(successColumn);
            foreach (DataRow row in baseTrialTable.Rows) {
                row[ColumnNames.Completed] = false;
            }
        }

        void AddTrialIndexColumnTo() {
            DataColumn trialIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = ColumnNames.TrialIndex,
                Unique = false,
                ReadOnly = false,
            };
            baseTrialTable.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in baseTrialTable.Rows) {
                row[ColumnNames.TrialIndex] = -1;
            }
        }

        void AddTotalTrialIndexColumnTo() {
            DataColumn trialIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = ColumnNames.TotalTrialIndex,
                Unique = false,
                ReadOnly = false,
            };
            baseTrialTable.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in baseTrialTable.Rows) {
                row[ColumnNames.TotalTrialIndex] = -1;
            }
        }

        void AddBlockNumberColumnTo() {
            DataColumn blockIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = ColumnNames.BlockIndex,
                Unique = false,
                ReadOnly = false,
            };
            baseTrialTable.Columns.Add(blockIndexColumn);
            blockIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in baseTrialTable.Rows) {
                row[ColumnNames.BlockIndex] = -1;
            }
        }

        public DataTable Copy() {
            return baseTrialTable.Copy();
        }

        public void AddBlockColumnsFrom(DataTable blockTable) {
            //Debug.Log("Adding Block Columns to Trial Table");
            foreach (DataColumn blockTableColumn in blockTable.Columns) {
                //Debug.Log($"Adding Column: {blockTableColumn.ColumnName}");
                baseTrialTable = baseTrialTable.AddColumnFromOtherTable(blockTableColumn, 0);
            }
        }
    }
}