using System;
using System.Collections;
using bmlTUX.Scripts.Managers;
using JetBrains.Annotations;
using UnityEngine;

namespace bmlTUX {
    
    /// <summary>
    /// Basic boilerplate code for Experiment > Block > Trial structure.
    /// Manages the basic functionality of these elements.
    /// </summary>
    public abstract class ExperimentPart {

        protected readonly ExperimentRunner Runner;
        protected float RunTime;
        protected bool Interrupt { get; private set; }

        protected ExperimentPart(ExperimentRunner runner) {
            Runner = runner;
            Interrupt = false;
            
            ExperimentEvents.OnStartPart += StartRunningPart;
            //TODO disable somehow. Disabling after trial completed does not allow it to restart. Perhaps do it using end of experiment event?
        }
        
        /// <summary>
        /// Start running the code that occurs before the main part of the Runner
        /// </summary>
        /// <returns></returns>
        void StartRunningPart(ExperimentPart experimentPart) {
            if (experimentPart != this) return;
            Interrupt = false;
            InternalPreMethod();
            PreMethod();
            Runner.StartCoroutine(PreCoroutineRunner(OnDonePreCoroutine));
        }
        
        IEnumerator PreCoroutineRunner(Action actionWhenDone) {
            yield return PreCoroutine();
            actionWhenDone.Invoke();
        }

        void OnDonePreCoroutine() {
            RunMainMethods();
        }


        void RunMainMethods() {
            Runner.StartCoroutine(MainCoroutineRunner(OnDoneMainCoroutine));
        }

        IEnumerator MainCoroutineRunner(Action onDoneMainCoroutine) {
            float startTime = Time.time;
            
            yield return RunMainCoroutine();
            
            float endTime = Time.time;
            RunTime = endTime - startTime;
            onDoneMainCoroutine.Invoke();
        }
        
        void OnDoneMainCoroutine() {
            Runner.StartCoroutine(PostCoroutineRunner(OnDonePostCoroutine));
        }

        IEnumerator PostCoroutineRunner(Action actionWhenDone) {
            yield return PostCoroutine();
            actionWhenDone.Invoke();
        }
        
        
        /// <summary>
        /// Start running the code that occurs after the main part of the Runner
        /// </summary>
        /// <returns></returns>
        void OnDonePostCoroutine() {
            PostMethod();
            InternalPostMethod();
        }

        [PublicAPI] protected virtual void InternalPreMethod() { }

        [PublicAPI] protected virtual void InternalPostMethod() { }

        /// <summary>
        /// Code that runs before this ExperimentPart.
        /// Overwrite this for custom behaviour.
        /// Useful for simple setup tasks that can be completed in a single frame.
        /// [Note: Called before PreCoroutine()]
        /// </summary>
        [PublicAPI] protected virtual void PreMethod() {}

        
        /// <summary>
        /// Code that runs before this ExperimentPart.
        /// Overwrite this for custom behaviour.
        /// Useful for more complex setup tasks/instructions that need to
        /// run for more than one frame.
        /// Must contain at least one "yield return" statement.
        /// [Note: Called after PreMethod()]
        /// </summary>
        /// <returns></returns>
        [PublicAPI] protected virtual IEnumerator PreCoroutine() {
            yield return null;
        }

        
        /// <summary>
        /// The main body of code that runs in the experiment part called for every part.
        /// </summary>
        /// <returns></returns>
        [PublicAPI] protected abstract IEnumerator RunMainCoroutine();

        /// <summary>
        /// Code that runs after this ExperimentPart.
        /// Overwrite this for custom behaviour.
        /// Useful for more complex cleanup tasks/instructions that need to
        /// run for more than one frame.
        /// Must contain at least one "yield return" statement.
        /// [Note: Called before PostMethod()]
        /// </summary>
        /// <returns></returns>
        [PublicAPI] protected virtual IEnumerator PostCoroutine() {
            yield return null;
        }

        /// <summary>
        /// Code that runs after each block. Overwrite this for custom behaviour.
        /// suggest doing trial cleanup and writing output to data here.
        /// Useful for simple setup tasks that can occur in a single frame.
        /// [Note: Called after PostCoroutine()]
        /// </summary>
        [PublicAPI] protected virtual void PostMethod() {}

        /// <summary>
        /// Interrupts the ExperimentPart from within
        /// </summary>
        [PublicAPI] protected void InterruptThis() {
            Interrupt = true;
        }
    }
}
