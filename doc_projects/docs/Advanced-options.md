---
id: AdvancedOptions
title: Advanced Options
---


In addition to the basic [Settings](Settings.md) There are several advanced options accessible from the the Variable Configuration File's inspector. 


# Manual Block Order Configuration

When using block variables, one must select a block order prior to each experimental session. However, if there are many values for block variables, the number of permutations can become enormous. In these cases, a warning message is displayed prompting users to manually define their possible block orders. 

To manually edit block order definitions, look at the Advanced section of the ExperimentDesignFile inspector.  

Clicking "Create and add new file" will create a new Block Order Definition File alongside your Design file, and it will be automatically selected for editing. 

To edit this Block Order Definition File, click on it, and look at the inspector. The order can be changed by dragging and reorder the block values shown in the inspector. The order will run top to bottom. Clicking "randomize" will create a random block order at runtime at the start of an experimental session.

At the beginning of an experimental session, the UI will prompt you to select one of the block order definitions from the list. 

Some tips:
* You should rename the order definition file for clarity. 
* You can define any number of block orders. 


# Pre-Generated Experiment Tables

By default, the experimental toolkit creates and randomizes all trials "On the fly", that is, at the start of each experimental session. However, in some circumstances, experimenters may want to determine the exact trial order and structure prior to any experiment taking place. In such circumstances, experimenters can select the option for using Pre-Generated experiment tables. The toolkit can assist in creating such pre-generated tables by clicking the generate button. The UI will prompt the experimenter at the beginning of the session to load the appropriate design file.

# Editor Settings

Some preferences can be adjusted in the bmlTUX tab in your project settings.

Debug Log Coloring: You can customize the color of the console logs for easier readability if desired