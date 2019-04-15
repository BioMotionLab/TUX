using System.Data;
using BML_ExperimentToolkit.Scripts.Managers;

// ReSharper disable MemberCanBePrivate.Global

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    /// <summary>
    /// One Trial of an Runner. The Runner calls Run on this trial,
    /// and it is in charge of setting and cleaning itself up
    /// </summary>
    public abstract class Trial : ExperimentPart {
        

        // ReSharper disable once NotAccessedField.Local
        protected readonly ExperimentRunner Runner;
        public readonly DataRow Data;

        public int     Index      => (int) Data[Runner.ConfigFile.ColumnNames.TrialIndex];
        public int     BlockIndex => (int) Data[Runner.ConfigFile.ColumnNames.BlockIndex];
        public string  TrialText  => $"Trial {Index} of Block {BlockIndex}";

        public bool CompletedSuccessfully {
            get => (bool)Data[Runner.ConfigFile.ColumnNames.Completed];
            set => Data[Runner.ConfigFile.ColumnNames.Completed] = value;
        }

        public float TrialTime {
            set => Data[Runner.ConfigFile.ColumnNames.TrialTime] = value;
        }

        public int Attempts {
            get => (int) Data[Runner.ConfigFile.ColumnNames.Attempts];
            set => Data[Runner.ConfigFile.ColumnNames.Attempts] = value;
        }

        public bool Skipped {
            get => (bool) Data[Runner.ConfigFile.ColumnNames.Skipped];
            set => Data[Runner.ConfigFile.ColumnNames.Skipped] = value;
        }

        protected Trial(ExperimentRunner runner, DataRow data) : base(runner) {
            Data = data;
            Runner = runner;
            CompletedSuccessfully = false;
            
        }

        

        /// <summary>
        /// Run the main section of trial
        /// </summary>
        /// <returns></returns>
        protected override void InternalPostMethod() {
            FinalizeTrial();
            if (!Interrupt) ExperimentEvents.TrialHasCompleted();
        }

        /// <summary>
        /// Finalizes the trial
        /// </summary>
        public void FinalizeTrial() {
            if (!Interrupt) {
                CompletedSuccessfully = true;
                Attempts++;
            }
            
            TrialTime = RunTime;
            

        }
        
        public void SkipCompletely() {
           
            Skipped = true;
            InterruptTrial();

        }

        public void InterruptTrial() {

            InterruptThis();
            FinalizeTrial();
        }

      
    }
}