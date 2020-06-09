using System;

namespace bmlTUX.Scripts.UI.RuntimeUI.UIUtilities {
    public class NoValueSelectedException : Exception {

        public NoValueSelectedException() {
        }

        public NoValueSelectedException(string message) :base(message) {}
    }
}