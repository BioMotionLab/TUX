using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class ExperimentConfig : ScriptableObject {
        public bool ShuffleTrialOrder;
        public int  NumberOfTimesToRepeatTrials = 1;



        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public const string TotalTrialIndexColumnName = "TrialNum";
        public const string BlockIndexColumnName      = "Block";
        public const string SkippedColumnName         = "Skipped";
        public const string AttemptsColumnName        = "Attempts";
        public const string TrialIndexColumnName      = "TrialInBlock";
        public const string SuccessColumnName         = "Completed";

        public void PrintTrials() {
            throw new System.NotImplementedException();
        }
    }
}