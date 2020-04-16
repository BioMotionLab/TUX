![overview image](Images/ExperimentalSession.png)

# Overview

This tutorial will guide you through the basic steps to make a simple experiment using bmlTUX. For this tutorial our experiment will present the participant with a simple stimulus, and then have the participant match the size of another object to the stimulus. We will vary the stimulus’ size, and distance, and the color of the stimulus. We will also collect participants' age and assign them an ID. We want to record how close the participant matched the size, and the time it took them.

The finished project from this tutorial can be found in the toolkit's Samples folder for reference.

_Note: This tutorial, for simplicity, does not teach best coding practices. It is designed to show the basic functionality of the experiment framework, and in some instances purposely uses poor coding practices to increase clarity._

* [Part 1]
* [Part 2](TutorialExperiment_Part2) 

# Updates

* 2019-11-18: Updated UI significantly
* 2019-10-08: Updated to v0.11.0b to explain Script helper tool.
* 2019-09-25: Updated to v0.10.0b.

# Download

Download the latest release of bmlTUX from [the Releases Page](https://github.com/BioMotionLab/bmlTux/releases)

**You do not need the source code, only the unity package.**

# Requirements
* **Important:** Unity 2019.2 or later required.
* See [Requirements section](Requirements) in main documentation for a more exhaustive list and tips on setting up Unity.
* This tutorial was written for bmlTUX version 0.12.0-beta. Since it is still under heavy development some names/process may be slightly different in newer versions.
* **Highly recommended:** Using [JetBrains Rider IDE](https://www.jetbrains.com/rider/?fromMenu), which is free for academic use (rather than Visual Studio)

# Solving Problems You May Encounter
1. Check the [Troubleshooting](Troubleshooting) page of the wiki.
2. Check syntax and spelling. Many errors are due to this.
3. Compare your code to the completed Tutorial Project located in the "SamplesAndTutorials" folder.
4. If you think you've uncovered a bug or mistake, open a new issue on the [Issues page](https://github.com/BioMotionLab/bmlTux/issues) so we can try to fix it.

# Create a new experiment
Set up a new experiment for the project:
1. Create a new Unity Project and adjust the following settings:
    1. Go to Edit > Project Settings > Player:
    2. Change the Api Compatibility Level to .Net 4.0
5. If you haven't done so, download the latest release of bmlTUX from [the Releases Page](https://github.com/BioMotionLab/bmlTux/releases)
    1. You do not need the source code, only the unity package.
6. Import the downloaded .unitypackage into your project:
    1. In Unity, go to Assets > Import Package > Custom Package and browse to the .unitypackage
    2. Click Import all.

# Set up the Unity Scene
1. Create a new Unity Scene and name it TutorialExperiment
2. Open the "Script Helper Tool" from the "bmlTUX" menu.

![Helper Tool](Images/ScriptHelper.png)

3. Name your Experiment "Tutorial"
4. Click "Automatically set everything up for me". The toolkit will create a bunch of files in your Assets folder and an empty GameObject in your unity scene with an ExperimentRunner script attached to it. This object is the main window of communication between the toolkit and your unity scene.

# Looking at the `ExperimentRunner` script for your experiment
1. Double click on the created `TutorialExperimentRunner.cs` C# file that was created in your Assets folder.
2. The file will load and you can see that it should already be populated with some code for the most basic experiment.
3. The script is a class called `TutorialExperimentRunner`, and it inherits from `ExperimentRunner` class.
   
Your file should look like this (there might also be some comments in it):

```csharp
using bmlTUX.Scripts.ExperimentParts;

public class TutorialExperimentRunner : ExperimentRunner
{

}
```

4. Click on the TutorialExperimentRunner `GameObject` that was created in the unity scene. Notice the TutorialExperimentRunner script was automatically attached to it. 

# Configuring the experimental design

The Script Helper Tool also created and linked a Design File to this `GameObject` and called it `TutorialDesignFile.asset`. This is the file used to set up our experimental design.

The design file provides a simple interface that access a powerful behind-the-scenes system for setting up your experimental design without having to write any code. 

1. Click once on the created design file in the Assets folder to look at its inspector. 
2. You'll see several options. For more detail on what these settings do please see the main [documentation](Home). 

![Design File Inspector](Images/DesignFile.png)

## Variables

1. Notice that there are sections for different types of variables. Briefly:
    1. **Independent variables:** any manipulated variables that change between trials or blocks in your experiment (e.g. which stimuli, presentation time, distance from participant etc.)
    2. **Dependent variables:** outputs or measurements from your experiment (i.e. response time, selection etc.)
    3. **Participant variables:** Collected at the start of the session from each participant (e.g. gender, ID, etc.)
    
    
### Independent Variables
    
Let’s set up our experiment’s variables. 

#### Color

We want to vary the color of our stimulus in each trial. So let’s define an independent variable named Color.

1. In the Design file’s inspector, in the Variable Creation section, under type to create, choose “String” to make a text-based variable.
2. Select Independent.
3. Click Create Variable.
4. You should see a new variable appear in the Independent Variables section.
5. Name it "Color"
6. We want to vary color every trial, so keep Block unchecked.
7. We want an the trials to be created for every possible color, so choose Balanced for the mixing type.
8. Define values for our variable. Click the plus button to add values.
    1. Add values: Red, Blue, Green.
9. You should see something like this:
 
![Image of color variable](Images/ColorVariable.png)


#### Distance

Next, Let’s create our Distance variable. In this case we want all the trials at a given distance to be grouped together, so we want to flag it as a "block" variable.

1. Create a float independent variable.
2. Name it "Distance".
3. Make sure "block" is checked.
4. Choose "balanced" mixing mode.
5. Add values: 1, 2, 3.

#### Size

Next, Let’s create our size variable. 

1. Create a float independent variable
2. Name it "Size".
3. Make sure "block" is unchecked.
4. Choose "balanced" mixing mode.
5. Add values 1, 1.5, 2.

### Participant Variables

We also want to record each participant's gender and age at the start of each experimental session.

Create a string participant variable named Gender
1. Check Constrain values. This ensures that the value is restricted to a set of values that we define.
2. Type in Male, Female, Other

Create an int Participant variable named Age.
1. Keep Constrain values unchecked so we can type in their age.

### Dependent Variables

We want to record how closely the participants match the size of the stimulus. We’ll record that in a float dependent variable called SelectedSize. The default value will be assigned to any missing values in case of problems or stopping the experiment early.


### Other Settings

Finally, we want to randomize the trial order completely.

1. In the top of the config file inspector, select the following settings:
    1. Block Randomization: Complete Randomization
    2. Trial Randomization: Randomize (and choose Different Permutations).
2. We don’t want to repeat anything so leave both repetition settings at 1.
 
# Test out our experiment design using basic components

The Script Helper tool defined enough of the experiment to let you get up and started quickly. Lets test out if everything is working as expected before moving forward.

Press play in the editor. You’ll see a window to start an experimental session. Unity may prompt you to install TextMeshPro assets. You should accept this since the Runner Interface depends on them.

**_Important Note: The Runner interface is meant to be displayed on a secondary monitor so that the experimenter can monitor and setup the experiment without the participant seeing it. To view both at the same time, make sure you have a second "Game" tab open set to "Display 2". The main experiment will be shown on Display 1, with the interface showing on Display 2. This can be adjusted in the settings._**

![Display Selection](Images/DisplaySelection.png)

**_Important Note for VR Users: SteamVR automatically sets all cameras in the scene to track the HMD. I haven't found a way to disable this. You need to disable it manually. In the TUX folder, there's a prefab for the UI called "ExperimentGUI". Open the prefab and there should be a camera object inside called "ExperimentUICamera". Adjust the "Target Eye" to "Display(None)". This will render the UI to the display rather than floating in front of the HMD. I'm looking for solutions to avoid this step._**

1. Take a look at the previewed trial table and ensure everything is set up properly. Note that the toolkit adds some useful columns to track progress and other metrics.
2. Debug mode is useful to check functionality during development without having to type in complicated options. Click Debug.
3. Because it’s debug mode it automatically chooses values for our participant variables and other things. A normal run through the program will force you to select a value. 
4. Once started, the toolkit automatically constructs your blocks and trials based on code that you provide the ExperimentRunner class. At the moment, we haven't provided any, so it reverts to the built-in defaults.
5. A new window will appear showing the progress through the experiment.
6. In the default scripts, a trial is simply defined as pressing the return key. Press it a few times to see how the trials advance, and the values of the independent variables change.  

_Note: Make sure you have one of the "Game" windows focused for the key presses to register_
 
It looks like everything is working properly. Although the auto-generated scripts have enough functionality to allow the experiment to run, but they don't _do_ anything to our Unity scene. 


# Next step:

In the next part of the tutorial we'll learn how to extend our auto-generated scripts by defining some custom behaviour in our experiment.

[Next Section](TutorialExperiment_Part2)
