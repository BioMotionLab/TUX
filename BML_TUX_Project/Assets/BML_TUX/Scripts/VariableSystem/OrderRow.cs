using System;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem {
    
    [Serializable]
    public class OrderRow {

        [SerializeField]
        public int Index;
        
        [SerializeField]
        public string Text;

        public OrderRow(int index, string text) {
            Index = index;
            Text = text;
        }
    }
}