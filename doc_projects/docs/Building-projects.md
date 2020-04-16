---
id: Building
title: Building to an Application
---

As of version 0.8.0b, you can build your projects as executables. 

The UI requires minimum Unity 2019.2.

This UI prefab object is located in the scripts/UI folder, and can be cusomized from there.

There is a known Unity bug in v2019.2.11f where the UI system breaks when multiple monitors are of different resolution in built projects. This can be fixed by making the UI show on display 1 and making your experiment shown on display 2.

Note that building projects is different just running in the editor. The scripts and folder structure needs to be different since built projects don't have access to the editor.Any scripts that reference any aspect of the editor environment will give compile errors when trying to build. Read about runtime vs. editor script assemblies in the unity docs for more info.