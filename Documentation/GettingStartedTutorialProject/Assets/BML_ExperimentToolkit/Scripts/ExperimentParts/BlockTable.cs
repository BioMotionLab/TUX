﻿using System.Collections.Generic;
using System.Data;
using System.Text;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using BML_Utilities.Extensions;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class BlockTable {

        readonly DataTable baseBlockTable;

        public BlockTable(List<Variable> allData, ExperimentDesign design) {

            //Get block Variables
            List<Variable> blockVariables = new List<Variable>();
            foreach (Variable datum in allData) {
                if (datum.TypeOfVariable == VariableType.Independent) {
                    IndependentVariable independentVariable = (IndependentVariable)datum;
                    if (independentVariable.Block) {
                        blockVariables.Add(independentVariable);
                    }
                }
            }

            baseBlockTable = design.SortAndAddIVs(blockVariables, true);
   
        }

        private BlockTable(DataTable blockTable) {
            baseBlockTable = blockTable;
        }

        public static implicit operator DataTable(BlockTable table) {
            return table.baseBlockTable;
        }

        public List<string> BlockPermutationsStrings {
            get {
                List<string> blockPermutations = new List<string>();
                int blockOrderIndex = 0;
                if (baseBlockTable.Rows.Count == 0) return null;
                foreach (List<DataRow> dataRows in baseBlockTable.GetPermutations()) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Order #{blockOrderIndex}:   ");
                    foreach (DataRow dataRow in dataRows) {
                        sb.Append($"{dataRow.AsString(separator: ", ", truncate: -1)} >   ");
                    }

                    blockPermutations.Add(sb.ToString());
                    blockOrderIndex++;
                }

                return blockPermutations;
            }
        }

        public bool HasBlocks => baseBlockTable.Rows.Count > 0;
        public DataRowCollection Rows => baseBlockTable.Rows;


        public BlockTable GetBlockOrderTable(int index) {

            DataTable orderedTable = baseBlockTable.Clone();

            if (!HasBlocks) return null;

            foreach (DataRow dataRow in baseBlockTable.GetPermutations()[index]) {
                orderedTable.ImportRow(dataRow);
            }

            BlockTable blockOrderTable = new BlockTable(orderedTable);
            return blockOrderTable;
        }

    }


}