using System;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using BML_Utilities.Extensions;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public class RunnableDesign {
        
        public   List<Block>      Blocks;
        readonly ExperimentRunner runner;
        readonly VariableConfigurationFile   configurationFile;
        readonly DataTable finalDataTable;
        public   int              BlockCount       => Blocks.Count;
        public   string           TrialTableHeader => Blocks[0].TrialTable.HeaderAsString(Delimiter.Comma, -1);
        public   int              TotalTrials      => finalDataTable.Rows.Count;
        public   bool             HasBlocks        => Blocks.Count > 1;

        public RunnableDesign(ExperimentRunner runner, DataTable finalDataTable, VariableConfigurationFile configurationFile) {
            this.runner = runner;
            this.configurationFile = configurationFile;
            this.finalDataTable = finalDataTable;
            CreateAndAddBlocks();
            WriteParticipantValuesToTables();
        }

        public static RunnableDesign CreateFromFile(ExperimentRunner runner, string fullPathToFile, VariableConfigurationFile configurationFile) {
            DataTable experimentTable = DataTableCsvUtility.DataTableFromCsv(configurationFile, fullPathToFile);
            return new RunnableDesign(runner, experimentTable, configurationFile);
        }
        
        
        Block CreateNewBlock(DataTable trialTable, DataRow dataRow) {
            Block newBlock = (Block) Activator.CreateInstance(runner.BlockType,
                                                              runner,
                                                              trialTable,
                                                              dataRow
                                                             );
            return newBlock;
        }
        
        void CreateAndAddBlocks() {
            
            List<IndependentVariable> blockVariables = configurationFile.Variables.BlockVariables;
            
            DataTable baseBlockTable = new DataTable();
            foreach (IndependentVariable blockVariable in blockVariables) {
                DataColumn newColumn = new DataColumn(blockVariable.Name, blockVariable.Type);
                baseBlockTable.Columns.Add(newColumn);
            }            

            Blocks = new List<Block>();

            int lastBlockIndex = 0;

            DataTable blockTrialTable = finalDataTable.Clone();
            foreach (DataRow finalTableRow in finalDataTable.Rows) {
                int currentBlockIndex = (int) finalTableRow[configurationFile.ColumnNamesSettings.BlockIndex];
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
            DataRow blockDataRow = blockTable.NewRow();
            foreach (IndependentVariable blockVariable in blockVariables) {
                DataRow firstBlockRow = blockTrialTable.Rows[0];
                blockDataRow[blockVariable.Name] = firstBlockRow[blockVariable.Name];
            }
            Block newBlock = CreateNewBlock(blockTrialTable, blockDataRow);
            Blocks.Add(newBlock);
        }


        void WriteParticipantValuesToTables() {
            foreach (ParticipantVariable participantVariable in configurationFile.Variables.ParticipantVariables) {
                participantVariable.AddValuesTo(finalDataTable);
                foreach (Block block in Blocks) {
                    participantVariable.AddValuesTo(block.TrialTable);
                }
            }
        }
      
    }
}