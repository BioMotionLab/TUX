using System.Collections;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;

namespace BML_ExperimentToolkit.Data {


    public class Simple : Trial {
        
        
        // You usually want to store a reference to your experiment runner
//        MyExperimentRunner myRunner;
        
        
        public Simple(ExperimentRunner runner, DataRow data) : base(runner, data) {
            // Good place to set up references to objects in the ExperimentRunner
            
            // For example:
//             myRunner = (MyExperimentRunner)runner;
//             GameObject myGameObject = myRunner.MyGameObject;
            
        }


        protected override void PreMethod() {
            // setup code (happens all in one frame)
            
            // Read independent variables like this:
//             float thisTrialsDistanceValue = (float)Data["MyDistanceFloatVariableName"];
        }

        protected override IEnumerator PreCoroutine() {
            // setup code (can happen over multiple frames)
            yield return null;
        }

        protected override IEnumerator RunMainCoroutine() {
            // Main code to run your trials (can happen over many frames)
            
//            // You usually want to do a while-loop to wait for participant response:
//            bool waitingForParticipantResponse = true;
//            while (waitingForParticipantResponse) {
//                
//                // will keep running each frame until you set waitingForParticipantResponse to false.
//                yield return null;
//            }
            
            
            yield return null;
        }

        
        protected override IEnumerator PostCoroutine() {
            // cleanup code (can happen over multiple frames)
            yield return null;
        }

        protected override void PostMethod() {
            // cleanup code (happens all in one frame)
            
            // Good place to write results to dependent variables. 
//             Data["MyDependentFloatVariable"] = someFloatVariable;
        }
    }
}