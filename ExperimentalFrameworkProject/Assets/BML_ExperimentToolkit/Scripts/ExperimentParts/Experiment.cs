using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_Utilities;
using BML_Utilities.Extensions;
using UnityEngine;

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

        readonly ExperimentDesign design;
        readonly ExperimentRunner runner;

        protected Experiment(ExperimentRunner runner, ExperimentDesign design) : base(runner) {
            this.runner = runner;
            if (design == null) {
                throw new NullReferenceException("Experiment created with null design");
            }
            this.design = design;
            Enable();
        }

        /// <summary>
        /// Called when Runner is loaded
        /// </summary>
        void Enable() {
            ExperimentEvents.OnTrialUpdated += TrialUpdated;
        }

        /// <summary>
        /// Called when Runner is over
        /// </summary>
        public void Disable() {
            ExperimentEvents.OnTrialUpdated -= TrialUpdated;
        }

        /// <summary>
        /// Called when a trial gets updated
        /// </summary>
        /// <param name="trials"></param>
        /// <param name="index"></param>
        void TrialUpdated(List<Trial> trials, int index) {
            OutputUpdated();
        }

        /// <summary>
        /// Updates the Runner's output
        /// </summary>
        void OutputUpdated() {
            ExperimentEvents.OutputUpdated(this);
        }

        protected override IEnumerator RunMainCoroutine() {
            

            BlockSequenceRunner blockRunner = new BlockSequenceRunner(runner, design.Blocks);
            blockRunner.Start();
            yield return null;
        }


        /// <summary>Gets the Runner as string.</summary>
        /// <value>The Runner as a string.</value>
        public string AsString {
            get {
                StringBuilder sb = new StringBuilder();

                string header = design.TrialTableHeader;
                sb.AppendLine(header);
                foreach (Block block in design.Blocks) {
                    foreach (Trial trial in block.Trials) {
                        sb.AppendLine(trial.Data.AsString(separator: Delimiter.Comma, truncate: -1));
                    }
                }

                return sb.ToString();

            }
        }
    }

}