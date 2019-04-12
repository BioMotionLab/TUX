using System.Data;
using BML_ExperimentToolkit.Scripts.Managers;

// ReSharper disable MemberCanBePrivate.Global

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    /// <summary>
    /// One Trial of an Runner. The Runner calls Run on this trial,
    /// and it is in charge of setting and cleaning itself up
    /// </summary>
    public abstract class Trial : ExperimentPart {
        bool           interrupt;

        // ReSharper disable once NotAccessedField.Local
        protected readonly ExperimentRunner Runner;
        public readonly DataRow Data;

        public int     Index      => (int) Data[Runner.ConfigDesignFile.ColumnNames.TrialIndex];
        public int     BlockIndex => (int) (Data[Runner.ConfigDesignFile.ColumnNames.BlockIndex]);
        public string  TrialText  => $"Trial {Index} of Block {BlockIndex}";

        public bool CompletedSuccessfully {
            get => (bool)Data[Runner.ConfigDesignFile.ColumnNames.Completed];
            set => Data[Runner.ConfigDesignFile.ColumnNames.Completed] = value;
        }

        public float TrialTime {
            set => Data[Runner.ConfigDesignFile.ColumnNames.TrialTime] = value;
        }

        public int Attempts {
            get => (int) Data[Runner.ConfigDesignFile.ColumnNames.Attempts];
            set => Data[Runner.ConfigDesignFile.ColumnNames.Attempts] = value;
        }

        public bool Skipped {
            get => (bool) Data[Runner.ConfigDesignFile.ColumnNames.Skipped];
            set => Data[Runner.ConfigDesignFile.ColumnNames.Skipped] = value;
        }

        protected Trial(ExperimentRunner runner, DataRow data) : base(runner) {
            Data = data;
            Runner = runner;
            CompletedSuccessfully = false;
            interrupt = false;
            Enable();
        }

        void Enable() {
            ExperimentEvents.OnTrialInterrupt += SkipTrial;
            ExperimentEvents.OnGoBackOneTrial += InterruptTrial;
            ExperimentEvents.OnSkipToNextTrial += InterruptTrial;

        }

        void Disable() {
            ExperimentEvents.OnTrialInterrupt -= SkipTrial;
            ExperimentEvents.OnGoBackOneTrial -= InterruptTrial;
            ExperimentEvents.OnSkipToNextTrial -= InterruptTrial;

        }

        /// <summary>
        /// Run the main section of trial
        /// </summary>
        /// <returns></returns>
        protected override void InternalPostMethod() {
            FinalizeTrial();
            if (!interrupt) ExperimentEvents.TrialHasCompleted();
        }

        /// <summary>
        /// Finalizes the trial
        /// </summary>
        public void FinalizeTrial() {
            if (!interrupt) {
                CompletedSuccessfully = true;
                Attempts++;
            }
            
            TrialTime = RunTime;
            Disable();
        }
        
        public void SkipTrial() {
            Skipped = true;
            InterruptTrial();
        }

        public void InterruptTrial() {
            interrupt = true;
            FinalizeTrial();
        }

      
    }
}