using System;
using System.Collections.Generic;
using System.Data;
using bmlTUX.Scripts.Utilities;
using bmlTUX.Scripts.Utilities.Extensions;
using bmlTUX.Scripts.VariableSystem;

namespace bmlTUX.Scripts.ExperimentParts {
    public class RunnableDesign {
        
        public   List<Block>      Blocks;
        readonly ExperimentRunner runner;
        readonly ExperimentDesignFile   designFile;
        readonly DataTable finalDataTable;
        public   int              BlockCount       => Blocks.Count;
        public   string           TrialTableHeader => Blocks[0].TrialTable.HeaderAsString(Delimiter.Comma, -1);
        public   int              TotalTrials      => finalDataTable.Rows.Count;
        public   bool             HasBlocks        => Blocks.Count > 1;

        public RunnableDesign(ExperimentRunner runner, DataTable finalDataTable, ExperimentDesignFile designFile) {
            this.runner = runner;
            this.designFile = designFile;
            this.finalDataTable = finalDataTable;
            CreateAndAddBlocks();
            WriteParticipantValuesToTables();
        }

        public static RunnableDesign CreateFromFile(ExperimentRunner runner, string fullPathToFile, ExperimentDesignFile configurationFile) {
            DataTable experimentTable = DataTableCsvUtility.DataTableFromCsv(configurationFile, fullPathToFile);
            return new RunnableDesign(runner, experimentTable, configurationFile);
        }
        
        
        Block CreateNewBlock(DataTable trialTable, DataRow dataRow, int index) {
            Block newBlock = (Block) Activator.CreateInstance(runner.BlockType,
                                                              runner,
                                                              trialTable,
                                                              dataRow,
                                                              index
                                                             );
            return newBlock;
        }
        
        void CreateAndAddBlocks() {
            
            List<IndependentVariable> blockVariables = designFile.Variables.BlockVariables;
            
            DataTable baseBlockTable = new DataTable();
            foreach (IndependentVariable blockVariable in blockVariables) {
                DataColumn newColumn = new DataColumn(blockVariable.Name, blockVariable.Type);
                baseBlockTable.Columns.Add(newColumn);
            }            

            Blocks = new List<Block>();

            int lastBlockIndex = 0;
            int currentBlockIndex = -998;
            DataTable blockTrialTable = finalDataTable.Clone();
            foreach (DataRow finalTableRow in finalDataTable.Rows) {
                currentBlockIndex = (int) finalTableRow[designFile.ColumnNamesSettings.BlockIndex];
                if (currentBlockIndex == lastBlockIndex) {
                    blockTrialTable.ImportRow(finalTableRow);
                }
                else {
                    if (blockTrialTable.Rows.Count > 0) {
                        Block newBlock = CreateBlockFromTable(baseBlockTable, blockVariables, blockTrialTable, currentBlockIndex);
                        Blocks.Add(newBlock);
                    }
                    blockTrialTable = finalDataTable.Clone();
                    blockTrialTable.ImportRow(finalTableRow);
                }

                lastBlockIndex = currentBlockIndex;
            }

            currentBlockIndex++;
            Block lastBlock = CreateBlockFromTable(baseBlockTable, blockVariables, blockTrialTable, currentBlockIndex);
            Blocks.Add(lastBlock);
        }

        Block CreateBlockFromTable(DataTable blockTable,      List<IndependentVariable> blockVariables,
                                  DataTable blockTrialTable, int                       currentBlockIndex) {
            DataRow blockDataRow = blockTable.NewRow();
            foreach (IndependentVariable blockVariable in blockVariables) {
                DataRow firstBlockRow = blockTrialTable.Rows[0];
                blockDataRow[blockVariable.Name] = firstBlockRow[blockVariable.Name];
            }
            Block newBlock = CreateNewBlock(blockTrialTable, blockDataRow, currentBlockIndex-1);
            return newBlock;
        }


        void WriteParticipantValuesToTables() {
            foreach (ParticipantVariable participantVariable in designFile.Variables.ParticipantVariables) {
                participantVariable.AddValuesTo(finalDataTable);
                foreach (Block block in Blocks) {
                    participantVariable.AddValuesTo(block.TrialTable);
                }
            }
        }
      
    }
}