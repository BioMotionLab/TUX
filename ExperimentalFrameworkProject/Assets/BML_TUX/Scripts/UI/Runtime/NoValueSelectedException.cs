using System;

namespace BML_ExperimentToolkit.Scripts.UI.Runtime {
    public class NoValueSelectedException : Exception {

        public NoValueSelectedException() {
        }

        public NoValueSelectedException(string message) :base(message) {}
    }
}