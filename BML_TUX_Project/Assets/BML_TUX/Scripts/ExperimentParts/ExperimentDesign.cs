using System;
using System.Collections.Generic;
using System.Data;
using BML_TUX.Scripts.Settings;
using BML_TUX.Scripts.VariableSystem;
using BML_Utilities.Extensions;

namespace BML_TUX.Scripts.ExperimentParts {
    public class ExperimentDesign {
        
        readonly BaseBlockTable  baseBlockTable;
        readonly BaseTrialTable  baseTrialTable;
        BaseBlockTable orderedBlockTable;
        DataTable finalTable;
        
        public bool HasBlocks => baseBlockTable.HasBlocks;
        
        readonly ExperimentDesignFile designFile;
        readonly ColumnNamesSettings columnNames;

        public List<string> BlockPermutationsStrings => GetBlockPermutationsStrings();

        public DataTable BaseBlockTable => baseBlockTable;

        public int NumberOfBlocks => baseBlockTable.Rows.Count;

        public bool RandomizedBlocks =>
            designFile.BlockRandomization == BlockRandomizationMode.CompleteRandomization 
            || designFile.BlockRandomization == BlockRandomizationMode.PartialRandomization;

        public const int MaxBlockPermutationsAllowed = 3;

        public static ExperimentDesign CreateFrom(ExperimentDesignFile configurationFileFile) {
            return new ExperimentDesign(configurationFileFile);
        }

        ExperimentDesign(ExperimentDesignFile designFile) {
            this.designFile = designFile;
            columnNames = designFile.ColumnNamesSettings;
            baseBlockTable = new BaseBlockTable(designFile);
            baseTrialTable = new BaseTrialTable(baseBlockTable, designFile);
        }

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


        public DataTable GetFinalExperimentTable(int selectedOrderIndex) {
            finalTable = BuildFinalTable(selectedOrderIndex);
            return finalTable;
        }

        DataTable BuildFinalTable(int selectedOrderIndex) {
            
            DataTable newFinalTable = baseTrialTable.Clone();
            
            var trialTable = RepeatAndShuffleTrialsIfNeeded();

            newFinalTable = HasBlocks ? 
                CreateTrialTableFromBlockTable(selectedOrderIndex, trialTable) 
                : RepeatAndShuffleAllTrialsIfNeeded(trialTable, newFinalTable);
            
            finalTable = newFinalTable;
            return finalTable;
        }

        DataTable RepeatAndShuffleAllTrialsIfNeeded(DataTable trialTable, DataTable newFinalOverallTable) {
            for (int i = 0; i < designFile.ExperimentRepetitions; i++) {
                
                DataTable singleExperimentRepetition = trialTable.Copy();
                if (designFile.TrialRandomization == TrialRandomizationMode.Randomized
                    && designFile.TrialPermutationType ==
                    TrialPermutationType.DifferentPermutations)
                    singleExperimentRepetition = singleExperimentRepetition.ShuffleRows();

                foreach (DataRow row in singleExperimentRepetition.Rows) {
                    newFinalOverallTable.ImportRow(row);
                }
            }

            if (designFile.TrialRandomization == TrialRandomizationMode.Randomized) {
                newFinalOverallTable = newFinalOverallTable.ShuffleRows();
            }

            AddTrialAndBlockIndicesToTrialTableWithNoBlocks(newFinalOverallTable);

            return newFinalOverallTable;
        }

        void AddTrialAndBlockIndicesToTrialTableWithNoBlocks(DataTable newFinalOverallTable) {
//Add trial indices
            for (int rowIndex = 0; rowIndex < newFinalOverallTable.Rows.Count; rowIndex++) {
                DataRow row = newFinalOverallTable.Rows[rowIndex];
                row[designFile.ColumnNamesSettings.BlockIndex] = 0;
                row[designFile.ColumnNamesSettings.TotalTrialIndex] = rowIndex;
                row[designFile.ColumnNamesSettings.TrialIndex] = rowIndex;
            }
        }

        DataTable RepeatAndShuffleTrialsIfNeeded() {
            //Copy base trial table
            DataTable trialTable = baseTrialTable.Copy();

            //Repeat individual trials if needed

            DataTable repeatedTrialTable = baseTrialTable.Clone();
            if (designFile.TrialRepetitions < 1)
                throw new ArgumentOutOfRangeException("Trial Repetitions cannot be less than 1");
            foreach (DataRow row in baseTrialTable.Rows) {
                for (int i = 0; i < designFile.TrialRepetitions; i++) {
                    repeatedTrialTable.ImportRow(row);
                }
            }

            trialTable = repeatedTrialTable;

            //Shuffle trial order if needed
            if (designFile.TrialRandomization == TrialRandomizationMode.Randomized) {
                trialTable = trialTable.ShuffleRows();
            }

            return trialTable;
        }

        DataTable CreateTrialTableFromBlockTable(int selectedOrderIndex, DataTable trialTable) {

            DataTable newFinalOverallTable = trialTable.Clone();
            
            DataTable blockOrderTableWIthRepetitions = RepeatAndRandomizeBlocksIfNeeded(selectedOrderIndex);

            for (int blockIndex = 0; blockIndex < blockOrderTableWIthRepetitions.Rows.Count; blockIndex++) {
                DataTable newFinalizedTrialTable = trialTable.Copy();
                DataRow orderedBlockRow = blockOrderTableWIthRepetitions.Rows[blockIndex];
                if (designFile.TrialRandomization == TrialRandomizationMode.Randomized
                    && designFile.TrialPermutationType == TrialPermutationType.DifferentPermutations) {
                    newFinalizedTrialTable = newFinalizedTrialTable.ShuffleRows();
                }

                newFinalizedTrialTable = AddBlockValuesToTrialTables(newFinalizedTrialTable, orderedBlockRow, blockIndex);
                newFinalOverallTable.Merge(newFinalizedTrialTable, true, MissingSchemaAction.Error);
            }

            return newFinalOverallTable;
        }

        DataTable RepeatAndRandomizeBlocksIfNeeded(int selectedOrderIndex) {
            DataTable selectedOrderedBlockTable = RandomizeBlockOrderOnceIfNeeded(selectedOrderIndex);

            //create empty block table for adding repetitions
            DataTable blockOrderTableWIthRepetitions = selectedOrderedBlockTable.Clone();

            for (int experimentRepetitionIndex = 0;
                experimentRepetitionIndex < designFile.ExperimentRepetitions;
                experimentRepetitionIndex++) {
                //Create temporary new block table
                DataTable repeatedBlockOrderTable = selectedOrderedBlockTable.Copy();

                //Randomize block order per repetition if needed
                if (designFile.BlockRandomization == BlockRandomizationMode.PartialRandomization
                    && designFile.BlockPartialRandomizationSubType ==
                    BlockPartialRandomizationSubType.DifferentPermutations) {
                    repeatedBlockOrderTable = repeatedBlockOrderTable.ShuffleRows();
                }

                //Add repetition to overall table
                for (int blockRowIndex = 0; blockRowIndex < repeatedBlockOrderTable.Rows.Count; blockRowIndex++) {
                    blockOrderTableWIthRepetitions.ImportRow(repeatedBlockOrderTable.Rows[blockRowIndex]);
                }
            }

            //Randomize across repetitions if needed
            if (designFile.BlockRandomization == BlockRandomizationMode.CompleteRandomization) {
                blockOrderTableWIthRepetitions = blockOrderTableWIthRepetitions.ShuffleRows();
            }

            return blockOrderTableWIthRepetitions;
        }

        DataTable RandomizeBlockOrderOnceIfNeeded(int selectedOrderIndex) {
            DataTable selectedOrderedBlockTable = baseBlockTable.GetOrderedBlockTable(selectedOrderIndex);
            if (RandomizedBlocks) {
                selectedOrderedBlockTable = selectedOrderedBlockTable.ShuffleRows();
            }

            return selectedOrderedBlockTable;
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