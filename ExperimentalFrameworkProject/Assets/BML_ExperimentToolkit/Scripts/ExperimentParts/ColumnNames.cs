using UnityEngine;
using MenuNames = BML_Utilities.MenuNames;


namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + 
                                "Create Column Names Config")]
    public class ColumnNames : ScriptableObject{
        public string TotalTrialIndex = "Trial";
        public string BlockIndex      = "Block";
        public string Skipped         = "Skipped";
        public string Attempts        = "Attempts";
        public string TrialIndex      = "TrialInBlock";
        public string Completed       = "Completed";
        public string TrialTime = "TrialTime";

        [Space]
        public int DefaultMissingValue = -999;
    }
}