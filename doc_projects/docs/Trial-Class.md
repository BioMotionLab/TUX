---
id: TrialClass
title: Trial Class
---
The code defined in your custom `Trial` class is general for all trials, but the behavior changes based on the values of your variables. Therefore, you only need to code the behavior for trials in one place and set up each trial based on the values of your variables for that trial.


## Automatically generating Trial Scripts

To define the behavior that occurs during a trial, you need to create a script that inherits from the `Trial` type". The toolkit can automatically generate a `Trial` script for you using a template from the "Script Helper Tool" located in the main "bmlTUX" menu.

## Manually creating a Trial Script
You can also create this script manually:

```csharp
using bmlTUX

public class MyTrial : Trial {   
}  
```

However, this will give you an error. In most editors you can automatically solve this by right-clicking on `MyTrial` and selecting “implement missing members”. The Editor will fill in the missing code automatically (a constructor method and an overriden `RunMainCoroutine()` method). 

The added constructor forwards its job up to the main `Trial` class (`base`), so you don’t have to worry about it. Constructors are the way Object Oriented Programming languages create objects. 


# The `RunMainCoroutine()` method

The `RunMainCoroutine()` method is where you define the majority of your custom code to run during a trial. 

```csharp
using bmlTUX

public class MyTrial : Trial {
    public MyTrial(ExperimentRunner runner, DataRow data) : base(runner, data) {}

    protected override IEnumerator RunMainCoroutine() {
        throw new System.NotImplementedException();
        //delete above and replace with own functionality
    }
}
```

You can also override the other `ExperimentPart` methods for custom setup/cleanup etc.

Remember, that the code defined in your custom `Trial` script is general for all `Trial`s, but the behavior changes based on the values of your variables. Therefore, you only need to code the behavior for all trials in one place and set up each one based on the values of your variables for that trial.

As a rule of thumb, it’s a good idea to follow the following structure:

#### In PreMethod():
* Cast the runner object to your custom type, store in field so it can be used below. (could also be in constructor)
* Access your independent variables to set up trial.
* Set up your environment for each trial.
* Set up stimuli, other variables.

#### In PreCoroutine():
* Show trial-specific instructions for a given time.
* Wait for user to do something before trial starts.
* Remember to yield return. (see coroutine section in this guide)

#### In RunMainCoroutine():
* Present stimuli.
* Collect responses.
* Take measurements.
* Remember to yield return. (see coroutine section in this guide)

#### In PostCoroutine():
* Wait for user to do something after trial.
* Remember to yield return. (see coroutine section in this guide)

#### In PostMethod():
* Finalize any measurements.
* Write data to your dependent variables.
* Reset everything in preparation for next trial.

For `Trial`s, after `PostMethod()` completes, the trial automatically updates the output .csv file with any values writen to its dependent variables.


# Access variables within a `Trial`

Accessing your variables within trials is easy. Their values are stored in a field of the `Trial` class called `Data`. This `Data` object will be updated and set to the correct values for each trial automatically by the toolkit.

Let’s say you defined an integer-type independent variable named `MyFirstVariable` with values 1 and 2. To access it from a trial, write the following: 

```csharp
int myFirstVariable = (int) data[“MyFirstVariable”]    
```

This stores each trial’s value into a new int variable called `myFirstVariable` which can be manipulated normally like any other c# variable. You need the cast `(int)` at the start to remind C# that your variable is type `int`.
Now you can modify things for each trial based on the value. 

For example, you could move an object based on its value:

```csharp
Vector3 positionToMoveTo = new Vector3(myFirstVariable, 0, 0);    
someGameObject.transform.localPosition = postionToMoveTo;  
``` 

With this script, each trial will move `someGameObject` to the correct position for that trial based on the value of the variable in that trial.

_Note: See the section on Accessing Unity Components and `MonoBehaviour`s from inside your custom `Experiment`/`Block`/`Trial` scripts for more information about customizing your trials._

#### Advanced

In many instances it makes sense to use an enum to represent variables. In this scenario, create a String Variable in your design file and set its values. Similarly, create a C# script with an Enum with the exact same options. Make sure to check your spelling.

bmlTUX has a built-in function to convert these. 

For example, a Variable "HandSide" might have values "Left" and "Right". Create an enum:
```csharp
public enum HandSide {
    Left
    Right
}
```

To convert string variable value to enum value:

```csharp
// Belongs where you normally read your variables.
HandSide side = EnumFromStringUtility.ExtractEnumFromData<HandSide>(Data, "HandSide")
```



# Writing output measurements to dependent variables in a `Trial`

To write responses or results to your dependent variables, write the following:
```csharp
float response = 5.6f;    
Data[“MyFloatDependentVariable”] = response;    
```

_Note: Any updated values for the dependent variables will be automatically added to the output CSV file on completion of the trial._
