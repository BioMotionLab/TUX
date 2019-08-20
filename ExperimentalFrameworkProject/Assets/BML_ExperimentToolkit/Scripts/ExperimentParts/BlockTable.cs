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

    public class BlockTable {

        readonly DataTable baseBlockTable;
        ExperimentDesign design;
        BlockTable currentOrderedTable;
        int currentOrderedTableIndex = -1;
          
        public const int MaxBlockPermutationsAllowed = 3;
        readonly List<OrderConfig> orderConfigs;
        
        public BlockTable(ExperimentDesign design, VariableConfig variableConfig) {
            orderConfigs = variableConfig.OrderConfigs;
            this.design = design;
            //Get block Variables
            List<Variable> blockVariables = new List<Variable>();
            foreach (Variable datum in variableConfig.AllVariables) {
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

        public List<string> GetBlockPermutationsStrings() {
            
            if (baseBlockTable.Rows.Count <= MaxBlockPermutationsAllowed && orderConfigs.Count == 0) {
                return BlockPermutationsStrings;
            }

            if (orderConfigs.Count > 0) return GetBlockOrderConfigStrings();
            
            throw new NullReferenceException("There are too many block values to create a permutation table. " +
                                             "Block orders must be defined manually using OrderConfig files. " +
                                             "See documentation for more information");

        }

        List<string> GetBlockOrderConfigStrings() {
            List<string> orderStrings = new List<string>();
            foreach (OrderConfig orderConfig in orderConfigs) {
                
                if (orderConfig.Length != baseBlockTable.Rows.Count) {
                    throw new ArgumentException($"OrderConfig file does not match length. See Below:\n" +
                                                $"Need to adjust length of orders.\n" +
                                                $"{orderConfig.name} should be {baseBlockTable.Rows.Count} long. But is {orderConfig.Length}\n\n" +
                                                $"Base Table:" +
                                                $"{baseBlockTable.AsString()}");
                    
                }

                if (orderConfig.Randomize) {
                    orderStrings.Add("Randomize");
                }
                else {
                    string orderString = "";
                    foreach (int orderIndex in orderConfig.OrderedIndices) {
                        string rowString = baseBlockTable.Rows[orderIndex].AsString(separator: ", ", truncateLength: -1);
                        orderString += rowString + " > ";
                    }
                    orderStrings.Add(orderString); 
                }
                
            }

            return orderStrings;
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


        BlockTable GetBlockOrderTableFromPermutations(int index) {

            DataTable orderedTable = baseBlockTable.Clone();

            if (!HasBlocks) return null;

            foreach (DataRow dataRow in baseBlockTable.GetPermutations()[index]) {
                orderedTable.ImportRow(dataRow);
            }

            BlockTable blockOrderTable = new BlockTable(orderedTable);
            return blockOrderTable;
        }

        BlockTable GetBlockOrderTableFromOrderConfigs(int orderChosenIndex, List<OrderConfig> orderConfigs) {
 
            if (orderChosenIndex > orderConfigs.Count- 1) throw new IndexOutOfRangeException($"Index chosen is {orderChosenIndex}, but count is {orderConfigs.Count}");
            
            OrderConfig chosenOrder = orderConfigs[orderChosenIndex];

            if (chosenOrder.Randomize && currentOrderedTableIndex == orderChosenIndex) {
                return currentOrderedTable;
            } 
            
            DataTable orderedTable = baseBlockTable.Clone();

            foreach (int index in chosenOrder.OrderedIndices) {
                orderedTable.ImportRow(baseBlockTable.Rows[index]);
            }
            BlockTable blockOrderTable = new BlockTable(orderedTable);
            currentOrderedTable = blockOrderTable;
            currentOrderedTableIndex = orderChosenIndex;
            return blockOrderTable;
        }

        public BlockTable GetOrderedBlockTable(int OrderChosenIndex) {
            BlockTable orderedBlockTable;
            if (Rows.Count <= MaxBlockPermutationsAllowed) {
                orderedBlockTable = GetBlockOrderTableFromPermutations(OrderChosenIndex);
            }
            else {
                orderedBlockTable = GetBlockOrderTableFromOrderConfigs(OrderChosenIndex, orderConfigs);
            }
                
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