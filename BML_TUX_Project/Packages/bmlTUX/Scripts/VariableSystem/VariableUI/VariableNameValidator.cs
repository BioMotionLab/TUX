using System;
using System.Linq;
using bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public static class VariableNameValidator {

        const string InvalidName =
            "Name Contains Illegal Characters. Name must be one word of letters or numbers only";

        const string Unnamed = "Unamed variable.";
        
        public static void Validate(string nameStringValue, VariableValidationResults resultContainer) {

            if (!nameStringValue.All(c => Char.IsLetterOrDigit(c) || c == '_')) {
                resultContainer.AddError(InvalidName);
            }

            if (nameStringValue == UnnamedColumn.Name) {
                resultContainer.AddError(Unnamed);
            }
            
        }
        
    }
}