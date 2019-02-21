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

        public Block(DataTable trialTable, string identity, Type trialType) {
            this.trialTable = trialTable;
            this.Identity = identity;
            MakeTrials(trialType);
        }

        void MakeTrials(Type trialType) {

            Trials = new List<Trial>();

            int i = 1;
            //configure block index
            foreach (DataRow row in trialTable.Rows) {
                Trial newTrial = (Trial)Activator.CreateInstance(trialType, row);
                Trials.Add(newTrial);
                i++;
            }
        }

        public string AsString(string separator = TabSeparator, int truncate = TruncateDefault) {
            string tableString = trialTable.AsString();
            return "Identity: " + Identity + "\n" + tableString;
        }

        public virtual IEnumerator Pre() {
            Debug.Log("No pre-block code defined");
            yield return null;
        }
        public virtual IEnumerator Post() {
            Debug.Log("no post-block code defined");
            yield return null;
        }

    }

}
