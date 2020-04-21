---
id: CreatingExperiment
title: Creating an Experiment
---

Creating a new experiment is best done in a new project.

Some project settings are required:
1. API compatibility level to .NET 4.0
2. Scripting Runtime Version .Net 4.x Equivalent
3. Import the latest release of the toolkit .unitypackage file.

# Setting up an experiment

From toolkit version 0.11.x onwards, there is a new helper tool that is accessible from the main "BML" menu that will automatically create all required files and set up a basic experiment for you. Open the Script Helper Window, and click "Automatically set everything up for me!". The toolkit will then add a new GameObject to your unity scene, generate the skeletons of all required scripts, and set up all the needed references. The created files will be in the root "Assets" folder of your unity project.

# Configuring your experimental design
If you used the helper tool to automatically create all files, the Design File should be created for you. Or, you can create your own using the tool or by right-clicking on a folder in your project and navigate to "Create > bmlTUX > New Design File". 

Select the Design file to open its inspector window. It should look something like (Figure 2).

![VariableConfigurationFile Screenshot](Images/DesignFile.png)

Figure 2: A screenshot of the variable creation inspector.

# Randomization and Repetition
At the top of this inspector you can adjust randomization and repetition settings.

### Randomization Mode
* **None:** No Randomization
* **Randomize but same order in each block:** Trial order is randomized but is consistent across blocks.
* **Randomize all:** Trial order is completely randomized across blocks

### Repeat Trials in block
Each trial is repeated multiple times within each block.

### Repeat All Blocks
Runs through all blocks multiple times.

# Configuring Variables
To add a variable, select your desired data type (int, string, etc.), type of variable (independent, dependent, participant), and then click on Create Variable. You should see your variable added to the list below. Configure your variable by naming it, adding values, selecting whether the variable will be used to create blocks of trials, etc. 
To delete a variable from the list click the Delete Variable button
Your changes will be saved automatically.

### Name
You must name your variables. This will be how you access their values within your trials. I recommend using one-word names, or joining words using underscores. Do not use any special characters or punctuation.

### Datatype
Once created, you cannot modify this variable’s datatype. This lets Unity know what kind of data to expect. Currently, supported datatypes include:
* Int
* Float
* String
* GameObject prefabs
* Vector3
Although a bit difficult, you can add support for your own types if desired.

### Variable Type
Independent and dependent variables are treated differently. Independent variables define your experimental structure, and dependent variables are values that determined from responses or measurements during each trial

# Independent Variables
Variables whose values change over the course of the experiment. Normal independent variables' values vary each trial, whereas blocked independent variables vary only over blocks of trials (several trials in a row have the same value). (e.g. which stimulus, color of stimulus, experimental conditions, etc.)

## Independent variable options:

### Block
Enabling this option will create blocks of trials for each value defined. Useful for counterbalancing or variables that require setup in real life (i.e. participants need to move between values) 

### Mixing Type of Variable
This option will define how the variable is mixed with the other variables when creating your experimental design.
#### Balanced
This will create trials for every combination of values of each balanced variable. Example: for two 3-value balanced variables, there will be 3x3=9 trials.

|Trial Number|BalancedVariable1|BalancedVariable2|
|---|---|---|
|1|1|1|
|2|1|2|
|3|1|3|
|4|2|1|
|5|2|2|
|6|2|3|
|7|3|1|
|8|3|2|
|9|3|3|

#### Looped:
This will loop through the values such that there is an equal number of trials with each value. Lowest common multiple.

Example1: For a variable with 2 values, and another variable with 4 values, the following table will be created. In this case the lowest common multiple is 4 trials.

|Trial Number|LoopedVariable1|LoopedVariable2|
|---|---|---|
|1|1|1|
|2|2|2|
|3|1|3|
|4|2|4|

Example2: For a variable with 2 values, and another variable with 3 values the following table. In this case the lowest common multiple is 6 trials.

|Trial Number|LoopedVariable1|LoopedVariable2|
|---|---|---|
|1|1|1|
|2|2|2|
|3|1|3|
|4|2|1|
|5|1|2|
|6|2|3|

Note that adding looped variables occurs before adding balanced variables, so all looped variables as a group are essentially treated as one balanced variable.

#### Even Probability:
Each trial will have a randomly selected value for this variable, with the same probability for each value.

Example, A balanced variable with 6 levels, and an even probability variable with 10 values (numbers 1-10).

|Trial Number|BalancedVariable1|EvenProbabilityVariable1|
|---|---|---|
|1|1|4|
|2|2|8|
|3|3|9|
|4|4|4|
|5|5|7|
|6|6|2|

#### Custom Probability:
Each trial will have a randomly selected value for this variable, with a defined probability of being selected. You define the probability to the right of each value (as a decimal). The final probability is automatically calculated to ensure they add up to 1.

Example, A balanced variable with 6 levels, and a custom probability variable with 2 values with the first value having 0.2 probability, and the second value having 0.8 probability.

|Trial Number|BalancedVariable1|CustomProbabilityVariable1|
|---|---|---|
|1|1|2|
|2|2|2|
|3|3|2|
|4|4|1|
|5|5|2|
|6|6|2|

### Values
Define the levels of each variable. You must define at least one value.

### Probability
Only visible when custom probability mixing type is selected. This is where you define your probability of each value being picked.

# Dependent Variables
Variables whose values are calculated or measured during the experiment. (e.g. reaction time, selected value, etc.)

## Dependent variable options:

### Default value
This is the value given to your dependent variables in case there is no response given, or the variable is not updated in your trial. This can be left blank if desired.

Tip: Dependent variables can also be used as input constants if you want them to also appear in your output file. Just set default value to constant and don't write to it.

# Participant Variables
Variables whose values are measured once per participant (e.g. gender, age, etc.)

The interface will prompt you to enter values at the start of each session.

## Participant variable options:

### Constrain Values
When checked, forces values to be among a specified set of values.

### Values
These are the set of values that are allowed when Constrain Values is checked.


# Adding Config file to ExperimentRunner
If you used the helper to tool to automatically set everything up, the design file should already be referenced by the Experiment Runner Module. If you did it manually, you will need to drag your Design File into the inspector of your custom ExperimentRunner GameObject (described below), and it will run.

The power of the configuration files is that you can run the same experiment with multiple saved configurations. For example, you can select different values for your experiment in different config files and drag your desired configuration into the experiment object prior to running participants. This also allows you to iteratively design your experiments while saving previous configurations. 

# Advanced Options

There are several advanced options available for further customization. Please see the [Advanced page](Advanced-options) of this wiki for more information
