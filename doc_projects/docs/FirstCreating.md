---
id: firstcreating
title: Part 2: Creating a new Experiment
---

## New Unity Project

1. Create a new Unity Project and adjust the following settings:
    1. Go to Edit > Project Settings > Player:
    2. Change the Api Compatibility Level to .Net 4.0
2. Install TextmeshPro package:
    1. Due to a bug in textmeshpro's earlier versions. bmlTux requires minimum version 2.1.0. At the time of writing, this was still in preview.
    2. Open Window > Package Manager. Select "All packages" from the dropdown
    3. Find TextMeshPro and install it. If you don't see 2.1.0 or later, you will need to enable preview packages in the "Advanced dropdown"
    4. A popup will appear after import asking to import essentials. Do this.


## Download bmlTUX

1. Download the latest release of bmlTUX from [the Releases Page](https://github.com/BioMotionLab/bmlTux/releases)
    1. **You do not need the source code, only the unity package.** 
2. Import the downloaded .unitypackage into your project:
    1. In Unity, go to Assets > Import Package > Custom Package and browse to the .unitypackage
    2. Click Import all.
    

## Set up the Unity Scene
1. Create a new Unity Scene and name it TutorialExperiment
2. Open the "Script Helper Tool" from the "bmlTUX" menu.
3. Name your Experiment "Tutorial"
4. Click "Automatically set everything up for me". The toolkit will create a bunch of files in your Assets folder and an empty GameObject in your unity scene with an ExperimentRunner script attached to it. This object is the main window of communication between the toolkit and your unity scene.

![Helper Tool](assets/Tutorials/ScriptHelper.png)

## Looking at the Experiment Runner script for your experiment
1. Double click on the created `TutorialExperimentRunner.cs` C# file that was created in your Assets folder.
2. The file will load in your editor and you can see that it should already be populated with some code for the most basic experiment.
3. The script is a class called `TutorialExperimentRunner`, and it inherits from `ExperimentRunner` class.
   
Your file should look like this (there might also be some comments in it):

```csharp
using bmlTUX.Scripts.ExperimentParts;

public class TutorialExperimentRunner : ExperimentRunner
{

}
```

4. Click on the TutorialExperimentRunner `GameObject` that was created in the unity scene. Notice the TutorialExperimentRunner script was automatically attached to it. This is where we'll reference objects in our Unity Scene that we need to manipulate.
