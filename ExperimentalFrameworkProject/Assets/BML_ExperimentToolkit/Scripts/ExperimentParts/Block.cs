using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BML_Utilities;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    /// <summary>
    /// This class stores a block a trials in an experiment.
    /// </summary>
    public abstract class Block {

        public readonly DataTable TrialTable;
        public readonly string    Identity;

        public bool Complete = false;
        public int  Index    = -1;

        public List<Trial> Trials;
        readonly Experiment experiment;

        // ReSharper disable once PublicConstructorInAbstractClass
        public Block(Experiment experiment, 
                     DataTable trialTable, 
                     string identity, 
                     Type trialType) {
            this.experiment = experiment;
            TrialTable = trialTable;
            Identity = identity;
            MakeTrials(trialType);
        }

        /// <summary>
        /// Makes the trials for this block.
        /// </summary>
        /// <param name="trialType">Type of the trial.</param>
        void MakeTrials(Type trialType) {

            Trials = new List<Trial>();
            
            foreach (DataRow row in TrialTable.Rows) {
                Trial newTrial = (Trial)Activator.CreateInstance(trialType, experiment, row);
                Trials.Add(newTrial);
                
            }
        }

        /// <summary>
        /// String output for the block
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        public string AsString(string separator = Delimiter.Tab) {
            string tableString = TrialTable.AsString();
            return "Identity: " + Identity + "\n" + tableString;
        }

        /// <summary>
        /// This code is run before each block. Overwrite this for custom behaviour
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Pre() {
            //Debug.Log("No pre-block code defined");
            yield return null;
        }

        /// <summary>
        /// This code is run after each block. Overwrite this for custom behaviour
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Post() {
            //Debug.Log("no post-block code defined");
            yield return null;
        }

    }

}
