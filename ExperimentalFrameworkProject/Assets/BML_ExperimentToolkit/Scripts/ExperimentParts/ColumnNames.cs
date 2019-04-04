using UnityEngine;


namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    [CreateAssetMenu(menuName = "BML/Create ColumnNames Config File")]
    public class ColumnNames : ScriptableObject{
        public string TotalTrialIndex = "Trial";
        public string BlockIndex      = "Block";
        public string Skipped         = "Skipped";
        public string Attempts        = "Attempts";
        public string TrialIndex      = "TrialInBlock";
        public string Completed       = "Completed";
    }
}