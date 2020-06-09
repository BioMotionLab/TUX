using System;
using System.Collections.Generic;
using System.Linq;
using bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    internal class VariableNameValidator {

        public bool Valid => reasonsInvalid.Count == 0;

        public string InvalidReasons {
            get {
                string reasons = "";
                foreach (string reason in reasonsInvalid) {
                    reasons += reason + "\n";
                }

                return reasons;
            }
        }

        readonly List<string> reasonsInvalid = new List<string>();

        const string InvalidName =
            "Name Contains Illegal Characters. Name must be one word of letters or numbers only";

        const string Unnamed = "Unamed variable.";
        
        public VariableNameValidator(string nameStringValue) {
            if (!nameStringValue.All(c => Char.IsLetterOrDigit(c) || c == '_')) {
                reasonsInvalid.Add(InvalidName);
            }

            if (nameStringValue == UnnamedColumn.Name) {
                reasonsInvalid.Add(Unnamed);
            }
        }
    }
}