using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {
    
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