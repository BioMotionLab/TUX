---
id: Coroutines
title: About Coroutines and IEnumerators
---

Normal loops in normal code keep looping until done, and don’t allow for you to extend code across multiple frames in a unity program.
For example, the following code will hang your program since it never lets the program get to the next frame.
```c#
void NormalKindOfMethod() {  
    while (true) {  
        //do something  
    }  
}  
```

What Coroutines and IEnumerators let you do is have a loop, but then pause at certain points to let the rest of the program run. Then, during the next frame of your program, it will pick up where it left off. Therefore, your program will run normally with the loop updating each frame. 
```c#
IEnumerator CoroutineMethod() {  
    while (true) {  
        //do something    
        yield return null; //continue below this after next frame  
	Debug.Log("this will print to console the next frame");  
    }  
}  
```

To begin a coroutine method, you cannot call it like a normal method. You need to use a special unity function called `StartCoroutine()`. This functionality is already written for you in the toolkit. For a coroutine function, you need only tell Unity where to pause to wait for the next frame using a `yield return null` statement. You can call normal methods inside coroutine functions, and everything else behaves as you would expect it to, with the added power of being able to have behavior that occurs over many frames.

In addition to waiting for a frame, you can have unity wait for a specified amount of time before continuing. This is great for displaying instructions or for creating time delays in your code. You can have unity display some instructions to a participant, wait for a few seconds, then stop displaying the instructions. Below is an example of displaying instructions at the start of your program for 5 seconds.
```c#
protected override IEnumerator PreCoroutine() {  
    DisplayInstructions(); //called right away  
    
    //the rest of your program will run normally while it waits.  
    yield return new WaitForSeconds(5);  
    
    //will only get called after 5 seconds  
    StopDisplayingInstructions();   
}  

void DisplayInstructions() {  
    //your code for displaying  
}  
	  
void StopDisplayingInstructions() {  
    //your code for stopping to display  
}
```

Coroutines can also yield to other coroutines. This allows you to wait for a different coroutine to finish inside another coroutine.
```c#
public override IEnumerator MainCoroutine() {
    //this code fires first
    yield return OtherCoroutineMethod();
    //this code fires only after OtherCoroutineMethod is finished.
}

public IEnumerator OtherCoroutineMethod() {
   //another coroutine
   yield return null
}
```