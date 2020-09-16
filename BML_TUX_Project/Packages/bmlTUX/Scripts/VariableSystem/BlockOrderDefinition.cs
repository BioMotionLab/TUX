using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Utilities.Extensions;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {
    public class BlockOrderDefinition : ScriptableObject {

        [SerializeField]
        bool initialized = false;
        public bool Initialized => initialized;

        [SerializeField]
        public bool isValid;

        [SerializeField]
        public List<OrderRow> List = new List<OrderRow>();
    
        public bool Randomize = false;
        
        [SerializeField]
        ExperimentDesignFile2 linkedDesignFile = null;
        public bool IsValid {
            get {
                UpdateValidity();
                return isValid;
            }
        }

        void UpdateValidity() {
            if (linkedDesignFile == null) throw new NullReferenceException("no linked file");
            List<OrderRow> currentList = new List<OrderRow>();
            ExperimentDesign experimentDesign = ExperimentDesign.CreateFrom(linkedDesignFile);
            List<IndependentVariable> blockVariables = linkedDesignFile.GetVariables.BlockVariables;
            for (int rowIndex = 0; rowIndex < experimentDesign.BaseBlockTable.Rows.Count; rowIndex++) {
                DataRow row = experimentDesign.BaseBlockTable.Rows[rowIndex];
                StringBuilder sb = new StringBuilder();
                foreach (DataColumn column in experimentDesign.BaseBlockTable.Columns) {
                    if (!ColumnIsBlockVariable(column, blockVariables)) {
                        Debug.Log($"skipping column {column.ColumnName}");
                        continue;
                    }

                    sb.Append(column.ColumnName + "= " + row[column.ColumnName]);
                }

                currentList.Add(new OrderRow(rowIndex, sb.ToString()));
            }

            bool valid = currentList.SequenceEqual(List);
            isValid = valid;
        }

        bool ColumnIsBlockVariable(DataColumn column, List<IndependentVariable> blockVariables) {
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

        
        
        public void Init(ExperimentDesignFile2 designFile) {
            if (designFile == null) throw new NullReferenceException("Design file null on init");
            linkedDesignFile = designFile;
            initialized = true;
            ExperimentDesign experimentDesign = ExperimentDesign.CreateFrom(designFile);
            List<IndependentVariable> blockVariables = designFile.GetVariables.BlockVariables;
            for (int rowIndex = 0; rowIndex < experimentDesign.BaseBlockTable.Rows.Count; rowIndex++) {
                DataRow row = experimentDesign.BaseBlockTable.Rows[rowIndex];
                StringBuilder sb = new StringBuilder();
                foreach (DataColumn column in experimentDesign.BaseBlockTable.Columns) {
                    if (!ColumnIsBlockVariable(column, blockVariables)) {
                        Debug.Log($"skipping column {column.ColumnName}");
                        continue;
                    }
                    sb.Append(column.ColumnName + "= " + row[column.ColumnName]);
                }

            
                List.Add(new OrderRow(rowIndex, sb.ToString()));
            }
        }
    }
}