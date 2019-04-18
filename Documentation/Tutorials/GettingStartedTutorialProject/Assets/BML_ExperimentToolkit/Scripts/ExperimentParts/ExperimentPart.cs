using BML_ExperimentToolkit.Scripts.Managers;
using System.Collections;
using UnityEngine;


namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public abstract class ExperimentPart {
        protected readonly ExperimentRunner runner;
        protected float RunTime;
        protected bool Interrupt { get; private set; }

        protected ExperimentPart(ExperimentRunner runner) {
            this.runner = runner;
            Interrupt = false;
            Enable();
        }

        void Enable() {
            ExperimentEvents.OnStartPart += StartPart;
        }

        void Disable() {
            ExperimentEvents.OnStartPart -= StartPart;
        }

        void StartPart(ExperimentPart experimentPart) {

            if (experimentPart != this) return;

            runner.StartCoroutine(Run());
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

        protected abstract IEnumerator MainCoroutine();


        IEnumerator Run() {
            yield return ConditionalCoroutine(RunPreMethods());

            float startTime = Time.time;

            yield return ConditionalCoroutine(MainCoroutine());

            //yield return MainCoroutine();

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


        protected virtual void InternalPreMethod() {
        }

        protected virtual void InternalPostMethod() {
        }

        /// <summary>
        /// Code that runs before this ExperimentPart.
        /// Overwrite this for custom behaviour.
        /// Useful for simple setup tasks that can be completed in a single frame.
        /// [Note: Called before PreCoroutine()]
        /// </summary>
        public virtual void PreMethod() {}

        /// <summary>
        /// Code that runs before this ExperimentPart.
        /// Overwrite this for custom behaviour.
        /// Useful for more complex setup tasks/instructions that need to
        /// run for more than one frame.
        /// Must contain at least one "yield return" statement.
        /// [Note: Called after PreMethod()]
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator PreCoroutine() {
            yield return null;
        }


        /// <summary>
        /// Code that runs after this ExperimentPart.
        /// Overwrite this for custom behaviour.
        /// Useful for more complex cleanup tasks/instructions that need to
        /// run for more than one frame.
        /// Must contain at least one "yield return" statement.
        /// [Note: Called before PostMethod()]
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator PostCoroutine() {
            yield return null;
        }

        /// <summary>
        /// Code that runs after each block. Overwrite this for custom behaviour.
        /// suggest doing trial cleanup and writing output to data here.
        /// Useful for simple setup tasks that can occur in a single frame.
        /// [Note: Called after PostCoroutine()]
        /// </summary>
        protected virtual void PostMethod() {}

        protected void InterruptThis() {
            Interrupt = true;
        }
    }
}
