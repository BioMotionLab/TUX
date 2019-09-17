using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class BaseBlockTable {

        readonly DataTable baseBlockTable;
        BaseBlockTable currentOrderedTable;
        int currentOrderedTableIndex = -1;
        
        readonly List<BlockOrderDefinition> orderConfigs;
        readonly IndependentVariables blockVariables;

        public BaseBlockTable(VariableConfigurationFile variableConfigurationFile) {
            orderConfigs = variableConfigurationFile.BlockOrderConfigurations;
            blockVariables = variableConfigurationFile.Variables.BlockVariables;
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

            //BlockOrderDefinition matters.
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
        
        public List<string> BlockPermutationsStrings {
            get {
                List<string> blockPermutations = new List<string>();
                int blockOrderIndex = 0;
                if (baseBlockTable.Rows.Count == 0) return null;
                if (baseBlockTable.Rows.Count >= 4) throw new TooManyPermutationsException();
                foreach (List<DataRow> dataRows in baseBlockTable.GetPermutations()) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Block Order #{blockOrderIndex}: ");
                    foreach (DataRow dataRow in dataRows) {
                        sb.Append($"{dataRow.AsString(separator: ", ", truncateLength: -1)} >   ");
                    }

                    blockPermutations.Add(sb.ToString());
                    blockOrderIndex++;
                }

                return blockPermutations;
            }
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
            
            BlockOrderDefinition chosenBlockOrderDefinition = orderConfigs[orderChosenIndex];

            if (chosenBlockOrderDefinition.Randomize && currentOrderedTableIndex == orderChosenIndex) {
                return currentOrderedTable;
            } 
            
            DataTable orderedTable = baseBlockTable.Clone();

            foreach (int index in chosenBlockOrderDefinition.IndexOrder) {
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

    public class TooManyPermutationsException : Exception {
    }
}