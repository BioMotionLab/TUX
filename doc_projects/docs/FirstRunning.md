---
id: firstrunning
title: Part 4: Writing Runner Scripts
---

In the previous section we learned how to get a project set up for an experiment, create the basic experiment components, define variables, and run an experiment. But our experiment doesn't DO anything yet. In this section we'll define some custom behaviour for our experiment.


## Define what happens in a trial

Now that we have configured our project and basic experiment, we have to define what occurs in a trial. We edit the Trial script that the helper tool created for us. The Trial script inherits from the toolkit's built-in `Trial` Class that contains all the functionality for creating and running trials for us. We simply must extend this base functionality to define the behavior unique to our experiment.

Open the trial script in an editor like Jetbrains Rider or Visual Studio (It should be called TutorialTrialScript.cs). 

You'll see the following code (I've deleted most the comments here for clarity):


```c#
using System.Collections;
using System.Data;
using UnityEngine;
using bmlTUX

public class TutorialTrialScript : Trial {
    
    
    public TutorialTrialScript(ExperimentRunner runner, DataRow data) : base(runner, data) {
        
    }

    
    // Main Trial Execution Code.
    protected override IEnumerator RunMainCoroutine() {
        
        bool waitingForParticipantResponse = true;
        while (waitingForParticipantResponse) {  
            if (Input.GetKeyDown(KeyCode.Space)) { 
                waitingForParticipantResponse = false;  
            }
            yield return null; 
        }
        
    }


}
```


The `RunMainCoroutine()` part is the important part. This is the main execution code of our trial where we will define what happens in our trials. This method is a Coroutine, which means it needs to have at minimum one yield return statement. If you don’t know about coroutines, there is a [section in the documentation](Coroutines-and-IEnumerators.md) explaining them. 
 
Let’s start simple and just have it print something to the console. inside the `MainCoroutine()` method type the following code. Remember to `yield return null` at the end.

```c#
    protected override IEnumerator RunMainCoroutine() {

        Debug.Log("Press the space key please!");  // <- ADD THIS LINE

        bool waitingForParticipantResponse = true;
        while (waitingForParticipantResponse) {  
            
            if (Input.GetKeyDown(KeyCode.Space)) { 
                waitingForParticipantResponse = false;  
            }
            yield return null; 
        }
        
    }
```

Now each trial should output that text once before waiting for the space key.

Let’s run our experiment from the runner window again to see if it worked. Check the console window for the output. If you don't see the console window, open it using **"Window" menu > General > Console**. Make sure all the scripts are saved first!

## Setting up the Stimuli

Now that we have our trial structure set up, we need to create some objects to use as stimuli.

Let’s first position our main Camera to 0,0,-10, and have it pointing along the Z axis (Rotation 0,0,0). 
1. Click the Main Camera in the scene and reset its transform component.

Let’s create the object that will be the model to which participants are trying to match.
1. Create a Capsule `GameObject`, name it MatchObject
2. We want the model to be off to the side. 
3. Set its X position to 2

Let’s create the stimulus object
1. Create a 2nd Capsule `GameObject`, name it Stimulus

Now we need a materials to change the stimulus color. 

1. To create material Right-click on folder > Create > Material
2. Create a 3 materials in the experiment folder, call them RedMaterial, BlueMaterial, GreenMaterial
3. For each material, in its inspector, click the little color next to the albedo, and change it to red/blue/green.

## Referencing GameObjects in your experiment

Now that we’ve set up the objects and materials for our stimuli, we need to be able to reference them in our scripts. We use our experiment runner as our main window to our scene.
1. Open the `TutorialRunner `script.
2. Create public fields for the two capsules and the 3 materials.
3. It should look like this:

```c#
using System.Collections;
using System.Data;
using UnityEngine;
using bmlTUX

public class TutorialRunner : ExperimentRunner
{
    public GameObject Stimulus;
    public GameObject MatchObject;

    public Material Red;
    public Material Blue;
    public Material Green;
}
```


Next, we have to populate these fields in the inspector.

1. In the scene, click on the GameObject in your scene hierarchy with your runner script on it. It should be called something like TutorialRunner,
2. In the inspector you should see fields now. 
3. Drag the appropriate gameObjects into the fields


## Update Trial Script

Now we need to reference these objects within our trial script. We could put this code inside our main coroutine method,  but this makes more sense in a setup method since it does not happen during the trial, but rather before each trial.

1. Open the `TutorialTrialScript.cs` script.

Now our trial script needs a reference to our custom `TutorialRunner` and the objects we defined in it.

1. Let’s create a field in `TutorialTrialScript` to store a reference to it.
2. The base class `Trial` already contains a built-in reference to it named `Runner`, but it’s not stored as our custom `TutorialRunner` but rather a generic `ExperimentRunner`. We need to cast it to our custom class `TutorialRunner`.
6. Add this cast into the body of the constructor. See the code below (some previous code not shown). 
 
```c#
using System.Collections;
using System.Data;
using UnityEngine;
using bmlTUX

public class TutorialTrialScript : Trial
{

    TutorialRunner tutorialRunner;

    public TutorialTrialScript(ExperimentRunner runner, DataRow data) : base(runner, data) {
        tutorialRunner = (TutorialRunner)runner;
    }

   
}
```

Now we can reference our scene’s objects to set them up. But how do we know what size, distance, and color to use? Our base class `Trial` also contains a field called `Data` that stores the values for each trial. We access our variables stored in this Data object to set up each trial. However, recall that we defined distance as a block variable. We’ll set that up later on using Blocks.

## Reading variable values

2. We want to extend the script by overriding the `PreMethod()` method, which is a function that the toolkit will call automatically. This `PreMethod()` is used for setting up a trial. It gets automatically called at the start of each trial.

We want to access the value for our Color variable for each trial.

A given trial's values for all variables are stored in the `Data` object:

```csharp
string colorString = (string)Data[“Color”];
```
 
Note the cast to a string. This is necessary to remind C# that your `Color` variable in the config file is actually a string.

Now we want to set up a switch-statement to set the stimulus’ material to the correct color. we also want code to read and update the stimulus based the size variable. Your code should look something like this:

```csharp
protected override void PreMethod() {

    //Get this trial's value for the Color variable
    string colorString = (string) Data["Color"];

    if (colorString == "Blue") {
        tutorialRunner.Stimulus.GetComponent<MeshRenderer>().material = tutorialRunner.Blue;
    }
    else if (colorString == "Red") {
        tutorialRunner.Stimulus.GetComponent<MeshRenderer>().material = tutorialRunner.Red;
    }
    else if (colorString == "Green") {
        tutorialRunner.Stimulus.GetComponent<MeshRenderer>().material = tutorialRunner.Green;
    }
    else {
        Debug.LogError("Invalid Color Given");
    }

    //Get this trial's value for the Size variable
    float size = (float) Data["Size"];

    //Set the size of Model 
    tutorialRunner.Stimulus.transform.localScale = new Vector3(size, size, size);

}
```

Save and run the experiment. You should notice the cylinder change colors and sizes as it goes through the trials.


## Outputting Data

Outputting data is similar to reading the values. Recall that we created a variable called MatchedSize. We want to output the final size that the participant selected into that variable. We do that after the trial is complete, using the `PostMethod()` which gets automatically called at the end of each trial.

We need to get and store the size the participant selected.
1. We can get the MatchObject's local scale, and read its x value (since y and z will be identical)
2. We write value into the variable stored in our `Data` Object:

```csharp
protected override void PostMethod() {
    Data["MatchedSize"] = tutorialRunner.MatchObject.transform.localScale.x;
}
```

As long as we update the variable in the Data object, the toolkit will automatically update the trial table, and add a row to our output file after each trial is complete with all values updated.

Run the experiment, and look at the Experiment Runner window to double check that the variable is being updated after each trial. Remember, to view the Runner interface at the same time as your scene, you need to open a 2nd tab of the game window and set it to "Display 2".

We now have a functional experiment. But we haven't yet handled user input, our Distance independent variable, or any randomization. Let's set that up now.
