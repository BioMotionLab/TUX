using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using bmlTUX;
using bmlTUX.Extensions;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.VariableSystem;
using UnityEngine;

namespace VariableSystem {
    public class BlockOrderDefinition : ScriptableObject {

        [SerializeField] public bool isValid;
        
        [SerializeField] public List<OrderRow> rawList = new List<OrderRow>();

        [SerializeField] public List<OrderRow> List = new List<OrderRow>();

        [SerializeField] public ExperimentDesignFile2 linkedDesignFile = default;
        
        
        public bool Randomize = false;
        
        public bool IsValid {
            get {
                UpdateValidity();
                return isValid;
            }
        }

        void UpdateValidity() {
            if (linkedDesignFile == null) throw new NullReferenceException("Block Order File has no linked file. Please report a bug");
            
            List<OrderRow> designFileCurrentList = GetOrderRows(linkedDesignFile);
            
            isValid = designFileCurrentList.SequenceEqual(rawList);
        }

        public void Init(ExperimentDesignFile2 designFile) {
            if (designFile == null) throw new NullReferenceException("Design file null on init");
            linkedDesignFile = designFile;

            rawList = GetOrderRows(designFile);
            OrderRow[] orderArray = new OrderRow[rawList.Count];
            rawList.CopyTo(orderArray);
            List = orderArray.ToList();
            
            UpdateValidity();
        }

        static List<OrderRow> GetOrderRows(ExperimentDesignFile2 designFile) {
            List<OrderRow> currentList = new List<OrderRow>();

            ExperimentDesign experimentDesign = ExperimentDesign.CreateFrom(designFile);
            List<IndependentVariable> blockVariables = designFile.GetVariables.BlockVariables;
            for (int rowIndex = 0; rowIndex < experimentDesign.BaseBlockTable.Rows.Count; rowIndex++) {
                DataRow row = experimentDesign.BaseBlockTable.Rows[rowIndex];
                List<string> values = new List<string>();
                foreach (DataColumn column in experimentDesign.BaseBlockTable.Columns) {
                    if (!ColumnIsBlockVariable(column, blockVariables)) {
                        Debug.Log($"skipping column {column.ColumnName}");
                        continue;
                    }

                    values.Add(column.ColumnName + "= " + row[column.ColumnName]);
                }


                currentList.Add(new OrderRow(rowIndex, string.Join(", ", values)));
            }

            return currentList;
        }

        static bool ColumnIsBlockVariable(DataColumn column, List<IndependentVariable> blockVariables) {
            foreach (IndependentVariable variable in blockVariables) {
                if (column.ColumnName == variable.Name) return true;
            }
            return false;
        }

        public int[] IndexOrder {
            get {
                List<int> order = new List<int>();
            
                if (Randomize) {
                    for (int i = 0; i < List.Count; i++) {
                        order.Add(i);
                    }
                    order.Shuffle();
                }
                else {
                    foreach (OrderRow row in List) {
                        order.Add(row.Index);
                    }
                }
                return order.ToArray();
            }
        }
    }
}