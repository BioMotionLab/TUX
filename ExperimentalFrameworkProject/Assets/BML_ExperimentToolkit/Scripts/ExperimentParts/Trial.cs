using System.Data;
using BML_ExperimentToolkit.Scripts.Managers;
using JetBrains.Annotations;


namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    /// <summary>
    /// One Trial of an Runner. The Runner calls Run on this trial,
    /// and it is in charge of setting and cleaning itself up
    /// </summary>
    public abstract class Trial : ExperimentPart {
        
        /// <summary>
        /// Stores the values of all variables for this particular trial.
        /// For example:
        /// bool boolValueForThisTrial = (bool)Data["BoolVariable1"];
        /// </summary>
        public readonly DataRow Data;

        /// <summary>
        /// The index of the Trial within a Block
        /// </summary>
        [PublicAPI] public int Index => (int) Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.TrialIndex];
        
        /// <summary>
        /// The index of the Block in which this Trial resides
        /// </summary>
        [PublicAPI] public int BlockIndex => (int) Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.BlockIndex];
        
        /// <summary>
        /// Text that describes the index of Trial and Block.
        /// </summary>
        [PublicAPI] public string  TrialText => $"Trial {Index} of Block {BlockIndex}";

        /// <summary>
        /// Whether the trial was Completed Successfully
        /// </summary>
        [PublicAPI] public bool CompletedSuccessfully {
            get => (bool)Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.Completed];
            set => Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.Completed] = value;
        }

        /// <summary>
        /// The time it took the MainCoroutine to complete.
        /// Not accurate enough for high-precision reaction times
        /// </summary>
        [PublicAPI] public float TrialTime {
            set => Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.TrialTime] = value;
        }

        /// <summary>
        /// How many times the trial was attempted
        /// </summary>
        [PublicAPI] public int Attempts {
            get => (int) Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.Attempts];
            set => Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.Attempts] = value;
        }

        /// <summary>
        /// Whether the trial was skipped over manually
        /// </summary>
        [PublicAPI] public bool Skipped {
            get => (bool) Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.Skipped];
            set => Data[Runner.VariableConfigurationFileFile.ColumnNamesSettings.Skipped] = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="data"></param>
        protected Trial(ExperimentRunner runner, DataRow data) : base(runner) {
            Data = data;
            CompletedSuccessfully = false;
        }

        
        /// <summary>
        /// Run the main section of trial
        /// </summary>
        /// <returns></returns>
        protected sealed override void InternalPostMethod() {
            FinalizeTrial();
            if (!Interrupt) ExperimentEvents.TrialHasCompleted();
        }

        /// <summary>
        /// Finalizes the trial
        /// </summary>
        void FinalizeTrial() {
            if (!Interrupt) {
                CompletedSuccessfully = true;
                Attempts++;
            }
            TrialTime = RunTime;
        }
        
        /// <summary>
        /// Skips this Trial manually so that it is not returned to at a later time
        /// </summary>
        public void SkipCompletely() {
            Skipped = true;
            InterruptTrial();
        }

        /// <summary>
        /// Interrupts this Trial, but allows for returning to it.
        /// </summary>
        public void InterruptTrial() {
            InterruptThis();
            FinalizeTrial();
        }

      
    }
}