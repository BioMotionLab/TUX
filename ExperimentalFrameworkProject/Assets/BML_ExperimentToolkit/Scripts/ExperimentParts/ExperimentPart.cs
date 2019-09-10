using BML_ExperimentToolkit.Scripts.Managers;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;


namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    
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
            
            ExperimentEvents.OnStartPart += StartPart;
            //TODO disable somehow. Disabling after trial completed does not allow it to restart. Perhaps do it using end of experiment event?
        }
        
        void StartPart(ExperimentPart experimentPart) {
            if (experimentPart != this) return;
            Runner.StartCoroutine(Run());
            Interrupt = false;
        }
        
        /// <summary>
        /// Start running the code that occurs before the main part of the Runner
        /// </summary>
        /// <returns></returns>
        IEnumerator RunPreMethods() {
            yield return null; // let last frame finish before starting
            InternalPreMethod();
            PreMethod();
            yield return PreCoroutine();
        }



        IEnumerator Run() {
            yield return ConditionalCoroutine(RunPreMethods());

            float startTime = Time.time;

            yield return ConditionalCoroutine(RunMainCoroutine());

            float endTime = Time.time;
            RunTime = endTime - startTime;

            yield return ConditionalCoroutine(RunPostMethods());
        }

        IEnumerator ConditionalCoroutine(IEnumerator coroutine) {
            while (coroutine.MoveNext()) {
                if (Interrupt) {
                    Debug.LogWarning($"Interrupted {nameof(coroutine)}");
                    break;
                }
                yield return coroutine.Current;
            }
        }

        /// <summary>
        /// Start running the code that occurs after the main part of the Runner
        /// </summary>
        /// <returns></returns>
        IEnumerator RunPostMethods() {
            yield return null; // let last frame finish before starting
            yield return PostCoroutine();
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
