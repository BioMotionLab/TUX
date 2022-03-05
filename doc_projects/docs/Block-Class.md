---
id: BlockClass
title: Block Class
---
If you flag any of your independent variables as blocking variables, the toolkit will automatically create blocks for you, and run them without any customization required. However, If you’d like to customize what happens at the start or end of a block, you can override its `ExperimentPart` methods. First you’ll need to create a script that inherits from `Block`:

## Automatically generating Block Scripts

To define the behavior that occurs before or after a block, you need to create a script that inherits from the `Block` type". The toolkit can automatically generate a `Block` script for you using a template from the "Script Helper Tool" located in the main "bmlTUX" menu.

## Manually creating a Trial Script

You can also create this script manually:

```csharp
using bmlTUX;
	
public class MyBlock : Block { }  
```

However, similar to `Trial`, this will give you an error. In most editors you can automatically solve this by right-clicking on `MyBlock` and selecting “implement missing members”. This creates a constructor. All it does is forward construction up to the main `Block` class (`base`), so you don’t have to worry about it. 

To customize behavior in your `Block`, overwrite the `ExperimentPart` methods as desired.
As a rule of thumb, it’s a good idea to follow the following structure:

1. `PreMethod()`
    * Cast the runner object to your custom type, store in field so it can be used below.
    * Access your independent variables to set up the block (see below for more info)
    * Set up your environment for each block.

2. `PreCoroutine()`
    * Show block-specific instructions.
    * Wait for user to do something before Block starts.
    * Remember to yield return. (see coroutine section in this guide)

3. `MainCoroutine()`
    * No customization allowed (runs trials in block automatically)

4. `PostCoroutine()`
    * Wait for user to do something after the block.
    * Remember to yield return. (see coroutine section in this guide)

5. `PostMethod()`
    * Reset everything in preparation for next block.

For example, in one of virtual reality my experiments, I had a blocking variables to manipulate where participants stood in the room. So, before each block I gave them instructions to move to a location, and only allowed the program to continue if they were standing in the correct location.

Remember that the code defined in your custom `Block` class is general for all blocks, but the behavior changes based on the values of your independent variables. Therefore, you only need to code the behavior for all blocks in one place and set up each one based on the values of your variables for that block.

## Access variables within a block

Accessing the values of your variables is easy. Their values are stored in a field of the Block class called “data”. This “data” object will be updated and set to the correct values for that block automatically by the toolkit. **Important**: Only independent variables flagged as “block” will be accessible within `Block` scripts, and an error will be given otherwise.

Let’s say you defined an integer-type independent block variable named `MyIntBlockVariable` with values 1 and 2. To access it from a Block, write the following: 

```csharp
int blockVariable = (int) Data[“MyIntBlockVariable”] 
```

This stores each block’s value for that variable into an int called `blockVariable` which can be manipulated normally like any other C# variable. You need the `(int)` at the start to remind C# that your variable is type int. _Note: This code should probably be in the PreMethod() since it has to do with setup._

Now you can modify things for each block based on the value. For example, you could move an object based on its value:

```csharp
Vector3 positionToMoveTo = new Vector3(blockVariable, 0, 0);  
someGameObject.transform.localPosition = postionToMoveTo;  
```

Now, each block will move `someGameObject` to the correct position for that block.

Note: See the section on [Accessing Unity Components and MonoBehaviours from inside your custom experiment/block/trial scripts](Accessing-GameObjects-and-Scripts-in-your-Scene) For more information on how to customize your Blocks.
