using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_Utilities;
using BML_Utilities.Extensions;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBePrivate.Global

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    /// <summary>
    /// The Runner class is the main backbone of the toolkit. This sets up the Runner design, the blocks and trial structure, and manages output
    /// </summary>
    /// <seealso cref="T:UnityEngine.MonoBehaviour" />
    /// <seealso cref="T:BML_ExperimentToolkit.Scripts.Managers.Outputtable" />
    // ReSharper disable once InheritdocConsiderUsage
    public abstract class Experiment : ExperimentPart, Outputtable {

        readonly RunnableDesign design;
        readonly ExperimentRunner runner;

        /// <summary>
        /// Constructor. Just call auto implementation and everything should work
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="design"></param>
        protected Experiment(ExperimentRunner runner, RunnableDesign design) : base(runner) {
            this.runner = runner;
            this.design = design ?? throw new NullReferenceException("Experiment created with null design");
            
            ExperimentEvents.OnTrialUpdated += TrialUpdated;
        }
      

        /// <summary>
        /// Called when Runner is over
        /// </summary>
        public void Disable() {
            ExperimentEvents.OnTrialUpdated -= TrialUpdated;
        }
        
        void TrialUpdated(List<Trial> trials, int index) {
            ExperimentEvents.OutputUpdated(this);
        }
        

        protected sealed override IEnumerator RunMainCoroutine() {
            BlockSequenceRunner blockRunner = new BlockSequenceRunner(runner, design.Blocks);
            blockRunner.Start();
            while (blockRunner.Running) {
                yield return null;
            }
        }

        /// <summary>
        /// Gets the Runner as string.
        /// </summary>
        public string AsString {
            get {
                StringBuilder sb = new StringBuilder();

                string header = design.TrialTableHeader;
                sb.AppendLine(header);
                foreach (Block block in design.Blocks) {
                    foreach (Trial trial in block.Trials) {
                        sb.AppendLine(trial.Data.AsString(separator: Delimiter.Comma, truncateLength: -1));
                    }
                }

                return sb.ToString();

            }
        }
    }

}