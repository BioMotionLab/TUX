using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using BML_Utilities.Extensions;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class BaseBlockTable {

        readonly DataTable baseBlockTable;
        BaseBlockTable currentOrderedTable;
        int currentOrderedTableIndex = -1;
        
        readonly List<OrderConfig> orderConfigs;
        readonly IndependentVariables blockVariables;

        public BaseBlockTable(VariableConfig variableConfig) {
            orderConfigs = variableConfig.OrderConfigs;
            blockVariables = variableConfig.Variables.BlockVariables;
            baseBlockTable = AddVariablesToTable();
   
        }

        BaseBlockTable(DataTable blockTable) {
            baseBlockTable = blockTable;
        }

        public string AsString() {
            return baseBlockTable.AsString();
        }
        
        DataTable AddVariablesToTable() {
            DataTable table = new DataTable();

            //Order matters.
            foreach (IndependentVariable independentVariable in blockVariables.Looped) {
                table = independentVariable.AddValuesTo(table);
            }
            foreach (IndependentVariable independentVariable in blockVariables.Balanced) {
                table = independentVariable.AddValuesTo(table);
            }
            foreach (IndependentVariable independentVariable in blockVariables.Probability) {
                table = independentVariable.AddValuesTo(table);
            }

            return table;
        }
        
        public static implicit operator DataTable(BaseBlockTable table) {
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


        BaseBlockTable GetBlockOrderTableFromPermutations(int index) {

            DataTable orderedTable = baseBlockTable.Clone();

            if (!HasBlocks) return null;

            foreach (DataRow dataRow in baseBlockTable.GetPermutations()[index]) {
                orderedTable.ImportRow(dataRow);
            }

            BaseBlockTable blockOrderTable = new BaseBlockTable(orderedTable);
            return blockOrderTable;
        }

        BaseBlockTable GetBlockOrderTableFromOrderConfigs(int orderChosenIndex, List<OrderConfig> orderConfigs) {
 
            if (orderChosenIndex > orderConfigs.Count- 1) throw new IndexOutOfRangeException($"Index chosen is {orderChosenIndex}, but count is {orderConfigs.Count}");
            
            OrderConfig chosenOrder = orderConfigs[orderChosenIndex];

            if (chosenOrder.Randomize && currentOrderedTableIndex == orderChosenIndex) {
                return currentOrderedTable;
            } 
            
            DataTable orderedTable = baseBlockTable.Clone();

            foreach (int index in chosenOrder.OrderedIndices) {
                orderedTable.ImportRow(baseBlockTable.Rows[index]);
            }
            BaseBlockTable blockOrderTable = new BaseBlockTable(orderedTable);
            currentOrderedTable = blockOrderTable;
            currentOrderedTableIndex = orderChosenIndex;
            return blockOrderTable;
        }

        public BaseBlockTable GetOrderedBlockTable(int OrderChosenIndex) {
            BaseBlockTable orderedBlockTable = orderConfigs.Count > 0 ? 
                GetBlockOrderTableFromOrderConfigs(OrderChosenIndex, orderConfigs) : 
                GetBlockOrderTableFromPermutations(OrderChosenIndex);
            return orderedBlockTable;
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