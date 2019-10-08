using BML_ExperimentToolkit.Scripts.VariableSystem;
using System;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_Utilities.Extensions;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public class ExperimentDesign {
        
        readonly BaseBlockTable  baseBlockTable;
        readonly BaseTrialTable  baseTrialTable;
        BaseBlockTable orderedBlockTable;
        DataTable finalTable;
        
        public bool HasBlocks => baseBlockTable.HasBlocks;
        

        readonly ExperimentDesignFile designFile;
        readonly ColumnNamesSettings columnNames;
        
        
        public static ExperimentDesign CreateFrom(ExperimentDesignFile configurationFileFile) {
            return new ExperimentDesign(configurationFileFile);
        }

        public List<string> BlockPermutationsStrings => GetBlockPermutationsStrings();
        public DataTable BaseBlockTable => baseBlockTable;
        public int NumberOfBlocks => baseBlockTable.Rows.Count;

        public const int MaxBlockPermutationsAllowed = 3;
        
        List<string> GetBlockPermutationsStrings() {
            
            if (baseBlockTable.Rows.Count <= MaxBlockPermutationsAllowed && designFile.BlockOrderConfigurations.Count == 0) {
                return baseBlockTable.BlockPermutationsStrings;
            }

            if (designFile.BlockOrderConfigurations.Count > 0) return GetBlockOrderConfigStrings();
            
            throw new NullReferenceException("There are too many block values to create a permutation table. " +
                                             "Block orders must be defined manually using OrderConfig files. " +
                                             "See documentation for more information");

        }
        
        List<string> GetBlockOrderConfigStrings() {
            List<string> orderStrings = new List<string>();
            foreach (BlockOrderDefinition orderConfig in designFile.BlockOrderConfigurations) {

                if (orderConfig.Randomize) {
                    orderStrings.Add("Randomize");
                }
                else {
                    string orderString = "";
                    foreach (int orderIndex in orderConfig.IndexOrder) {
                        string rowString = baseBlockTable.Rows[orderIndex].AsString(separator: ", ", truncateLength: -1);
                        orderString += rowString + " > ";
                    }
                    orderStrings.Add(orderString); 
                }
                
            }

            return orderStrings;
        }

        ExperimentDesign(ExperimentDesignFile designFile) {
            this.designFile = designFile;
            columnNames = designFile.ColumnNamesSettings;
            baseBlockTable = new BaseBlockTable(designFile);
            baseTrialTable = new BaseTrialTable(baseBlockTable, designFile);
        }

        
        public DataTable GetFinalExperimentTable(int selectedOrderIndex) {
            orderedBlockTable = baseBlockTable.GetOrderedBlockTable(selectedOrderIndex);
            finalTable = BuildFinalTable();
            return finalTable;
        }

        DataTable BuildFinalTable() {
            DataTable trialTable = baseTrialTable.Copy();
            if (orderedBlockTable == null) {
                AddTrialAndBlockIndicesWhenNoBlocks(trialTable);
                return trialTable;
            }
            
            var newFinalTable = baseTrialTable.Clone();
            for (int blockRepetitionCount = 0; blockRepetitionCount < designFile.RepeatAllBlocks; blockRepetitionCount++) {
                for (int rowIndex = 0; rowIndex < orderedBlockTable.Rows.Count; rowIndex++) {
                    DataRow orderedBlockRow = orderedBlockTable.Rows[rowIndex];
                    if (designFile.RandomizationMode == RandomizationMode.RandomizeCompletely) trialTable = trialTable.ShuffleRows();
                    trialTable = AddBlockValuesToTrialTables(trialTable, orderedBlockRow, (blockRepetitionCount*orderedBlockTable.Rows.Count)+(rowIndex));
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