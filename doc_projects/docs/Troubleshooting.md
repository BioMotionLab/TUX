---
id: Troubleshooting 
title: Troubleshooting
---
* My `MonoBehaviour` script is not showing up in the inspector.
    * Make sure the Class name matches the Filename exactly. `MonoBehaviour`s require this.

* My custom `ExperimentRunner` script is not showing up in the inspector. Or I can’t drag it to a `GameObject` in the scene
    * Make sure the class name matches the filename exactly. `MonoBehaviour`s require this.
    * Make sure there are no syntax errors in any code in your project. One error will prevent unity from dragging `MonoBehaviour`s to objects.

* My Experiment runs, but doesn't seem to call my custom `Block` or `Trial` scripts
    * Make sure that your ExperimentRunner script is attached to a gameObject in the scene, and references the appropriate script files in its inspector

* I'm getting compile errors
    * Make sure your project settings are set up for API Compatibility Level is set to .Net 4.x. (Edit > Project Settings > Player > Other Settings)

* I'm getting argument exceptions about columns missing
    * Make sure you've spelled your variables and your access correctly. These errors are usually due to mismatch between the spelling of a variable in your Variable Config file and your custom Trial script. For example if you spelled your variable "Color" **(no u)** but accessed it using `Data["Colour"]` **(with a u)**, you will get an `ArgumentException` error.

* How can I access the current trial or block index, or the total number of trials?
    * The toolkit is designed specifically so that you don't have to ever worry about this. 
    * If you find yourself wanting to use these numbers, it's probably a sign that you are not designing things simply.
    * One example: "I want to show instructions after every repetition through the trials, and there are 8 trials, so I need to know the index so I can show instructions on the right trial."
        * Solution: Create a block variable such that each block contains the 8 trials, and show instructions at the start of every block.
    * If you really need to, trials and blocks have an Index property that will tell you its index (at runtime only)

* The Experiment Runner UI Looks strange or is Tiny when I have a VR headset hooked up.
    * SteamVR automatically sets all cameras in the scene to track the HMD. I haven't found a way to disable this. You need to disable it manually. In the TUX folder, there's a prefab for the UI called "ExperimentGUI". Open the prefab and there should be a camera object inside called "ExperimentUICamera". Adjust the "Target Eye" to "Display(None)". This will render the UI to the display rather than floating in front of the HMD. I'm looking for solutions to avoid this step.

## Bug reporting

if you found a bug or something that needs improvement, please open a ticket on our [issues](https://github.com/BioMotionLab/TUX/issues) page