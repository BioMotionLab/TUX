using System;

namespace BML_TUX.Scripts.UI.Runtime {
    public class NoValueSelectedException : Exception {

        public NoValueSelectedException() {
        }

        public NoValueSelectedException(string message) :base(message) {}
    }
}