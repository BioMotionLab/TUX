using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public class TrialTable {

        public DataTable BaseTrialTable;
        readonly ColumnNamesSettings columnNamesSettings;
        public int NumberOfTrials => BaseTrialTable.Rows.Count;
        ExperimentDesign design;

        public TrialTable(List<Variable> allData,  
                          ExperimentDesign design,           
                          BlockTable  baseBlockTable, 
                          bool shuffleBaseTrialOrder,
                          int numberOfRepetitions, 
                          ColumnNamesSettings columnNamesSettings) 
        {
            this.columnNamesSettings = columnNamesSettings;
            this.design = design;
            BaseTrialTable = SortAndAddIVs(allData);

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
                ColumnName = columnNamesSettings.Skipped,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(skippedColumn);
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNamesSettings.Skipped] = false;
            }
        }

        void AddAttemptsColumnTo() {
            DataColumn attemptsColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = columnNamesSettings.Attempts,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(attemptsColumn);
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNamesSettings.Attempts] = 0;
            }
        }

        void AddSuccessColumnTo() {
            DataColumn successColumn = new DataColumn {
                DataType = typeof(bool),
                ColumnName = columnNamesSettings.Completed,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(successColumn);
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNamesSettings.Completed] = false;
            }
        }

        void AddTrialIndexColumnTo() {
            DataColumn trialIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = columnNamesSettings.TrialIndex,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNamesSettings.TrialIndex] = columnNamesSettings.DefaultMissingValue;
            }
        }

        void AddTotalTrialIndexColumnTo() {
            DataColumn trialIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = columnNamesSettings.TotalTrialIndex,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNamesSettings.TotalTrialIndex] = columnNamesSettings.DefaultMissingValue;
            }
        }

        void AddBlockNumberColumnTo() {
            DataColumn blockIndexColumn = new DataColumn {
                DataType = typeof(int),
                ColumnName = columnNamesSettings.BlockIndex,
                Unique = false,
                ReadOnly = false,
            };
            BaseTrialTable.Columns.Add(blockIndexColumn);
            blockIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNamesSettings.BlockIndex] = columnNamesSettings.DefaultMissingValue;
            }
        }
        void AddTrialTimeColumnTo() {
            DataColumn trailTimeColumn = new DataColumn {
                                                             DataType = typeof(float),
                                                             ColumnName = columnNamesSettings.TrialTime,
                                                             Unique = false,
                                                             ReadOnly = false,
                                                         };
            BaseTrialTable.Columns.Add(trailTimeColumn);
            foreach (DataRow row in BaseTrialTable.Rows) {
                row[columnNamesSettings.TrialTime] = 0;
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




        public DataTable SortAndAddIVs(List<Variable> allData, bool block = false) {
            DataTable table = new DataTable();


            SortedVariableContainer sortedVariables = new SortedVariableContainer(allData, block);
            design.SortedVariables = sortedVariables;
            //Order matters.
            foreach (IndependentVariable independentVariable in sortedVariables.BalancedIndependentVariables) {
                table = independentVariable.AddValuesTo(table);
            }

            foreach (IndependentVariable independentVariable in sortedVariables.LoopedIndependentVariables) {
                table = independentVariable.AddValuesTo(table);
            }

            foreach (IndependentVariable independentVariable in sortedVariables.ProbabilityIndependentVariables) {
                table = independentVariable.AddValuesTo(table);
            }

            foreach (DependentVariable dependentVariable in sortedVariables.DependentVariables) {
                table = dependentVariable.AddValuesTo(table);
            }


            return table;
        }
    }
}