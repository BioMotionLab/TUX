---
id: firstblocks
title: Part 5: Defining Blocks
---


Recall that we set up the distance variable as a block variable. We can define block-specific behavior similar to the way we defined our `TutorialTrialScript`. We can modify our `TutorialBlockScript` that inherits from the base `Block` class. We can use a similar procedure to overwrite methods to add functionality.

## Update Block Script

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

## Run and Test

Verify that it works! Run the experiment and make sure that the distance changes for each block.


## Moving forward

This tutorial got you started making a functional experiment. You learned how to set up a basic experiment, customize trial and block behavior, and output to a file. Check out the [main documentation](Home) to learn more about using the more advanced features of the Toolkit.

