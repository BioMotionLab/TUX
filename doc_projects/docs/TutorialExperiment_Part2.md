
# Tutorial Experiment Part 2

* [Part 1](TutorialExperiment)
* [Part 2] 

In the previous section we learned how to get a project set up for an experiment, create the basic experiment components, define variables, and run an experiment. But our experiment doesn't DO anything yet. In this section we'll define some custom behaviour for our experiment.


# Define what happens in a trial

Now that we have configured our project and basic experiment, we have to define what occurs in a trial. We edit the Trial script that the helper tool created for us. The Trial script inherits from the toolkit's built-in `Trial` Class that contains all the functionality for creating and running trials for us. We simply must extend this base functionality to define the behavior unique to our experiment.

Open the trial script in an editor like Jetbrains Rider or Visual Studio (It should be called TutorialTrialScript.cs). 

You'll see the following code (I've deleted most the comments here for clarity):


```csharp
using System.Collections;
using System.Data;
using UnityEngine;
using bmlTUX.Scripts.ExperimentParts;

public class TutorialTrialScript : Trial {
    
    // Constructor
    public TutorialTrialScript(ExperimentRunner runner, DataRow data) : base(runner, data) {
        
    }

    protected override void PreMethod() {}

    protected override IEnumerator PreCoroutine() {
        yield return null;
    }

    
    // Main Trial Execution Code.
    protected override IEnumerator RunMainCoroutine() {
        
        bool waitingForParticipantResponse = true;
        while (waitingForParticipantResponse) {  
            if (Input.GetKeyDown(KeyCode.Return)) { 
                waitingForParticipantResponse = false;  
            }
            yield return null; 
        }
        
    }

    
    protected override IEnumerator PostCoroutine() {
        yield return null;
    }

    protected override void PostMethod() {
    }
}
```

The `RunMainCoroutine()` part is the important part. This is the main execution code of our trial where we will define what happens in our trials. This method is a Coroutine, which means it needs to have at minimum one yield return statement. If you don’t know about coroutines, there is a [section in the documentation](Coroutines-and-IEnumerators) explaining them. 
 
Let’s start simple and just have it print something to the console. inside the `MainCoroutine()` method type the following code. Remember to `yield return null` at the end.

```csharp
    protected override IEnumerator RunMainCoroutine() {
        Debug.Log("Press the return key please!");  // <- ADD THIS LINE

        bool waitingForParticipantResponse = true;
        while (waitingForParticipantResponse) {  
            
            if (Input.GetKeyDown(KeyCode.Return)) { 
                waitingForParticipantResponse = false;  
            }
            yield return null; 
        }
        
    }
```

Now each trial should output that text once before waiting for the return key.

Let’s run our experiment from the runner window again to see if it worked. Check the console window for the output. If you don't see the console window, open it using **"Window" menu > General > Console**. Make sure all the scripts are saved first!

# Setting up the Stimuli

Now that we have our trial structure set up, we need to create some objects to use as stimuli.

Let’s first position our main Camera to 0,0,0, and have it pointing along the Z axis. 
1. Click the Main Camera in the scene and reset its transform component.

Let’s create the object that will be the model to which participants are trying to match.
1. Create a Capsule `GameObject`, name it ModelObject
2. We want the model to be off to the side. 
3. Set its X position to 2

Let’s create the stimulus object
1. Create a 2nd Capsule `GameObject`, name it SimulusObject

Now we need a materials to change the stimulus color. 

1. To create material Right-click on folder > Create > Material
2. Create a 3 materials in the experiment folder, call them RedMaterial, BlueMaterial, GreenMaterial
3. For each material, in its inspector, click the little color next to the albedo, and change it to red/blue/green.

# Referencing GameObjects in your experiment

Now that we’ve set up the objects and materials for our stimuli, we need to be able to reference them in our scripts. We use our experiment runner as our main window to our scene.
1. Open the `TutorialExperimentRunner `script.
2. Create public fields for the two capsules and the 3 materials.
3. It should look like this:

```csharp
using System.Collections;
using System.Data;
using UnityEngine;
using bmlTUX.Scripts.ExperimentParts;

public class TutorialExperimentRunner : ExperimentRunner
{
    public GameObject Stimulus;
    public GameObject Model;

    public Material Red;
    public Material Blue;
    public Material Green;
}
```


Next, we have to populate these fields in the inspector.

1. In the scene, click on the GameObject in your scene hierarchy with your runner script on it. It should be called something like TutorialExperimentRunner,
2. In the inspector you should see fields now. 
3. Drag the appropriate gameObjects into the fields
4. It should look like this:

![Inspector for experiment object](Images/ExperimentObject.png)


Now we need to reference these objects within our trial script. We could put this code inside our main coroutine method,  but this makes more sense in a setup method since it does not happen during the trial, but rather before each trial.

1. Open the `TutorialTrialScript.cs` script.
2. We want to extend the script by override the `PreMethod()` method, which is a function that the toolkit will call automatically. This `PreMethod()` is used for setting up a trial. It gets automatically called at the start of each trial.

Now our trial script needs a reference to our custom `TutorialExperimentRunner` and the objects we defined in it.

1. Let’s create a field in `TutorialTrialScript` to store a reference to it.
2. The base class `Trial` already contains a built-in reference to it named `Runner`, but it’s not stored as our custom `TutorialExperimentRunner` but rather a generic `ExperimentRunner`. We need to cast it to our custom class `TutorialExperimentRunner`.
6. Add this cast into the body of the `PreMethod()`. See the code below (some previous code not shown). 
 
```csharp
using System.Collections;
using System.Data;
using UnityEngine;
using bmlTUX.Scripts.ExperimentParts;

public class TutorialTrialScript : Trial
{
    public TutorialTrialScript(ExperimentRunner runner, DataRow data) : base(runner, data) {
    }

    TutorialExperimentRunner tutorialRunner;

    protected override void PreMethod() {
        tutorialRunner = (TutorialExperimentRunner)Runner;
    }
}
```

Now we can reference our scene’s objects to set them up. But how do we know what size, distance, and color to use? Our base class `Trial` also contains a field called `Data` that stores the values for each trial. We access our variables stored in this Data object to set up each trial. However, recall that we defined distance as a block variable. We’ll set that up later on using Blocks.

## Reading values from `Data`

We want to access the value for our Color variable for each trial:

```csharp
string colorString = (string)Data[“Color”];
```
 
Note the cast to a string. This is necessary to remind C# that your `Color` variable in the config file is actually a string.

Now we want to set up a switch-statement to set the stimulus’ material to the correct color. we also want code to read and update the stimulus based the size variable. Your code should look something like this:

```csharp
protected override void PreMethod() {
    tutorialRunner = (TutorialExperimentRunner)Runner;

    //Get this trial's value for the Color variable
    string colorString = (string) Data["Color"];

    //Set the material of the stimulus to the color
    MeshRenderer stimulusMesh = tutorialRunner.Stimulus.GetComponent<MeshRenderer>();
    switch (colorString) {
        case "Red":
            stimulusMesh.material = tutorialRunner.Red;
            break;
        case "Blue":
            stimulusMesh.material = tutorialRunner.Blue;
            break;
        case "Green":
            stimulusMesh.material = tutorialRunner.Green;
            break;
        default:
            throw new ArgumentOutOfRangeException($"No material defined for {colorString}");
    }

    //Get this trial's value for the Size variable
    float size = (float) Data["Size"];

    //Set the size of Model 
    Transform modelTransform = tutorialRunner.Model.transform;
    modelTransform.localScale = new Vector3(size, size, size);

}
```

Save and run the experiment. You should notice the cylinder change colors and sizes as it goes through the trials.


# Participant input

Now that our trials are being set up with the `PreMethod()`, we need let the participants resize the stimulus to match it to the model. Since this will occur during the main execution of the trial, we'll code this in the `RunMainCoroutine()` method. We also want our script to keep checking and waiting until the participant ends the trial using the return key. This functionaly should be automatically included in your script if you used the Script Helper Tool. It should look something like this:

```csharp
protected override IEnumerator RunMainCoroutine() {

    Debug.Log("Trial Running! Press Return key to end trial");

    bool waitingForParticipantResponse = true;
    while (waitingForParticipantResponse) {
        if (Input.GetKeyDown(KeyCode.Return)) {
            waitingForParticipantResponse = false;
        }

        
        yield return null; // This is very important to avoid infinite loops!
    }
}
```

**IMPORTANT:** Always remember to `yield return null` inside while loops in coroutines so the program can continue to the next frame rather than hanging it in an infinite loop. Tip: If you ever get stuck in an infinite loop, press Control-Alt-Delete, open the task manager and close the Unity Editor  program.

Test out that it works. Unity only detects key presses when the "Game" window is focused in the editor. So make sure to click on the game window if it’s not picking up your key presses.


Next, we'll set up keys to scale the stimulus up or down. Let’s use `w`and `s` keys.

1. Add if statements for the keys. In this instance we want `GetKey` instead of `GetKeyDown` to check f the participant held the key down.
2. Adjust size of stimulus via its `transform.localScale`.
3. We’ll create variable for adjusting the speed that it gets resizing so we can adjust it later.
4. We resize it depending on how much time has passed since last frame. This will keep the speed consistent.
5. Your code should look something like this:
 
```csharp
    protected override IEnumerator RunMainCoroutine() {
        Debug.Log("Trial Running!");
        bool waitingForParticipantResponse = true;
        while (waitingForParticipantResponse) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                waitingForParticipantResponse = false;
            }

            //Define moveSpeed (this should probably be at top of file somewhere)
            float resizeSpeed = 1;

            //Store the stimulus' transform so we reduce typing (this should probably be in PreMethod)
            Transform stimulusTransform = tutorialRunner.Stimulus.transform;

            //calculate how much to change size based on time since last frame and resizeSpeed
            float sizeChange = Time.deltaTime * moveSpeed;
            Vector3 changeVector = new Vector3(sizeChange, sizeChange, sizeChange);

            //find the new value for the localScale of the stimulus.
            //Add on W key, subtract on S key
            Vector3 newScale = stimulusTransform.localScale;
            if (Input.GetKey(KeyCode.W)) {
                newScale += changeVector;
            }
            else if (Input.GetKey(KeyCode.S)) {
                newScale -= changeVector;
            }

            //Update the localScale to the new value
            stimulusTransform.localScale = newScale;

            yield return null;
        }
    }
```

Test it out. We now have a pretty functional experiment. We can try to match the size of the colored stimulus to the model using the W and S keys, And when closely matched, we can hit the Return key to move to the next trial. 

Although our trials are set up well, we still need to output the participant’s response.


# Outputting Data

Outputting data is similar to reading the values. Recall that we created a variable called SelectedSize. We want to output the final size that the participant selected into that variable. We do that after the trial is complete, using the `PostMethod()` which gets automatically called at the end of each trial.

We need to get and store the size the participant selected.
1. We can get the stiumulus’ local scale, and read its x value (since y and z will be identical)
2. We write value into the variable stored in our `Data` Object:

```csharp
protected override void PostMethod() {
    float selectedSize = tutorialRunner.Stimulus.transform.localScale.x;
    Data["SelectedSize"] = selectedSize;
}
```

As long as we update the variable in the Data object, the toolkit will automatically update the trial table, and add a row to our output file after each trial is complete with all values updated.

Run the experiment, and look at the Experiment Runner window to double check that the variable is being updated after each trial. Remember, to view the Runner interface at the same time as your scene, you need to open a 2nd tab of the game window and set it to "Display 2".

![outputted data](Images/Running%20experiment.png)

When using debug mode, the output is saved in a debug file located in your computer's Documents folder inside the bmlTUX sub folder:
2. Open it up and take a look to make sure the output is right.

When debug mode is not used you can enter in values for the participant variables and save the file to disk for data analysis. Try it out!

We now have a functional experiment. But we haven't yet handled our Distance independent variable, which we flagged as a "block" variable. Let's set that up now.

# Defining Blocks

Recall that we set up the distance variable as a block variable. We can define block-specific behavior similar to the way we defined our `TutorialTrialScript`. We can modify our `TutorialBlockScript` that inherits from the base `Block` class. We can use a similar procedure to overwrite methods to add functionality.

1. Open `TutorialBlockScript.cs`.
2. As before, override the `PreMethod()` to add code that runs at the start of each block of trials.
3. We want to read the Distance variable from the `Data` object just like we did in a trial. 

```csharp
float distance = (float) Data["Distance"];
```

_Note: A `Block`’s `Data` object only has access to block variables, not other variables However, a `Trial` has access to both block and trial variables._

1. We want to move the stimulus to the different distances at the start of each block.
2. We have to first get access to our TutorialExperimentRunner as before.
 
```csharp
using System.Collections;
using System.Data;
using UnityEngine;
using bmlTUX.Scripts.ExperimentParts;

public class TutorialBlock : Block
{
    public TutorialBlock(ExperimentRunner runner, DataTable trialTable, DataRow dataRow) 
        : base(runner, trialTable, dataRow) {
    }

    protected override void PreMethod() {
        //Get reference to custom ExperimentRunner
        TutorialExperimentRunner tutorialRunner = (TutorialExperimentRunner)Runner;

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
```

6. Verify that it works! Run the experiment and make sure that the distance changes for each block.


# Moving forward

This tutorial got you started making a functional experiment. You learned how to set up a basic experiment, customize trial and block behavior, and output to a file. Check out the [main documentation](Home) to learn more about using the more advanced features of the Toolkit.


* [Part 1](TutorialExperiment)
* [Part 2] 