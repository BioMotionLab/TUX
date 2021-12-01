using System;
using bmlTUX.Scripts.Utilities;

namespace bmlTUX {
    class BlockPermutationError : Exception {
        public BlockPermutationError(string message) : base(TuxLog.Prefix + message){
            
        }
    }
}