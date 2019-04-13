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

        public List<Block> Blocks;
        readonly BlockTable baseBlockTable;
        TrialTable baseTrialTable;
        SortedVariableContainer sortedVariables;

        public List<string> BlockPermutationsStrings => baseBlockTable.BlockPermutationsStrings;

        public int TotalTrials => Blocks.Count * baseTrialTable.NumberOfTrials;
        public int BlockCount => Blocks.Count;
        public string TrialTableHeader => Blocks[0].TrialTable.HeaderAsString(separator: Delimiter.Comma, truncate: -1);

        public bool HasBlocks => baseBlockTable.HasBlocks;

        readonly bool shuffleTrialsBetweenBlocks;

        public ExperimentDesign(ExperimentRunner runner, List<Variable> allData, bool shuffleTrialOrder,
                                int numberOfRepetitions, bool shuffleTrialsBetweenBlocks) {
            this.runner = runner;
            this.shuffleTrialsBetweenBlocks = shuffleTrialsBetweenBlocks;
            baseBlockTable = new BlockTable(allData, this);
            baseTrialTable = new TrialTable(allData, this, baseBlockTable, shuffleTrialOrder, numberOfRepetitions,
                                            runner.ConfigFile.ColumnNames);
            Enable();
        }
        
        void Enable() {
            ExperimentEvents.OnBlockOrderChosen += BlockOrderSelected;
            ExperimentEvents.OnStartExperiment += ExperimentStarted;
        }

        

        public void Disable() {
            ExperimentEvents.OnBlockOrderChosen -= BlockOrderSelected;
            ExperimentEvents.OnStartExperiment -= ExperimentStarted;
        }

        void BlockOrderSelected(int selectedOrderIndex) {
            OrderedBlockTable = baseBlockTable.GetBlockOrderTable(selectedOrderIndex);
            CreateAndAddBlocks();
        }
        
        public DataTable GetBlockOrderTable(int sessionOrderChosenIndex) {
            BlockTable orderedBlockTable = baseBlockTable.GetBlockOrderTable(sessionOrderChosenIndex);
            return orderedBlockTable;
        }

        void CreateAndAddBlocks() {
            Blocks = new List<Block>();

            if (OrderedBlockTable == null) {
                Debug.Log("No Block Variables");
                DataTable trialTable = baseTrialTable.Copy();
                for (int i = 0; i < trialTable.Rows.Count; i++) {
                    DataRow trialRow = trialTable.Rows[i];
                    trialRow[runner.ConfigFile.ColumnNames.BlockIndex] = 0;
                    trialRow[runner.ConfigFile.ColumnNames.TrialIndex] = i;
                    trialRow[runner.ConfigFile.ColumnNames.TotalTrialIndex] = i;
                }

                const string blockIdentity = "MainCoroutine Block";
                Block newBlock = CreateNewBlock(trialTable, blockIdentity);
                Blocks.Add(newBlock);

            }
            else {
                for (int i = 0; i < OrderedBlockTable.Rows.Count; i++) {
                    DataRow orderedBlockRow = OrderedBlockTable.Rows[i];

                    DataTable trialTable = baseTrialTable.Copy();
                    if (shuffleTrialsBetweenBlocks) {
                        trialTable = trialTable.Shuffle();
                    }

                    trialTable = UpdateWithBlockValues(trialTable, orderedBlockRow, i);

                    string blockIdentity = orderedBlockRow.AsStringWithColumnNames(separator: ", ");
                    Block newBlock = CreateNewBlock(trialTable, blockIdentity);
                    Blocks.Add(newBlock);

                    //Debug.Log($"{newBlock.AsString()}");
                }
            }

            //Debug.Log($"Blocks added {Blocks.Count}");
        }

        void ExperimentStarted(Session session) {
            UpdateParticipantValues();
        }

        void UpdateParticipantValues() {
            
            foreach (ParticipantVariable participantVariable in sortedVariables.ParticipantVariables) {
                
                participantVariable.AddValuesTo(baseTrialTable);
                foreach (Block block in Blocks) {
                    participantVariable.AddValuesTo(block.TrialTable);

                }

            }

        }

        Block CreateNewBlock(DataTable trialTable, string blockIdentity) {
            Block newBlock = (Block)Activator.CreateInstance(runner.BlockType,
                                                              runner,
                                                              trialTable,
                                                              blockIdentity,
                                                              runner.TrialType
                                                             );
            return newBlock;
        }

        DataTable UpdateWithBlockValues(DataTable blockTrialTable, DataRow blockTableRow, int blockIndex) {
            DataTable newTable = blockTrialTable.Copy();

            foreach (DataColumn blockTableColumn in blockTableRow.Table.Columns) {
                string columnName = blockTableColumn.ColumnName;
                int startingTotalTrialIndex = blockIndex * newTable.Rows.Count;
                for (int trialIndexInBlock = 0; trialIndexInBlock < newTable.Rows.Count; trialIndexInBlock++) {
                    DataRow trialRow = newTable.Rows[trialIndexInBlock];
                    trialRow[columnName] = blockTableRow[columnName];
                    trialRow[runner.ConfigFile.ColumnNames.BlockIndex] = blockIndex;
                    trialRow[runner.ConfigFile.ColumnNames.TrialIndex] = trialIndexInBlock;
                    trialRow[runner.ConfigFile.ColumnNames.TotalTrialIndex] = startingTotalTrialIndex;
                    startingTotalTrialIndex++;
                }
            }

            return newTable;
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