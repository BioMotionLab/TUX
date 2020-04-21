---
id: ExperimentClass
title: Experiment Class
---
To Define custom behavior  experiment, you need to create a new script and have it inherit from the `Experiment `class. 

## Automatically generating Experiment Scripts

To define the behavior that occurs during a trial, you need to create a script that inherits from the `Experiment` type". The toolkit can automatically generate an `Experiment` script for you using a template from the "Script Helper Tool" located in the main "BML" menu.

## Manually creating a Experiment Scripts
You can also create this script manually.

Similar to manually creating `Blocks` and `Trials` you’ll need to import the appropriate namespace and implement missing members

```csharp
using BML_ExperimentToolkit.Scripts.ExperimentParts

public class MyExperiment : Experiment {  }  
```

As a rule of thumb, it’s a good idea to follow the following structure:

1. `PreMethod()`
    * Initialize your experiment (calibration, etc.)

2. `PreCoroutine()`
    * Show instructions, welcome screen.
    * Wait for user to do something before Experiment starts.
    * Remember to yield return. (see coroutine section in this guide)

3. `MainCoroutine()`
    * No customization allowed (runs Blocks automatically)

4. `PostCoroutine()`
    * Wait for user to do something after Experiment.
    * Thank you screen
    * Remember to yield return. (see coroutine section in this guide)

5. `PostMethod()`
    * Finalize your experiment (confirm calibration still valid, etc.)	
