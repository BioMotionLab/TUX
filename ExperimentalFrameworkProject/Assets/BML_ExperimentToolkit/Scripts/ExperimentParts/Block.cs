using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_Utilities.Extensions;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    /// <summary>
    /// This class stores a block a trials in an Runner.
    /// </summary>
    public abstract class Block : ExperimentPart {

        public DataTable TrialTable;
        public readonly string    Identity;

        public bool Complete = false;
        ExperimentRunner runner;
        public List<Trial> Trials;

        readonly DataRow data;

        protected DataRow Data {
            get {
                if (data == null) {
                    throw
                        new NullReferenceException("Trying to access Block data in experiment with no blocks defined");
                }

                return data;
            }
        }

        protected Block(ExperimentRunner runner,
                     DataTable trialTable, 
                     string identity, 
                     Type trialType,
                     DataRow dataRow) 
                        : base(runner) {
            this.runner = runner;
            TrialTable = trialTable;
            Identity = identity;
            MakeTrials(trialType);
            data = dataRow;
        }

        /// <summary>
        /// Makes the trials for this block.
        /// </summary>
        /// <param name="trialType">Type of the trial.</param>
        void MakeTrials(Type trialType) {

            Trials = new List<Trial>();
            
            foreach (DataRow row in TrialTable.Rows) {
                Trial newTrial = (Trial)Activator.CreateInstance(trialType, runner, row);
                Trials.Add(newTrial);
                
            }
        }

        protected override IEnumerator RunMainCoroutine() {

            TrialSequenceRunner trialSequenceRunner = new TrialSequenceRunner(Trials);
            trialSequenceRunner.Start();
            while (trialSequenceRunner.Running) {
                yield return null;
            }
            
        }

        /// <summary>
        /// String output for the block
        /// </summary>
        /// <returns></returns>
        public string AsString() {
            string tableString = TrialTable.AsString();
            return "Identity: " + Identity + "\n" + tableString;
        }

    }

}
