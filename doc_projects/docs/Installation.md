---
id: Installation
title: Installation
---


There are two ways to install bmlTUX.

For both methods, you should be in a new unity project that meets the [Requirements](Requirements.md)

* Make sure you are targetting .NET 4.x in the Project Settings > Player > Other
* Make sure you have TextMeshPro version 2.1 or later installed. Earlier versions contain a bug that causes interface mis-alignments. At the time of writing, this requires a pre-release package.

## Through Unity Package Manager

1. Go to the releases page https://github.com/BioMotionLab/TUX/releases. Don't download anything, just note the number of the most recent release. It Should be in the format X.Y.Z, for example 1.0.1.
2. In a new unity project (Unity 2019.3 or later), open the Package Manager from the Window menu.
3. Back in unity, click the plus button at the top of the window, selecting "Add package from git URL"
4. Type in the following url, replacing X.Y.Z with the correct number noted above. and press "Add". 

```text
https://github.com/BioMotionLab/TUX.git#X.Y.Z
```

At first you may notice nothing happens. The Unity Package Manager provides little feedback, but behind the scenes, it should be downloading and importing. Eventually, you should see a loading bar appear.

When finished, in the Project window, expand the Packages folder, and you should see a folder called bmlTUX. You should also notice a new menu at the top of the screen.

You're all set.

## As an old-style .unitypackage

This is a bit simpler than above, but has several shortcomings. 
* It is more difficult to update to newer versions.
* It pollutes your Assets folder.
* Increases compile time.
* Easier to accidentally break things.

On the releases page https://github.com/BioMotionLab/TUX/releases, download the most recent .unitypackage bundle.

In a new unity project, from the Assets Menu, select Import Package > Custom Package. Navigate to the download.
 
After import you should be all set.
