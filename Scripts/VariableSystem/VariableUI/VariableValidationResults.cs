using System.Collections.Generic;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public class VariableValidationResults {
        public bool IsValid => errors.Count == 0;
        List<string> errors = new List<string>();
        List<string> warnings = new List<string>();
        public List<string> Errors => errors;
        public List<string> Warnings => warnings;

        public void AddError(string error) {
            errors.Add(error);
        }

        public void AddWarning(string warningText) {
            warnings.Add(warningText);
        }
    }
}