---
id: AdvancedOptions
title: Advanced Options
---
There are several advanced options accessible from the the Variable Configuration File's inspector. 


# Manual Block Order Configuration

When using block variables, one must select a block order prior to each experimental session. However, if there are many values for block variables, the number of permutations can become enormous. In these cases, a warning message is displayed prompting users to manually define their possible block orders. 

To manually edit block order definitions, look at the Advanced section of the ExperimentDesignFile inspector.  

Clicking "Create and add new file" will create a new Block Order Definition File alongside your Design file, and it will be automatically selected for editing. 

To edit this Block Order Definition File, click on it, and look at the inspector. The order can be changed by dragging and reorder the block values shown in the inspector. The order will run top to bottom. Clicking "randomize" will create a random block order at runtime at the start of an experimental session.

At the beginning of an experimental session, the UI will prompt you to select one of the block order definitions from the list. 

Some tips:
* You should rename the order definition file for clarity. 
* You can define any number of block orders. 


# Customized Settings

You can replace the default settings with your own files. For example, you can change the default names of the automatically added columns in the output file. Simply create a copy of the appropriate settings file in your assets folder, rename it, and drag then drag this copy into the correct field in the Design File's inspector. 

* ColumnNames: The names of the automatically created columns in the output file.
* ControlSettings: Keyboard controls for skipping and navigating between trials [advanced use].
* GuiSettings: Can be used to swap out the in-game experiment GUI to a custom GUI.
* FileLocationSettings: Defines the location of internal data files saved to disk. 


# Pre-Generated Experiment Tables

By default, the experimental toolkit creates and randomizes all trials "On the fly", that is, at the start of each experimental session. However, in some circumstances, experimenters may want to determine the exact trial order and structure prior to any experiment taking place. In such circumstances, experimenters can select the option for using Pre-Generated experiment tables. The toolkit can assist in creating such pre-generated tables by clicking the generate button. The UI will prompt the experimenter at the beginning of the session to load the appropriate design file.

