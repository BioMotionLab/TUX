using System.Collections;
using System.Data;
using BML_ExperimentToolkit.Scripts.Managers;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    /// <summary>
    /// One Trial of an experiment. The experiment calls Run on this trial,
    /// and it is in charge of setting and cleaning itself up
    /// </summary>
    public abstract class Trial {
        public readonly DataRow Data;

        MonoBehaviour  runner;
        public int     Index      => (int) Data[ConfigDesignFile.TrialIndexColumnName];
        public int     BlockIndex => (int) (Data[ConfigDesignFile.BlockIndexColumnName]);
        public string  TrialText  => $"Trial {Index} of Block {BlockIndex}";

        public bool CompletedSuccessfully {
            get { return (bool) Data[ConfigDesignFile.SuccessColumnName]; }
            set { Data[ConfigDesignFile.SuccessColumnName] = value; }
        }

        public int Attempts {
            get { return (int) Data[ConfigDesignFile.AttemptsColumnName]; }
            set { Data[ConfigDesignFile.AttemptsColumnName] = value; }
        }

        public bool Skipped {
            get { return (bool) Data[ConfigDesignFile.SkippedColumnName]; }
            set { Data[ConfigDesignFile.SkippedColumnName] = value; }
        }

        protected Trial(Experiment experiment, DataRow data) {
            Data = data;
            Experiment = experiment;

        }

        protected bool TrialRunning = true;
        bool           interrupt;


        // ReSharper disable once NotAccessedField.Local
        protected readonly Experiment Experiment;

        /// <summary>
        /// Run the whole trial
        /// </summary>
        /// <param name="theRunner"></param>
        /// <returns></returns>
        public IEnumerator Run(MonoBehaviour theRunner) {
            this.runner = theRunner;
            InitializeTrial();
            //Skip a frame to allow any previous things to end
            yield return null;

            runner.StartCoroutine(RunExperimentControls());

            Debug.Log($"{TrialText} Running...");
            
            IEnumerator pre = Pre();
            while (!interrupt && pre.MoveNext()) yield return pre.Current;
            
            IEnumerator main = Main();
            while (!interrupt && main.MoveNext()) yield return main.Current;
            
            IEnumerator post = Post();
            while (!interrupt && post.MoveNext()) yield return post.Current;
            
            FinalizeTrial();

            if (!interrupt) ExperimentEvents.TrialHasCompleted();
        }

        /// <summary>
        /// Initializes the trial
        /// </summary>
        void InitializeTrial() {
            CompletedSuccessfully = false;
            interrupt = false;
            TrialRunning = true;
        }

        /// <summary>
        /// Finalizes the trial
        /// </summary>
        public void FinalizeTrial() {
            TrialRunning = false;
            if (!interrupt) {
                //Debug.Log($"Finalizing {TrialText}");
                CompletedSuccessfully = true;
                Attempts++;
            }

        }

        /// <summary>
        /// Allows experimenter to control the experiments and jump between trials.
        /// </summary>
        /// <returns></returns>
        public IEnumerator RunExperimentControls() {

            while (TrialRunning) {

                //let things from last frame finish up before running
                yield return null;

                if (Input.GetKeyDown(KeyCode.Backspace)) {
                    //Debug.Log("detected skip key");
                    interrupt = true;
                    TrialRunning = false;
                    Skipped = true;
                    FinalizeTrial();
                    //Let notifications disperse through program for a frame
                    yield return null;

                    ExperimentEvents.InterruptTrial();
                }

                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                    //Debug.Log($"detected last key");
                    interrupt = true;
                    TrialRunning = false;
                    FinalizeTrial();
                    //Let notifications disperse through program for a frame
                    yield return null;

                    ExperimentEvents.GoBackOneTrial();

                }

                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                    //Debug.Log($"detected next key");
                    interrupt = true;
                    TrialRunning = false;
                    FinalizeTrial();
                    //Let notifications disperse through program for a frame
                    yield return null;


                    ExperimentEvents.SkipToNextTrial();

                }

                if (!TrialRunning) {
                    runner.StopCoroutine(this.Run(runner));
                }

            }

            //Let notifications disperse through program for a frame
            yield return null;
        }

        //Interrupts the trial while allowing the frame to complete
        public void Interrupt() {
            interrupt = true;
        }

        /// <summary>
        /// Code that runs before each trial. Overwrite this for custom behaviour.
        /// Suggest doing trial setup here.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Pre() {
            //Debug.Log($"No Pre-Trial code defined");
            yield return null;
        }

        /// <summary>
        /// Code that runs during trial. You must overwrite this.
        /// Setup should go in Pre() method
        /// Cleanup and writing to dependent variables should go in Post() method
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator Main();

        /// <summary>
        /// Code that runs after each trial. Overwrite this for custom behaviour.
        /// suggest doing trial cleanup and writing output to data here
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Post() {
            //Debug.Log($"No post trial code defined");
            yield return null;
        }

    }
}