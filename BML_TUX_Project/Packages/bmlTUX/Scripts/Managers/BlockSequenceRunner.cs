﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace bmlTUX {



    public class BlockSequenceRunner {
        readonly ExperimentRunner runner;
        readonly List<Block> blocks;

        Block currentlyRunningBlock;
        public bool Running = false;

        public BlockSequenceRunner(ExperimentRunner runner, List<Block> blocks) {
            OnEnable();
            this.runner = runner;
            this.blocks = blocks;
        }


        void OnEnable() {
            ExperimentEvents.OnBlockCompleted += BlockDoneRunning;
            ExperimentEvents.OnJumpToBlock += JumpToBlock;
        }

        void OnDisable() {
            ExperimentEvents.OnBlockCompleted -= BlockDoneRunning;
            ExperimentEvents.OnJumpToBlock -= JumpToBlock;
        }


        public void Start() {

            if (blocks.Count <= 0) {
                throw new InvalidDataException("Runner blocks not created correctly");
            }

            Running = true;
            StartRunningBlock(blocks[0]);
        }


        void StartRunningBlock(Block block) {

            currentlyRunningBlock = block;
            Debug.Log("");
            TuxLog.Log($"{TuxLog.Warn("Starting")} Block {BlockIndex(currentlyRunningBlock)+1} / {blocks.Count}");
            ExperimentEvents.BlockHasStarted(block);
            ExperimentEvents.StartPart(block);

        }

        IEnumerator RunPostBlock() {
            
            FinishBlock();
            GoToNextBlock();
            yield return null;
        }



        void GoToNextBlock() {
            int newIndex = BlockIndex(currentlyRunningBlock) + 1;

            if (newIndex > blocks.Count - 1) {
                DoneBlockSequence();
            }
            else {
                Block nextBlock = blocks[newIndex];
                StartRunningBlock(nextBlock);
            }
        }

        void FinishBlock() {
            int blockNum = BlockIndex(currentlyRunningBlock);
            Debug.Log("");
            Debug.Log($"{TuxLog.Warn("Finished")} Block {blockNum + 1}\n {currentlyRunningBlock.AsString()}");
            currentlyRunningBlock.Complete = true;
            ExperimentEvents.UpdateBlock(blocks, BlockIndex(currentlyRunningBlock));
        }

        void BlockDoneRunning(Block unused) {
            runner.StartCoroutine(RunPostBlock());
        }

        void DoneBlockSequence()
        {
            Debug.Log($"{TuxLog.Warn("Experiment Complete!")}");
            ExperimentEvents.BlockSequenceHasCompleted(blocks);
            ExperimentEvents.EndExperiment();
            Running = false;
            OnDisable();
        }

        void JumpToBlock(int jumpToIndex) {
            Debug.Log($"{TuxLog.Prefix} Got jump event");
            FinishBlock();
            StartRunningBlock(blocks[jumpToIndex]);
        }

        int BlockIndex(Block block) {
            return blocks.IndexOf(block);
        }

    }
}
