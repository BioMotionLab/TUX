---
id: AccessingScene
title: Accessing GameObjects and Scripts in your Scene
---
The `Experiment`, `Block`, and `Trial `classes reside in the voids of C# and don’t really interact with the unity system at all. For this reason, you need to create a link between your unity project and your custom trial script.

To do this, you need to create references to them in your custom `ExperimentRunner` class. For example:
```csharp
public GameObject gameObjectThatMoves  
```

Then, in your custom `Trial `script you can reference your custom `ExperimentRunner` using the following
```csharp
MyCustomExperimentRunner myRunner = (MyCustomExperimentRunner)Runner;
```

This line converts the generic `ExperimentRunner `that is stored in the `Runner` field into your customized version of the class. 

Then you can access a `GameObject`, or other public unity field referenced within the customized `ExperimentRunner` class with:
```csharp
GameObject gameObjectInScene = myRunner.SomeReferencedGameObject;
```

This allows you to use your `ExperimentRunner` object as a container to drag and drop references to `GameObject`s and other things in your unity scene that need to be controlled or accessed during your experiment.