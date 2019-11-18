using System.Data;
using bmlTUX.Scripts.ExperimentParts;
using UnityEngine;

namespace bmlTUX.SamplesAndTutorials.TutorialExperiment {
    public class TutorialBlockScript : Block
    {
    
        public TutorialBlockScript(ExperimentRunner runner, DataTable trialTable, DataRow dataRow) : base(runner, trialTable, dataRow) {
        }

        protected override void PreMethod() {
            //Get reference to custom ExperimentRunner
            TutorialExperimentRunner tutorialRunner = (TutorialExperimentRunner) Runner;

            //Get value of distance for this block.
            float distance = (float) Data["Distance"];

            //get position
            Vector3 position = tutorialRunner.Stimulus.transform.localPosition;

            //set z of position to the distance value.
            position.z = distance;

            //update stimulus position to new position
            tutorialRunner.Stimulus.transform.localPosition = position;

        }

   
    }
}
