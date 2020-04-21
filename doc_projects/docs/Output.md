---
id: Output
title: Output
---
By default, the toolkit outputs your experiment results as a .CSV file after the completion of every trial. It automatically numbers your trials by block and by trial number inside each block. It adds a column for each variable including your dependent variables. You can change the names of the automatically added columns in the Data Folder under Settings.
To add a custom column, create a dependent variable of the appropriate type, and set its value in each trial.

The best place to write data to your dependent variables is in the `PostMethod()` method of your custom `Trial` script. See the [documentation for the `Trial` class](Trial-Class) for more information on writing data to dependent variables. 

