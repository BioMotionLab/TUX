using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {
    
    [Serializable]
    public struct OrderRow : IEquatable<OrderRow> {

        [SerializeField]
        public int Index;
        
        [SerializeField]
        public string Text;

        public OrderRow(int index, string text) {
            Index = index;
            Text = text;
        }


        public bool Equals(OrderRow other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Index == other.Index && Text == other.Text;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OrderRow) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Index * 397) ^ (Text != null ? Text.GetHashCode() : 0);
            }
        }
    }
}