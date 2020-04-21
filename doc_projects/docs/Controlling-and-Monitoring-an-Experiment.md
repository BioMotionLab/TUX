# Experiment Runner window
Once the experiment’s variables have been configured, the experiment can be run from the Experiment Runner Window. You can open this window from the BML menu. This window can be docked and moved around just like any other editor window.
This is the main display and control for the toolkit. Once play mode is started, you will control and monitor the experiment from here. The window interfaces with a Custom ExperimentRunner class that you define (see below), to notify the unity scene to set up and begin the experiment. 

Press play in the unity editor. The unity scene will begin, but the experiment will not yet run. It will prompt you for relevant settings, including picking which block order to use. 

![Runner Window](https://github.com/BioMotionLab/ExperimentalFramework/blob/master/Documentation/WikiImages/runnerwindow.PNG)

When all required settings have been selected, the Experiment controls will be displayed, allowing you to begin the experiment. The experiment will show the trial structure of your experiment and track your progress through the experiment. 

# In-Trial Experimenter Controls
During the experiment you can jump between trials by pressing the “go” button next to the trial in the Experiment Runner Window. Any incomplete trials will be revisited at the end of the block. It is not currently possible to jump between blocks. This might be useful if your participant says they made an error. Additional attempts on a trial will be recorded in the output in the “Attempts” column.

![runner](https://github.com/BioMotionLab/ExperimentalFramework/blob/master/Documentation/WikiImages/runnercontrols.PNG)

The experimenter can also skip between trials using the keyboard using the following default controls:
* Navigate between trials (back, next, etc.) using the WASD keys.
* Skipped trials will be automatically repeated at the end of the current block of trials.
* Skip trial completely using the X key.
* You can modify the keys through the control settings located in the Data folder.

# Session
The session class defines one time through an experiment. It stores options that affect the whole experiment. You can set options such as debug mode for testing out your experiment, the participant’s ID, block order number and the output file. This class doesn’t need to be modified except for advanced uses.

When first pressing play, the Experiment Runner Window will prompt you for session settings when running your experiment program. By default, the session settings will display your previously-used settings.
Debug mode will simplify your session settings for testing and save your output into a debug file created in your project’s Assets/Debug/debugFile. It can also be used to change functionality in your trials for testing purposes. See the section below on Debug Mode.

You can manually name your output files, or let the toolkit do it for you based on the current date, time, and participant ID.

This is where you choose your desired block order. Or you can have it randomly select the block order for you [Not yet implemented].
