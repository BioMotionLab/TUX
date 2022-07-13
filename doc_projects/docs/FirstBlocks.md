---
id: firstblocks
title: Part 5: Defining Blocks
---


## Participant input

We need let the participants resize the stimulus to match it to the model. Lets create a new `MonoBehaviour` script attached to our MatchObject GameObject. We'll call it Matcher.

Next, we'll set up keys to scale the stimulus up or down. Let’s use `up`and `down` arrow keys.

1. Add if statements for the keys. In this instance we want `GetKey` instead of `GetKeyDown` to check if the participant held the key down.
2. Adjust size of stimulus via its `transform.localScale`.
3. We’ll create variable for adjusting the speed that it gets resizing so we can adjust it later.
4. We resize it depending on how much time has passed since last frame. This will keep the speed consistent.
5. Finally, we want to disable this based on a boolean value.

Your code should look something like this:
 
```csharp
using UnityEngine;

public class Matcher : MonoBehaviour {

    public bool allowAdjustment = false;
    public float resizeSpeed = 1;
    

    void Update()
    {
        if (allowAdjustment) {

            if (Input.GetKey(KeyCode.UpArrow)) {
                IncreaseSize();
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                DecreaseSize();
            }

        }

    }

    void IncreaseSize() {
        float amount = resizeSpeed * Time.deltaTime;
        transform.localScale += new Vector3(amount, amount, amount);
    }

    void DecreaseSize() {
        float amount = resizeSpeed * Time.deltaTime;
        transform.localScale -= new Vector3(amount, amount, amount);
    }
}
```

### Update TutorialRunner with reference

We need to get a reference to the Matcher Script in our TutorialRunner. So add a public field:

```c#
public Matcher Matcher;
```

Then drag in the MatchObject to the field in the inspector.


### Update Trial Script to enable

We want to enable this matcher script during the `RunMainCoroutine()` method of our trials, as we're waiting for confirmation with the space key. Let's modify our method like so. 

```c#
protected override IEnumerator RunMainCoroutine() {
    bool waitingForParticipantResponse = true;
    while (waitingForParticipantResponse) {

        tutorialRunner.Matcher.allowAdjustment = true;  // <----

        if (Input.GetKeyDown(KeyCode.Space)) { 
            waitingForParticipantResponse = false;  
        }

        yield return null; 
    }

    tutorialRunner.Matcher.allowAdjustment = false; // <----
}
```

Test it out. We now have a pretty functional experiment. We can try to match the size of the colored stimulus to the model using the W and S keys, And when closely matched, we can hit the Space key to move to the next trial. 


## Block Variables

Let’s create our Distance variable. To move our stimuli to different distances. In this case we want all the trials at a given distance to be grouped together, so we want to flag it as a "block" variable.

1. Create a float independent variable.
2. Name it "Distance".
3. Make sure "block" is checked.
4. Choose "balanced" mixing mode.
5. Add values: 1, 2, 3.

### Update Block Script

We can define block-specific behavior similar to the way we defined our `TutorialTrialScript`. We can modify our `TutorialBlock` that inherits from the base `Block` class. We can use a similar procedure to overwrite methods to add functionality.


1. Open `TutorialBlock.cs`.

1. We want to move the stimulus to the different distances at the start of each block.
2. We have to first get access to our TutorialRunner as before.
3. As before, override the `PreMethod()` to add code that runs at the start of each block of trials.
4. We want to read the Distance variable from the `Data` object just like we did in a trial. 
5. Let's save its initial position, so we can reset it at the end of the block in `PostMethod`, and then adjust its position to the value of the distance variable on the Z axis.


_Note: A `Block`’s `Data` object only has access to block variables, not other variables. However, a `Trial` has access to both block and trial variables._

```csharp
public class TutorialBlock : Block {

    TutorialRunner tutorialRunner;
    Vector3 initialPosition;

    public TutorialBlock(ExperimentRunner runner, DataTable trialTable, DataRow data, int index) 
        : base(runner, trialTable, data, index) {
        tutorialRunner = (TutorialRunner) runner;
    }

    protected override void PreMethod() {
        float distance = (float) Data["Distance"];
        initialPosition = tutorialRunner.Stimulus.transform.position;
        tutorialRunner.Stimulus.transform.position = new Vector3(initialPosition.x, initialPosition.y, distance);
    }

    
    protected override void PostMethod() {
        tutorialRunner.Stimulus.transform.position = initialPosition;
    }
}
```

## Randomization

Finally, we want to randomize the trial order completely.

1. In the top of the Design File Inspector, select the following settings:
    1. Block Randomization: Complete Randomization
    2. Trial Randomization: Randomize (and choose Different Permutations).
2. We don’t want to repeat anything so leave both repetition settings at 1.


## Run and Test

Verify that it works! Run the experiment and make sure that the distance changes for each block.


## Moving forward

This tutorial got you started making a functional experiment. You learned how to set up a basic experiment, customize trial and block behavior, and output to a file. Check out the [main documentation](Home) to learn more about using the more advanced features of the Toolkit.

