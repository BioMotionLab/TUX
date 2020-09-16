using System;
using bmlTUX.Scripts.Utilities;

namespace bmlTUX.Scripts.ExperimentParts {
    class BlockPermutationError : Exception {
        public BlockPermutationError(string message) : base(TuxLog.Prefix + message){
            
        }
    }
}