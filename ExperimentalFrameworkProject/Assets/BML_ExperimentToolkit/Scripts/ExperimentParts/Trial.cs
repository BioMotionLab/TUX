using System.Collections;
using System.Data;
using MyNamespace;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    /// <summary>
    /// One Trial of an experiment. The experiment calls Run on this trial,
    /// and it is in charge of setting and cleaning itself up
    /// </summary>
    public abstract class Trial {
        protected readonly DataRow data;

        public DataRow Data => data;
        MonoBehaviour  runner;
        public int     Index      => (int) data[ConfigDesignFile.TrialIndexColumnName];
        public int     BlockIndex => (int) (data[ConfigDesignFile.BlockIndexColumnName]);
        public string  TrialText  => $"Trial {Index} of Block {BlockIndex}";

        public bool CompletedSuccessfully {
            get { return (bool) data[ConfigDesignFile.SuccessColumnName]; }
            set { data[ConfigDesignFile.SuccessColumnName] = value; }
        }

        public int Attempts {
            get { return (int) data[ConfigDesignFile.AttemptsColumnName]; }
            set { data[ConfigDesignFile.AttemptsColumnName] = value; }
        }

        public bool Skipped {
            get { return (bool) data[ConfigDesignFile.SkippedColumnName]; }
            set { data[ConfigDesignFile.SkippedColumnName] = value; }
        }

        protected Trial(Experiment experiment, DataRow data) {
            this.data = data;
            this.experiment = experiment;

        }

        protected bool trialRunning = true;
        bool           interrupt    = false;
        Experiment experiment;

        public IEnumerator Run(MonoBehaviour theRunner) {
            this.runner = theRunner;
            InitializeTrial();
            //Skip a frame to allow any previous things to end
            yield return null;

            runner.StartCoroutine(RunExperimentControls());


            Debug.Log($"{TrialText} Running...");



            Debug.Log($"{TrialText} Running pre trial");
            IEnumerator pre = Pre();
            while (!interrupt && pre.MoveNext()) {
                yield return pre.Current;

            }

            Debug.Log("Done running pre trial");


            Debug.Log($"{TrialText} Running main trial");
            IEnumerator main = Main();
            while (!interrupt && main.MoveNext()) {
                yield return main.Current;

            }

            Debug.Log("Done running main");


            Debug.Log($"{TrialText} Running post trial");
            IEnumerator post = Post();
            while (!interrupt && post.MoveNext()) {
                yield return post.Current;

            }

            Debug.Log("Done running post trial");


            FinalizeTrial();

            if (!interrupt) {
                ExperimentEvents.TrialHasCompleted();
            }

        }

        void InitializeTrial() {
            CompletedSuccessfully = false;
            interrupt = false;
            trialRunning = true;
        }

        public void FinalizeTrial() {
            trialRunning = false;
            if (!interrupt) {
                Debug.Log($"Finalizing {TrialText}");
                CompletedSuccessfully = true;
                Attempts++;
            }

        }

        public IEnumerator RunExperimentControls() {

            while (trialRunning) {

                //let things from last frame finish up
                yield return null;

                if (Input.GetKeyDown(KeyCode.Backspace)) {
                    Debug.Log($"detected skip key");
                    interrupt = true;
                    trialRunning = false;
                    Skipped = true;
                    Debug.Log("Finalizing from within controls");
                    FinalizeTrial();
                    //Let notifications disperse through program for a frame
                    yield return null;

                    ExperimentEvents.InterruptTrial();
                }

                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                    Debug.Log($"detected last key");
                    interrupt = true;
                    trialRunning = false;
                    Debug.Log("Finalizing from within controls");
                    FinalizeTrial();
                    //Let notifications disperse through program for a frame
                    yield return null;

                    ExperimentEvents.GoBackOneTrial();

                }

                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                    Debug.Log($"detected next key");
                    interrupt = true;
                    trialRunning = false;
                    Debug.Log("Finalizing from within controls");
                    FinalizeTrial();
                    //Let notifications disperse through program for a frame
                    yield return null;


                    ExperimentEvents.SkipToNextTrial();

                }

                if (!trialRunning) {
                    runner.StopCoroutine(this.Run(runner));
                }

            }

            //Let notifications disperse through program for a frame
            yield return null;
        }


        public void Interrupt() {
            interrupt = true;
        }

        protected virtual IEnumerator Pre() {
            Debug.Log($"No Pre-Trial code defined");
            yield return null;
        }

        protected abstract IEnumerator Main();

        protected virtual IEnumerator Post() {
            Debug.Log($"No post trial code defined");
            yield return null;
        }

    }
}