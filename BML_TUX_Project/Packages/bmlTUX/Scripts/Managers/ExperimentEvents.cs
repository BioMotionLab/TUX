﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace bmlTUX {
    
    public static class ExperimentEvents {

        public delegate void TrialHasStartedEvent(Trial trial);

        public static event TrialHasStartedEvent OnTrialHasStarted;
       


        public static void TrialHasStarted(Trial trial) {
            OnTrialHasStarted?.Invoke(trial);
        }

        public delegate void InterruptTrialEvent();

        public static event InterruptTrialEvent OnTrialInterrupt;

        public static void InterruptTrial() {
            OnTrialInterrupt?.Invoke();
        }

        public delegate void GoBackOneTrialEvent();

        public static event GoBackOneTrialEvent OnGoBackOneTrial;

        public static void GoBackOneTrial() {
            OnGoBackOneTrial?.Invoke();
        }

        public delegate void SkipToNextTrialEvent();

        public static event SkipToNextTrialEvent OnSkipToNextTrial;

        public static void SkipToNextTrial() {
            OnSkipToNextTrial?.Invoke();
        }


        public delegate void JumpToTrialEvent(int indexToJumpTo);

        public static event JumpToTrialEvent OnJumpToTrial;

        public static void JumpToTrial(int indexToJumpTo) {
            OnJumpToTrial?.Invoke(indexToJumpTo);
        }

        public delegate void TrialHasCompletedEvent();

        public static event TrialHasCompletedEvent OnTrialHasCompleted;

        public static void TrialHasCompleted() {
            OnTrialHasCompleted?.Invoke();
        }

        public delegate void BlockHasStartedEvent(Block block);

        public static event BlockHasStartedEvent OnBlockHasStarted;

        public static void BlockHasStarted(Block block) {
            OnBlockHasStarted?.Invoke(block);
        }


        public delegate void BlockHasCompletedEvent(Block block);

        public static event BlockHasCompletedEvent OnBlockCompleted;

        public static void BlockCompleted(Block block) {
            OnBlockCompleted?.Invoke(block);
        }

        public delegate void JumpToBlockEvent(int indexToJumpTo);

        public static event JumpToBlockEvent OnJumpToBlock;

        public static void JumpToBlock(int indexToJumpTo) {
            OnJumpToBlock?.Invoke(indexToJumpTo);
        }


        public delegate void TrialSequenceHasCompletedEvent(List<Trial> trials);

        public static event TrialSequenceHasCompletedEvent OnTrialSequenceHasCompleted;

        public static void TrialSequenceHasCompleted(List<Trial> trials) {
            OnTrialSequenceHasCompleted?.Invoke(trials);
        }


        public delegate void BlockSequenceHasCompletedEvent(List<Block> blocks);

        public static event BlockSequenceHasCompletedEvent OnBlockSequenceHasCompleted;

        public static void BlockSequenceHasCompleted(List<Block> blocks) {
            OnBlockSequenceHasCompleted?.Invoke(blocks);
        }


        public delegate void TrialUpdatedEvent(List<Trial> trials, int index);

        public static event TrialUpdatedEvent OnTrialUpdated;

        public static void UpdateTrial(List<Trial> trials, int index) {
            OnTrialUpdated?.Invoke(trials, index);
        }

        public delegate void OnBlockUpdatedEvent(List<Block> blocks, int index);

        public static event OnBlockUpdatedEvent OnBlockUpdated;

        public static void UpdateBlock(List<Block> blocks, int index) {
            OnBlockUpdated?.Invoke(blocks, index);
        }

        public delegate void StartExperimentEvent(Session session);

        public static event StartExperimentEvent OnStartRunningExperiment;

        public static void StartRunningExperiment(Session session) {
            OnStartRunningExperiment?.Invoke(session);
        }

        public delegate void ExperimentStartedEvent();

        public static event ExperimentStartedEvent OnExperimentStarted;

        public static void ExperimentStarted() {
            OnExperimentStarted?.Invoke();
        }

        public delegate void InitExperimentEvent(ExperimentRunner experimentRunner);

        public static event InitExperimentEvent OnInitExperiment;

        public static void InitExperiment(ExperimentRunner experimentRunner) {

            OnInitExperiment?.Invoke(experimentRunner);
        }

        public delegate void TimeToUpdateOutputEvent(Outputtable output);

        public static event TimeToUpdateOutputEvent OnTimeToUpdateOutput;

        public static void UpdateOutput(Outputtable output) {
            OnTimeToUpdateOutput?.Invoke(output);
        }

        public delegate void OutputSuccessfullyUpdatedEvent(string filePath);

        public static event OutputSuccessfullyUpdatedEvent OnOutputSuccessfullyUpdated;

        public static void OutputSuccessfullyUpdated(string filePath) {
            OnOutputSuccessfullyUpdated?.Invoke(filePath);
        }


        public delegate void EndExperimentEvent();

        public static event EndExperimentEvent OnEndExperiment;

        public static void EndExperiment() {
            OnEndExperiment?.Invoke();
        }

        public delegate void BlockOrderChosenEvent(int blockOrderIndex);

        public static event BlockOrderChosenEvent OnBlockOrderChosen;

        public static void BlockOrderSelected(int blockOrderIndex) {
            OnBlockOrderChosen?.Invoke(blockOrderIndex);
        }

        public delegate void StartRunningExperimentPartEvent(ExperimentPart experimentPart);
        public static event StartRunningExperimentPartEvent OnStartPart;

        public static void StartPart(ExperimentPart experimentPart) {
            OnStartPart?.Invoke(experimentPart);
        }


        public delegate void TriggerStartExperimentWithoutButtonEvent(Session session);

        public static event TriggerStartExperimentWithoutButtonEvent ExperimentTriggeredWithoutButton;

        [PublicAPI]
        public static void StartExperimentFromScript(Session session)
        {
            ExperimentTriggeredWithoutButton?.Invoke(session);
            StartRunningExperiment(session);
        }
        
    }
}
