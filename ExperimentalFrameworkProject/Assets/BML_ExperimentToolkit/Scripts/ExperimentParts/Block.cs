using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    /// <summary>
    /// This class stores a block a trials in an experiment.
    /// </summary>
    public abstract class Block {
        private const string TabSeparator    = "\t";
        private const int    TruncateDefault = 10;

        public DataTable trialTable;
        public string    Identity;

        public bool Complete = false;
        public int  Index    = -1;

        public List<Trial> Trials;
        Experiment experiment;

        public Block(Experiment experiment, 
                     DataTable trialTable, 
                     string identity, 
                     Type trialType) {
            this.experiment = experiment;
            this.trialTable = trialTable;
            this.Identity = identity;
            MakeTrials(trialType);
        }

        /// <summary>
        /// Makes the trials for this block.
        /// </summary>
        /// <param name="trialType">Type of the trial.</param>
        void MakeTrials(Type trialType) {

            Trials = new List<Trial>();

            int i = 1;
            //configure block index
            foreach (DataRow row in trialTable.Rows) {
                Trial newTrial = (Trial)Activator.CreateInstance(trialType, experiment, row);
                Trials.Add(newTrial);
                i++;
            }
        }

        /// <summary>
        /// String output for the block
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="TruncateToNum">The number of characters to leave.</param>
        /// <returns></returns>
        public string AsString(string separator = TabSeparator, int TruncateToNum = TruncateDefault) {
            string tableString = trialTable.AsString();
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
