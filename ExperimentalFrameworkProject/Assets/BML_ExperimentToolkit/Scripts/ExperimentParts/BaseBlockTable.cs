using System;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;

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

        BaseBlockTable GetBlockOrderTableFromOrderConfigs(int orderChosenIndex) {
 
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

        public BaseBlockTable GetOrderedBlockTable(int orderChosenIndex) {
            BaseBlockTable orderedBlockTable = orderConfigs.Count > 0 ? 
                GetBlockOrderTableFromOrderConfigs(orderChosenIndex) : 
                GetBlockOrderTableFromPermutations(orderChosenIndex);
            return orderedBlockTable;
        }

    }

}