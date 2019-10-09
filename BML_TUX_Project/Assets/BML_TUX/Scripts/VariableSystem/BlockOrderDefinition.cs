using System.Collections.Generic;
using System.Data;
using System.Text;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_Utilities;
using BML_Utilities.Extensions;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [CreateAssetMenu()]
    public class BlockOrderDefinition : ScriptableObject {
        [SerializeField]
        public List<OrderRow> List = new List<OrderRow>();
    
        public bool Randomize = false;

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

        public void InitFromDesign(ExperimentDesignFile experimentDesignFile) {
        
            ExperimentDesign experimentDesign = ExperimentDesign.CreateFrom(experimentDesignFile);
            for (int rowIndex = 0; rowIndex < experimentDesign.BaseBlockTable.Rows.Count; rowIndex++) {
                DataRow row = experimentDesign.BaseBlockTable.Rows[rowIndex];
                StringBuilder sb = new StringBuilder();
                foreach (DataColumn column in experimentDesign.BaseBlockTable.Columns) {
                    sb.Append(column.ColumnName + "= " + row[column.ColumnName]);
                }

            
                List.Add(new OrderRow(rowIndex, sb.ToString()));
            }
        
        }
    }
}