using System;

namespace bmlTUX {
    class BlockPermutationError : Exception {
        public BlockPermutationError(string message) : base(TuxLog.Prefix + message){
            
        }
    }
}