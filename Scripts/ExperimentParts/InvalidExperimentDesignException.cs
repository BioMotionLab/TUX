﻿using System;

namespace bmlTUX {
    
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