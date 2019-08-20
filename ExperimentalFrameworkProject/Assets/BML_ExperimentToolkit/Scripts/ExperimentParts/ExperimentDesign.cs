using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using BML_Utilities.Extensions;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public partial class ExperimentDesign {

        readonly ExperimentRunner runner;

        public BlockTable OrderedBlockTable;

        public   List<Block> Blocks;
        readonly BlockTable  baseBlockTable;
        readonly TrialTable  baseTrialTable;
        
        public int    TotalTrials      => Blocks.Count * baseTrialTable.NumberOfTrials;
        public int    BlockCount       => Blocks.Count;
        public string TrialTableHeader => Blocks[0].TrialTable.HeaderAsString(Delimiter.Comma, -1);

        public bool HasBlocks => baseBlockTable.HasBlocks;
        
        SortedVariableContainer sortedVariables;
        readonly VariableConfig variableConfig;

        public List<string> BlockPermutationsStrings => baseBlockTable.BlockPermutationsStrings;
        public static int MaxBlockPermutationsAllowed => BlockTable.MaxBlockPermutationsAllowed;
        
        
        public static ExperimentDesign CreateFrom(VariableConfig variableConfigFile, ExperimentRunner experimentRunner) {
            return new ExperimentDesign(variableConfigFile, experimentRunner);
        }
        
        public ExperimentDesign(VariableConfig variableConfig,             
                                ExperimentRunner runner) {
            this.variableConfig = variableConfig;
            this.runner = runner;
            baseBlockTable = new BlockTable(this, variableConfig);
            baseTrialTable = new TrialTable(this, baseBlockTable, variableConfig);
        }

        public void FinalizeDesign(int selectedOrderIndex) {
            //TODO make sure this is called
            OrderedBlockTable = baseBlockTable.GetOrderedBlockTable(selectedOrderIndex);
            CreateAndAddBlocks();
            WriteParticipantValuesToTables();
        }


        public DataTable GetBlockOrderTable(int sessionOrderChosenIndex) {
            BlockTable orderedBlockTable = baseBlockTable.GetOrderedBlockTable(sessionOrderChosenIndex);
            return orderedBlockTable;
        }
        

        void CreateAndAddBlocks() {
            Blocks = new List<Block>();

            DataTable trialTable = baseTrialTable.Copy();
            if (OrderedBlockTable == null) {
                Debug.Log("No Block Variables");
                AddTrialAndBlockIndicesWhenNoBlocks(trialTable);
                const string blockIdentity = "Main Block (No Block Variables)";
                Block newBlock = CreateNewBlock(trialTable, blockIdentity, null);
                Blocks.Add(newBlock);
            }
            else {
                for (int blockRepetitionCount = 0; blockRepetitionCount < variableConfig.RepeatAllBlocks; blockRepetitionCount++) {
                    for (int rowIndex = 0; rowIndex < OrderedBlockTable.Rows.Count; rowIndex++) {
                        DataRow orderedBlockRow = OrderedBlockTable.Rows[rowIndex];
                        if (variableConfig.ShuffleDifferentlyForEachBlock) {
                            trialTable = trialTable.Shuffle();
                        }

                        trialTable = AddBlockValuesToTrialTables(trialTable, orderedBlockRow, (blockRepetitionCount*OrderedBlockTable.Rows.Count)+(rowIndex));
                        string blockIdentity = orderedBlockRow.AsStringWithColumnNames(separator: ", ");
                        Block newBlock = CreateNewBlock(trialTable, blockIdentity, orderedBlockRow);
                        Blocks.Add(newBlock);
                    }
                }
            }

            //Debug.Log($"Blocks added {Blocks.Count}");
        }

        DataTable AddBlockValuesToTrialTables(DataTable blockTrialTable, DataRow blockTableRow, int blockIndex) {
            DataTable newTable = blockTrialTable.Copy();

            foreach (DataColumn blockTableColumn in blockTableRow.Table.Columns) {
                string columnName = blockTableColumn.ColumnName;
                int startingTotalTrialIndex = blockIndex * newTable.Rows.Count;

                AddTrialAndBlockIndicesWhenThereAreBlocks(blockTableRow, blockIndex, newTable, columnName, startingTotalTrialIndex);
            }

            return newTable;
        }

        void AddTrialAndBlockIndicesWhenNoBlocks(DataTable trialTable) {
            for (int rowIndex = 0; rowIndex < trialTable.Rows.Count; rowIndex++) {
                DataRow trialRow = trialTable.Rows[rowIndex];
                trialRow[variableConfig.ColumnNamesSettings.BlockIndex] = 0;
                trialRow[variableConfig.ColumnNamesSettings.TrialIndex] = rowIndex;
                trialRow[variableConfig.ColumnNamesSettings.TotalTrialIndex] = rowIndex;
            }
        }

        void AddTrialAndBlockIndicesWhenThereAreBlocks(DataRow blockTableRow, int blockIndex, DataTable newTable,
                                                       string  columnName,    int startingTotalTrialIndex) {
            for (int rowIndex = 0; rowIndex < newTable.Rows.Count; rowIndex++) {
                DataRow trialRow = newTable.Rows[rowIndex];
                trialRow[columnName] = blockTableRow[columnName];
                trialRow[variableConfig.ColumnNamesSettings.BlockIndex] = blockIndex;
                trialRow[variableConfig.ColumnNamesSettings.TrialIndex] = rowIndex;
                trialRow[variableConfig.ColumnNamesSettings.TotalTrialIndex] = startingTotalTrialIndex;
                startingTotalTrialIndex++;
            }
        }
        
        void WriteParticipantValuesToTables() {
            foreach (ParticipantVariable participantVariable in sortedVariables.ParticipantVariables) {
                participantVariable.AddValuesTo(baseTrialTable);
                foreach (Block block in Blocks) {
                    participantVariable.AddValuesTo(block.TrialTable);

                }
            }
        }

        Block CreateNewBlock(DataTable trialTable, string blockIdentity, DataRow dataRow) {
            Block newBlock = (Block) Activator.CreateInstance(runner.BlockType,
                                                              runner,
                                                              trialTable,
                                                              blockIdentity,
                                                              runner.TrialType,
                                                              dataRow
                                                             );
            return newBlock;
        }

        public DataTable SortAndAddIVs(List<Variable> allData, bool block = false) {
            DataTable table = new DataTable();
            
            sortedVariables = new SortedVariableContainer(allData, block);

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