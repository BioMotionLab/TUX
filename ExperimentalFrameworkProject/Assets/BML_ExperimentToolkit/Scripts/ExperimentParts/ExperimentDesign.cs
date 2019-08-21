using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_Utilities.Extensions;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class ExperimentDesign {

        readonly ExperimentRunner runner;

        public BlockTable OrderedBlockTable;

        public   List<Block> Blocks;
        readonly BlockTable  baseBlockTable;
        readonly TrialTable  baseTrialTable;
        
        public int    TotalTrials      => Blocks.Count * baseTrialTable.NumberOfTrials;
        public int    BlockCount       => Blocks.Count;
        public string TrialTableHeader => Blocks[0].TrialTable.HeaderAsString(Delimiter.Comma, -1);

        public bool HasBlocks => baseBlockTable.HasBlocks;
        
        readonly VariableConfig config;
        ColumnNamesSettings columnNames;

        public List<string> BlockPermutationsStrings => baseBlockTable.BlockPermutationsStrings;
        public static int MaxBlockPermutationsAllowed => BlockTable.MaxBlockPermutationsAllowed;
        
        public static ExperimentDesign CreateFrom(VariableConfig configFile, ExperimentRunner experimentRunner) {
            return new ExperimentDesign(configFile, experimentRunner);
        }
        
        ExperimentDesign(VariableConfig config,             
                                ExperimentRunner runner) {
            this.config = config;
            this.runner = runner;
            columnNames = config.ColumnNamesSettings;
            baseBlockTable = new BlockTable(config);
            baseTrialTable = new TrialTable(baseBlockTable, config);
        }

        public void FinalizeDesign(int selectedOrderIndex) {
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
                for (int blockRepetitionCount = 0; blockRepetitionCount < config.RepeatAllBlocks; blockRepetitionCount++) {
                    for (int rowIndex = 0; rowIndex < OrderedBlockTable.Rows.Count; rowIndex++) {
                        DataRow orderedBlockRow = OrderedBlockTable.Rows[rowIndex];
                        if (config.ShuffleDifferentlyForEachBlock) {
                            trialTable = trialTable.ShuffleRows();
                        }

                        trialTable = AddBlockValuesToTrialTables(trialTable, orderedBlockRow, (blockRepetitionCount*OrderedBlockTable.Rows.Count)+(rowIndex));
                        string blockIdentity = orderedBlockRow.AsStringWithColumnNames(", ");
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
                trialRow[columnNames.BlockIndex] = 0;
                trialRow[columnNames.TrialIndex] = rowIndex;
                trialRow[columnNames.TotalTrialIndex] = rowIndex;
            }
        }

        void AddTrialAndBlockIndicesWhenThereAreBlocks(DataRow blockTableRow, int blockIndex, DataTable newTable,
                                                       string  columnName,    int startingTotalTrialIndex) {
            for (int rowIndex = 0; rowIndex < newTable.Rows.Count; rowIndex++) {
                DataRow trialRow = newTable.Rows[rowIndex];
                trialRow[columnName] = blockTableRow[columnName];
                trialRow[columnNames.BlockIndex] = blockIndex;
                trialRow[columnNames.TrialIndex] = rowIndex;
                trialRow[columnNames.TotalTrialIndex] = startingTotalTrialIndex;
                startingTotalTrialIndex++;
            }
        }
        
        void WriteParticipantValuesToTables() {
            foreach (ParticipantVariable participantVariable in config.Variables.ParticipantVariables) {
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
                                                              dataRow
                                                             );
            return newBlock;
        }


    }
}