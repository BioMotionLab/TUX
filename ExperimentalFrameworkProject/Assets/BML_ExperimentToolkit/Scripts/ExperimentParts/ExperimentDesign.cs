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

    public class RunnableDesign {
        
        public List<Block> Blocks;
        readonly ExperimentRunner runner;
        readonly VariableConfig config;
        readonly DataTable finalDataTable;
        DataTable baseBlockTable;
        public int         BlockCount       => Blocks.Count;
        public string      TrialTableHeader => Blocks[0].TrialTable.HeaderAsString(Delimiter.Comma, -1);
        public int TotalTrials => finalDataTable.Rows.Count;
        public bool HasBlocks => Blocks.Count > 1;

        public RunnableDesign(ExperimentRunner runner, DataTable finalDataTable, VariableConfig config) {
            this.runner = runner;
            this.config = config;
            this.finalDataTable = finalDataTable;
            CreateAndAddBlocks();
            WriteParticipantValuesToTables();
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
        
        void CreateAndAddBlocks() {
            
            List<IndependentVariable> blockVariables = config.Variables.BlockVariables;
            
            baseBlockTable = new DataTable();
            foreach (IndependentVariable blockVariable in blockVariables) {
                DataColumn newColumn = new DataColumn(blockVariable.Name, blockVariable.Type);
                baseBlockTable.Columns.Add(newColumn);
            }            

            Blocks = new List<Block>();

            int lastBlockIndex = 0;

            DataTable blockTrialTable = finalDataTable.Clone();
            foreach (DataRow finalTableRow in finalDataTable.Rows) {
                int currentBlockIndex = (int) finalTableRow[config.ColumnNamesSettings.BlockIndex];
                if (currentBlockIndex == lastBlockIndex) {
                    blockTrialTable.ImportRow(finalTableRow);
                }
                else {
                    if (blockTrialTable.Rows.Count > 0) {
                        CreateBlockFromTable(baseBlockTable, blockVariables, blockTrialTable);
                    }
                    blockTrialTable = finalDataTable.Clone();
                    blockTrialTable.ImportRow(finalTableRow);
                }

                lastBlockIndex = currentBlockIndex;
            }
            CreateBlockFromTable(baseBlockTable, blockVariables, blockTrialTable);
        }

        void CreateBlockFromTable(DataTable blockTable, List<IndependentVariable> blockVariables, DataTable blockTrialTable) {
            string blockIdentity = "TODO block identity"; //TODO fix block identity
            DataRow blockDataRow = blockTable.NewRow();
            foreach (IndependentVariable blockVariable in blockVariables) {
                DataRow firstBlockRow = blockTrialTable.Rows[0];
                blockDataRow[blockVariable.Name] = firstBlockRow[blockVariable.Name];
            }
            Block newBlock = CreateNewBlock(blockTrialTable, blockIdentity, blockDataRow);
            Blocks.Add(newBlock);
        }


        void WriteParticipantValuesToTables() {
            foreach (ParticipantVariable participantVariable in config.Variables.ParticipantVariables) {
                participantVariable.AddValuesTo(finalDataTable);
                foreach (Block block in Blocks) {
                    participantVariable.AddValuesTo(block.TrialTable);
                }
            }
        }
      
    }


    public class ExperimentDesign {
        
        readonly BaseBlockTable  baseBlockTable;
        readonly BaseTrialTable  baseTrialTable;
        public BaseBlockTable OrderedBlockTable;
        DataTable finalTable;
        
        public bool HasBlocks => baseBlockTable.HasBlocks;
        

        readonly VariableConfig config;
        readonly ColumnNamesSettings columnNames;
        
        
        public static ExperimentDesign CreateFrom(VariableConfig configFile, ExperimentRunner experimentRunner) {
            return new ExperimentDesign(configFile, experimentRunner);
        }

        public List<string> BlockPermutationsStrings => GetBlockPermutationsStrings();

        public const int MaxBlockPermutationsAllowed = 3;
        
        List<string> GetBlockPermutationsStrings() {
            
            if (baseBlockTable.Rows.Count <= MaxBlockPermutationsAllowed && config.OrderConfigs.Count == 0) {
                Debug.Log("returning BlockPermutationStrings");
                return BlockPermutationsStrings;
            }

            if (config.OrderConfigs.Count > 0) return GetBlockOrderConfigStrings();
            
            throw new NullReferenceException("There are too many block values to create a permutation table. " +
                                             "Block orders must be defined manually using OrderConfig files. " +
                                             "See documentation for more information");

        }
        
        
        List<string> GetBlockOrderConfigStrings() {
            List<string> orderStrings = new List<string>();
            foreach (OrderConfig orderConfig in config.OrderConfigs) {
                
                if (orderConfig.Length != baseBlockTable.Rows.Count) {
                    throw new ArgumentException($"OrderConfig file does not match length. See Below:\n" +
                                                $"Need to adjust length of orders.\n" +
                                                $"{orderConfig.name} should be {baseBlockTable.Rows.Count} long. But is {orderConfig.Length}\n\n" +
                                                $"Base Table:" +
                                                $"{baseBlockTable.AsString()}");
                    
                }

                if (orderConfig.Randomize) {
                    orderStrings.Add("Randomize");
                }
                else {
                    string orderString = "";
                    foreach (int orderIndex in orderConfig.OrderedIndices) {
                        string rowString = baseBlockTable.Rows[orderIndex].AsString(separator: ", ", truncateLength: -1);
                        orderString += rowString + " > ";
                    }
                    orderStrings.Add(orderString); 
                }
                
            }

            return orderStrings;
        }
        
        
        ExperimentDesign(VariableConfig config,             
                                ExperimentRunner runner) {
            this.config = config;
            columnNames = config.ColumnNamesSettings;
            baseBlockTable = new BaseBlockTable(config);
            baseTrialTable = new BaseTrialTable(baseBlockTable, config);
        }

        public DataTable GetFinalExperimentTable(int selectedOrderIndex) {
            OrderedBlockTable = baseBlockTable.GetOrderedBlockTable(selectedOrderIndex);
            finalTable = BuildFinalTable();
            return finalTable;
        }

        DataTable BuildFinalTable() {
            DataTable trialTable = baseTrialTable.Copy();
            if (OrderedBlockTable == null) {
                AddTrialAndBlockIndicesWhenNoBlocks(trialTable);
                return trialTable;
            }
            
            var newFinalTable = baseTrialTable.Clone();
            for (int blockRepetitionCount = 0; blockRepetitionCount < config.RepeatAllBlocks; blockRepetitionCount++) {
                for (int rowIndex = 0; rowIndex < OrderedBlockTable.Rows.Count; rowIndex++) {
                    DataRow orderedBlockRow = OrderedBlockTable.Rows[rowIndex];
                    if (config.ShuffleDifferentlyForEachBlock) trialTable = trialTable.ShuffleRows();
                    trialTable = AddBlockValuesToTrialTables(trialTable, orderedBlockRow, (blockRepetitionCount*OrderedBlockTable.Rows.Count)+(rowIndex));
                    newFinalTable.Merge(trialTable, true, MissingSchemaAction.Error);
                }
            }
            return newFinalTable;
        }

        public DataTable GetBlockOrderTable(int sessionOrderChosenIndex) {
            BaseBlockTable orderedBlockTable = baseBlockTable.GetOrderedBlockTable(sessionOrderChosenIndex);
            return orderedBlockTable;
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




    }
}