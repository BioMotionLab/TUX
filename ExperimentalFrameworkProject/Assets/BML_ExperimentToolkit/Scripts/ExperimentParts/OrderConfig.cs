using System;
using System.Collections.Generic;
using BML_Utilities.Extensions;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    [CreateAssetMenu]
    public class OrderConfig : ScriptableObject {
        [SerializeField]
        [Header("Add indexes of block table in desired order")]
        private List<int> Order = new List<int>();

        [Tooltip("Check this to randomize block order. Still type in required indexes.")]
        public bool Randomize;
        
        public List<int> OrderedIndices {
            get {
                for (int i = 0; i < Order.Count; i++) {
                    if (Order.Contains(i)) continue;
                    UnityEditor.EditorApplication.isPlaying = false;
                    Application.Quit();
                    throw new MissingIndexException($"OrderConfig named: |{this.name}| is missing index {i}\n" +
                                                    $"The Order Config should be the order of indexes, not values. So it should start at zero\n" +
                                                    $"and end at n, where n is the number of values");
                }

                List<int> orderedIndices = Randomize ? Order.ShuffledCopy() : Order;
                return orderedIndices;
            }
            
        }

        
        public int Length => OrderedIndices.Count;

        public string AsString() {
            throw new NotImplementedException();
        }

        class MissingIndexException : Exception {
            public MissingIndexException(
                string message) : base(message) {
            }

        }
        
    }
}