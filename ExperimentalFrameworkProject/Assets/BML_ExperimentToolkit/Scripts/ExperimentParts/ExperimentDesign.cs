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

        public const int MaxBlockPermutationsAllowed = 3;

        public List<string> BlockPermutationsStrings {
            get {
                
                if (baseBlockTable.Rows.Count <= MaxBlockPermutationsAllowed && orderConfigs.Count == 0) {
                    return baseBlockTable.BlockPermutationsStrings;
                }

                if (orderConfigs.Count > 0) return GetBlockOrderConfigStrings();
                
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                Application.Quit();
                throw new NullReferenceException("There are too many block values to create a permutation table. " +
                                                 "Block orders must be defined manually using OrderConfig files. " +
                                                 "See documentation for more information");



            }
        }

        List<string> GetBlockOrderConfigStrings() {
            List<string> orderStrings = new List<string>();
            foreach (OrderConfig orderConfig in orderConfigs) {
                
                if (orderConfig.Length != baseBlockTable.Rows.Count) {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                    Application.Quit();
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

        readonly List<OrderConfig> orderConfigs;

        public int    TotalTrials      => Blocks.Count * baseTrialTable.NumberOfTrials;
        public int    BlockCount       => Blocks.Count;
        public string TrialTableHeader => Blocks[0].TrialTable.HeaderAsString(separator: Delimiter.Comma, truncate: -1);

        public bool HasBlocks => baseBlockTable.HasBlocks;
        
        readonly bool shuffleTrialsBetweenBlocks;
        readonly int repeatBlocks;
        SortedVariableContainer sortedVariables;

        public ExperimentDesign(ExperimentRunner runner,              List<Variable> allData, bool shuffleTrialOrder,
                                int              RepeatTrials, bool           shuffleTrialsBetweenBlocks, int repeatBlocks, List<OrderConfig> orderConfigs) {
            this.runner = runner;
            this.shuffleTrialsBetweenBlocks = shuffleTrialsBetweenBlocks;
            this.repeatBlocks = repeatBlocks;
            this.orderConfigs = orderConfigs;
            baseBlockTable = new BlockTable(allData, this);
            baseTrialTable = new TrialTable(allData, this, baseBlockTable, shuffleTrialOrder, RepeatTrials,
                                            runner.VariableConfigFile.ColumnNamesSettings);
            Enable();
        }

        void Enable() {
            ExperimentEvents.OnBlockOrderChosen += BlockOrderSelected;
            ExperimentEvents.OnStartRunningExperiment += RunningExperimentStarted;
        }



        public void Disable() {
            ExperimentEvents.OnBlockOrderChosen -= BlockOrderSelected;
            ExperimentEvents.OnStartRunningExperiment -= RunningExperimentStarted;
        }

        void BlockOrderSelected(int selectedOrderIndex) {
            OrderedBlockTable = GetOrderedBlockTable(selectedOrderIndex);
            CreateAndAddBlocks();
        }

        public DataTable GetBlockOrderTable(int sessionOrderChosenIndex) {
            BlockTable orderedBlockTable = GetOrderedBlockTable(sessionOrderChosenIndex);
            return orderedBlockTable;
        }

        private BlockTable GetOrderedBlockTable(int OrderChosenIndex) {
            BlockTable orderedBlockTable;
            if (baseBlockTable.Rows.Count <= MaxBlockPermutationsAllowed) {
                orderedBlockTable = baseBlockTable.GetBlockOrderTableFromPermutations(OrderChosenIndex);
            }
            else {
                orderedBlockTable = baseBlockTable.GetBlockOrderTableFromOrderConfigs(OrderChosenIndex, orderConfigs);
            }
            
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
                for (int j = 0; j < repeatBlocks; j++) {
                    for (int i = 0; i < OrderedBlockTable.Rows.Count; i++) {
                        DataRow orderedBlockRow = OrderedBlockTable.Rows[i];
                        if (shuffleTrialsBetweenBlocks) {
                            trialTable = trialTable.Shuffle();
                        }

                        trialTable = UpdateTrialTableWithBlockValues(trialTable, orderedBlockRow, (j*OrderedBlockTable.Rows.Count)+(i));
                        string blockIdentity = orderedBlockRow.AsStringWithColumnNames(separator: ", ");
                        Block newBlock = CreateNewBlock(trialTable, blockIdentity, orderedBlockRow);
                        Blocks.Add(newBlock);
                    }
                }
            }

         
         

            //Debug.Log($"Blocks added {Blocks.Count}");
        }

        DataTable UpdateTrialTableWithBlockValues(DataTable blockTrialTable, DataRow blockTableRow, int blockIndex) {
            DataTable newTable = blockTrialTable.Copy();

            foreach (DataColumn blockTableColumn in blockTableRow.Table.Columns) {
                string columnName = blockTableColumn.ColumnName;
                int startingTotalTrialIndex = blockIndex * newTable.Rows.Count;

                AddTrialAndBlockIndicesWhenThereAreBlocks(blockTableRow, blockIndex, newTable, columnName, startingTotalTrialIndex);
            }

            return newTable;
        }

        void AddTrialAndBlockIndicesWhenNoBlocks(DataTable trialTable) {
            for (int i = 0; i < trialTable.Rows.Count; i++) {
                DataRow trialRow = trialTable.Rows[i];
                trialRow[runner.VariableConfigFile.ColumnNamesSettings.BlockIndex] = 0;
                trialRow[runner.VariableConfigFile.ColumnNamesSettings.TrialIndex] = i;
                trialRow[runner.VariableConfigFile.ColumnNamesSettings.TotalTrialIndex] = i;
            }
        }

        void AddTrialAndBlockIndicesWhenThereAreBlocks(DataRow blockTableRow, int blockIndex, DataTable newTable,
                                                       string  columnName,    int startingTotalTrialIndex) {
            for (int i = 0; i < newTable.Rows.Count; i++) {
                DataRow trialRow = newTable.Rows[i];
                trialRow[columnName] = blockTableRow[columnName];
                trialRow[runner.VariableConfigFile.ColumnNamesSettings.BlockIndex] = blockIndex;
                trialRow[runner.VariableConfigFile.ColumnNamesSettings.TrialIndex] = i;
                trialRow[runner.VariableConfigFile.ColumnNamesSettings.TotalTrialIndex] = startingTotalTrialIndex;
                startingTotalTrialIndex++;
            }
        }


        void RunningExperimentStarted(Session session) {
            WriteParticipantValuesToTables();
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