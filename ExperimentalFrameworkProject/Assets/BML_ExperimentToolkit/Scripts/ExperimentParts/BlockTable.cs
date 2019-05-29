using System;
using System.Collections.Generic;
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

        BlockTable(DataTable blockTable) {
            baseBlockTable = blockTable;
        }

        public string AsString() {
            return baseBlockTable.AsString();
        }
        
        public static implicit operator DataTable(BlockTable table) {
            return table.baseBlockTable;
        }

        public List<string> BlockPermutationsStrings {
            get {
                List<string> blockPermutations = new List<string>();
                int blockOrderIndex = 0;
                if (baseBlockTable.Rows.Count == 0) return null;
                if (baseBlockTable.Rows.Count >= 4) throw new TooManyPermutationsException();
                foreach (List<DataRow> dataRows in baseBlockTable.GetPermutations()) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Order #{blockOrderIndex}:   ");
                    foreach (DataRow dataRow in dataRows) {
                        sb.Append($"{dataRow.AsString(separator: ", ", truncateLength: -1)} >   ");
                    }

                    blockPermutations.Add(sb.ToString());
                    blockOrderIndex++;
                }

                return blockPermutations;
            }
        }

        public bool HasBlocks => baseBlockTable.Rows.Count > 0;
        public DataRowCollection Rows => baseBlockTable.Rows;


        public BlockTable GetBlockOrderTableFromPermutations(int index) {

            DataTable orderedTable = baseBlockTable.Clone();

            if (!HasBlocks) return null;

            foreach (DataRow dataRow in baseBlockTable.GetPermutations()[index]) {
                orderedTable.ImportRow(dataRow);
            }

            BlockTable blockOrderTable = new BlockTable(orderedTable);
            return blockOrderTable;
        }

        public BlockTable GetBlockOrderTableFromOrderConfigs(int orderChosenIndex, List<OrderConfig> orderConfigs) {
            
            OrderConfig chosenOrder = orderConfigs[orderChosenIndex];
            
            DataTable orderedTable = baseBlockTable.Clone();
            
            foreach (int index in chosenOrder.OrderedIndices) {
                orderedTable.ImportRow(baseBlockTable.Rows[index]);
            }
            BlockTable blockOrderTable = new BlockTable(orderedTable);
            return blockOrderTable;
        }
    }

    
    public class TooManyPermutationsException : Exception {
        public TooManyPermutationsException()
        {
        }

        public TooManyPermutationsException(string message)
            : base(message)
        {
        }

        public TooManyPermutationsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    
}