---
id: ExperimentBehavior
title: Setting Up Your Experiment's Behavior
---
# Interfacing the experiment with your unity scene

## Experiment Runner

The `ExperimentRunner` `MonoBehaviour` Script is the main connection point between the experimental system and your unity scene. This script interfaces with the Experiment Runner GUI to control and set up your experiment.
`ExperimentRunner` is an abstract class, meaning you need to create your own unique version of it for each experiment. The ExperimentRunner script is attached to an empty gameObject in the unity scene. 

There is a helper tool that can create a basic ExperimentRunner script for you and automatically create and attach it to a gameObject in the currently running scene. To access the helper tool, open the "bmlTUX" menu and select "Script Helper Tool" and follow the instructions. You can also create an `ExperimentRunner` script yourself in a new C# script, and have it inherit from `ExperimentRunner` rather than `MonoBehaviour`. You will need to import the proper namespaces and implement the basic constructor yourself, and attach it to a `GameObject` in the Unity Scene. The `ExperimentRunner` gameObject also needs a reference to a variable configuration file (see above). Drag one into the appropriate field in the inspector.


Now, within the `ExperimentRunner` script, you can declare public fields for any references to other unity objects in your scene that may be required. 


# Defining custom scripts for your experiment

The experiment is separated into several parts that are nested together

The `ExperimentPart` type has three subtypes:
* `Experiment` (usually only one)
    * Creates and runs all blocks
* `Block`
    * Creates and runs trials in block
* `Trial`
    * Runs main logic of experiment (stimulus presentation, data collection, etc.)

Lacking references to customized scripts, the toolkit will load simple pre-defined scripts with the behavior needed to run a simple experiment. You can create your own subclasses of these scripts to customize behaviour.

When you customize behavior for your `Experiment`, `Block`, and `Trial` scripts, you need to tell the `ExperimentRunner` to point to those scripts rather than the default. If you used to Script Helper tool, this should be handled for you automatically. 

If you created the custom scripts manually, you need set up references for the `ExperimentRunner` gameObject to the  custom scripts. From the `ExperimentRunner` gameObject's inspector, drag the appropriate script files into the appropriate fields in the "Script References" section.


## Customizing Script Behaviour
To define custom behavior inside your custom `ExperimentPart`s, the framework automatically calls several methods that you can override to implement your own behavior. 

In call order:

1. `PreMethod()` – Called before the main execution. Useful for setting simpler things up, calibration etc. Such setup can only be done in a single frame.

2. `PreCoroutine()` – Called after `PreMethod()`, useful for setup that spans more than one frame. For example, displaying instructions to the participant for several seconds, or waiting until a key is pressed.

3. `RunMainCoroutine()` – The “business” execution code of the `ExperimentPart`. For trials this is where the main part of the `Trial` should be coded (participant responses, any variable setup). This method can only be overwritten within `Trial`s. This is because in `Block`s and `Experiment`s, the execution is simply to pass control to other `ExperimentPart`s (i.e. an `Experiment` runs its blocks, a `Block` runs its trials). 

4. `PostCoroutine()` – Called after `RunMainCoroutine()`, useful for cleanup that spans more than one frame.

5. `PostMethod()` – Called after `PostCoroutine()`, useful for cleanup that takes places within a single frame. Useful for writing output data.


For example:
```csharp
using bmlTUX

public class MyTrial : Trial {  
    protected override IEnumerator PreCoroutine() {  
        //Your code here that might last for more than one frame 
        yield return null;  
    }  

    protected override IEnumerator RunMainCoroutine() {  
        //Main Trial Code that can last for more than one frame 
        yield return null;  
    }  

    protected override IEnumerator PostMethod() {  
        //Your code here  
        //No yield return needed here  
    }  
}  
```

To learn more about Coroutines, see the following page on Coroutines.

# Accessing other `GameObjects` from within your experiment code

To access other objects in your unity scene within your custom scripts, you must access the Runner object that is stored in every `ExperimentPart`. However, the stored Runner is a generic `ExperimentRunner`, not your custom type. Therefore you must cast it to your custom type to access any objects or variables stored within. Like so:

```csharp

YourCustomRunnerType customRunnerType;

protected override PreMethod() {

    // Cast generic runner to your runner's type.
    customRunnerType = (YourCustomRunnerType)Runner;

    // Now Access something referenced in your ExperimentRunner script using
    GameObject referencedObject = customRunnerType.someGameObjectStoredThere;
}
```