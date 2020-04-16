---
id: AdvancedOptions
title: Advanced Options
---
There are several advanced options accessible from the the Variable Configuration File's inspector. 

# Pre-Generated Experiment Tables

By default, the experimental toolkit creates and randomizes all trials "On the fly", that is, at the start of each experimental session. However, in some circumstances, experimenters may want to determine the exact trial order and structure prior to any experiment taking place. In such circumstances, experimenters can select the option for using Pre-Generated experiment tables. The toolkit can assist in creating such pre-generated tables by clicking the generate button. The UI will prompt the experimenter at the beginning of the session to load the appropriate design file.

# Manual Block Order Configuration

When using block variables, one must select a block order prior to each experimental session. However, if there are many values for block variables, the number of permutations can become enormous. In these cases, a warning message is displayed prompting users to manually define their possible block orders. 

To manually create a block order definition, click the Add new BlockOrderDefinition button. A new file will be created alongside your variable configuration file, and it will be automatically selected. To specify a particular block order, drag and reorder the block values shown in the inspector. To randomize the block order, click randomize. You can rename the order definition file for clarity. Make sure the order file is properly referenced in the variable configuration file before continuing.

You can define any number of block orders. Just click the Add button again to define another. At the beginning of an experimental session, the UI will prompt you to select one. 

# Advanced Settings

You can replace the default settings with your own files. For example, you can change the default names of the automatically added columns in the output file. Simply create a copy of the appropriate settings file, and rename it, and drag the copy into the correct field. 

* ColumnNames: The names of the automatically created columns in the output file.
* ControlSettings: Keyboard controls for skipping and navigating between trials [advanced use].
* GuiSettings: Can be used to swap out the in-game experiment GUI to a custom GUI.
* FileLocationSettings: Defines the location of internal data files saved to disk. 

_Note that updating the toolkit package may revert settings to default, so you may need to re-drag your custom settings into the correct fields_