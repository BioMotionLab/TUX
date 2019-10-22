using System.Collections;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;

namespace BML_TUX.Data.ScriptingTemplates {
    /// <summary>
    /// Classes that inherit from Block define custom behaviour for your experiment's blocks.
    /// This might be useful for instructions that differ between each block, setting up the scene for each block, etc.
    ///
    /// This template shows how to set up a custom Block script using the toolkit's built-in functions.
    ///
    /// You can delete any unused methods and unwanted comments. The only required part is the constructor.
    ///
    /// You cannot edit the main execution part of Blocks since their main execution is to run their trials.
    /// </summary>
    public class ___BlockClassName___ : Block {
    
    
        // // You usually want to store a reference to your Experiment runner
        // YourCustomExperimentRunner myRunner;
    
    
        // Required Constructor. Good place to set up references to objects in the unity scene
        public ___BlockClassName___(ExperimentRunner runner, DataTable trialTable, DataRow data) : base(runner, trialTable, data) {
            // myRunner = (YourCustomExperimentRunner)runner;  //cast the generic runner to your custom type.
            // GameObject myGameObject = myRunner.MyGameObject;  // get reference to gameObject stored in your custom runner
        
        }


        // Optional Pre-Block code. Useful for calibration and setup common to all blocks. Executes in a single frame at the start of the block
        protected override void PreMethod() {
        
            // float thisBlocksDistanceValue = (float)Data["MyDistanceFloatVariableName"]; // Read values of independent variables
            // myGameObject.transform.position = new Vector3(thisBlocksDistanceValue, 0, 0); // set up scene based on value
        }

    
        // Optional Pre-Block code spanning multiple frames. Useful for pre-Block instructions.
        // Can execute over multiple frames at the start of a block
        protected override IEnumerator PreCoroutine() {
            yield return null; // yield return required for coroutine. Waits until next frame
            
            // Other ideas:
            // yield return new WaitForSeconds(5);     Waits for 5 seconds worth of frames;
            // can also wait for user input in a while-loop with a yield return null inside.
        }
    
    
        // Optional Post-Block code spanning multiple frames. Useful for Block debrief instructions.
        protected override IEnumerator PostCoroutine() {
            yield return null; //required for coroutine
        }

    
        // Optional Post-Block code.
        protected override void PostMethod() {
            // cleanup code (happens all in one frame at end of block)
        }
    }
}
