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
        /// Code that runs before each block. Overwrite this for custom behaviour.
        /// Suggest doing block setup here.
        /// Useful for more complex cleanup tasks/instructions that need to
        /// run for more than one frame.
        /// Must contain at least one "yield return" statement.
        /// [Note: Called after PreMethod()]
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator PreCoroutine() {
            yield return null;
        }

        /// <summary>
        /// Code that runs before each block. Overwrite this for custom behaviour.
        /// Suggest doing trial setup here.
        /// Useful for simple setup tasks that can be completed in a single frame.
        /// [Note: Called before PreCoroutine()]
        /// </summary>
        public virtual void PreMethod() { }

        

        /// <summary>
        /// Code that runs after each blcok. Overwrite this for custom behaviour.
        /// suggest doing trial cleanup and writing output to data here.
        /// Useful for more complex cleanup tasks/instructions that need to
        /// run for more than one frame.
        /// Must contain at least one "yield return" statement.
        /// [Note: Called before PostMethod()]
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator PostCoroutine() {
            //Debug.Log($"No post trial code defined");
            yield return null;
        }

        /// <summary>
        /// Code that runs after each block. Overwrite this for custom behaviour.
        /// suggest doing trial cleanup and writing output to data here.
        /// Useful for simple setup tasks that can occur in a single frame.
        /// [Note: Called after PostCoroutine()]
        /// </summary>
        public virtual void PostMethod() { }

    }

}
