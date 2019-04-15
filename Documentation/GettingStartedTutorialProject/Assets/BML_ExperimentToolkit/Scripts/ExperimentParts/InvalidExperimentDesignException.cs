using System;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public partial class ExperimentDesign {
        public class InvalidExperimentDesignException : Exception {
            public InvalidExperimentDesignException() {
            }

            public InvalidExperimentDesignException(string message) : base(message) {
            }

            public InvalidExperimentDesignException(string message, Exception inner)
                : base(message, inner) {
            }
        }
    }


}