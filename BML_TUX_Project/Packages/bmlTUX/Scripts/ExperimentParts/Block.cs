using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace bmlTUX {

    /// <summary>
    /// This class stores a block a trials in an Runner.
    /// </summary>
    public abstract class Block : ExperimentPart {

        /// <summary>
        /// The table of trials for this block
        /// </summary>
        public readonly DataTable TrialTable;
        
        /// <summary>
        /// Text describing the block
        /// TODO need to fix identity
        /// </summary>
        public readonly string    Identity;

        public readonly int index;
        
        /// <summary>
        /// Whether the block is complete
        /// </summary>
        public bool Complete = false;
        
        /// <summary>
        /// List of trials in this block
        /// </summary>
        public List<Trial> Trials;

        
        readonly DataRow data;

        /// <summary>
        /// The values of variables for this block.
        /// For Example: bool blockVariableBoolValueForThisBlock = (bool)Data["BoolBlockVariable1"];
        /// Note that the block does not have access to trial-specific variables nor their values.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        [PublicAPI]
        protected DataRow Data {
            get {
                if (data == null) 
                    throw new NullReferenceException("Trying to access Block data in experiment with no blocks defined");
                return data;
            }
        }

        /// <summary>
        /// Constructor. Just auto implement the base constructor and everything should work.
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="trialTable"></param>
        /// <param name="dataRow"></param>
        protected Block(ExperimentRunner runner,
                     DataTable trialTable,
                     DataRow dataRow,
                     int index) 
                        : base(runner) {
            TrialTable = trialTable;
            Identity = dataRow.AsStringWithColumnNames();
            MakeTrials();
            data = dataRow;
            this.index = index;
        }
        
        /// <summary>
        /// This is obsolete. Please use the constructor with an index
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="trialTable"></param>
        /// <param name="dataRow"></param>
        [Obsolete]
        protected Block(ExperimentRunner runner,
                        DataTable        trialTable,
                        DataRow          dataRow) 
            : base(runner) {
            TrialTable = trialTable;
            Identity = dataRow.AsStringWithColumnNames();
            MakeTrials();
            data = dataRow;
            this.index = -99;
        }
        
        

        /// <summary>
        /// Makes the trials for this block.
        /// </summary>
        void MakeTrials() {
            Trials = new List<Trial>();
            foreach (DataRow row in TrialTable.Rows) {
                Trial newTrial = (Trial)Activator.CreateInstance(Runner.TrialType, Runner, row);
                Trials.Add(newTrial);
                
            }
        }
        
        protected sealed override IEnumerator RunMainCoroutine() {
            TrialSequenceRunner trialSequenceRunner = new TrialSequenceRunner(Trials);
            trialSequenceRunner.Start();
            while (trialSequenceRunner.Running) {
                yield return null;
            }
        }

        protected override void InternalPostMethod() {
            ExperimentEvents.BlockCompleted(this);
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
