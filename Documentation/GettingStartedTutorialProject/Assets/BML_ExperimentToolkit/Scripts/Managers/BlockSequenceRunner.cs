using System.Collections;
using System.Collections.Generic;
using System.IO;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.Managers {



    public class BlockSequenceRunner {

        ExperimentRunner runner;
        List<Block> blocks;

        Block currentlyRunningBlock;

        public BlockSequenceRunner(ExperimentRunner runner, List<Block> blocks) {
            OnEnable();
            this.runner = runner;
            this.blocks = blocks;
        }


        void OnEnable() {
            ExperimentEvents.OnTrialSequenceHasCompleted += BlockDoneRunning;
            ExperimentEvents.OnJumpToBlock += JumpToBlock;
        }

        void OnDisable() {
            ExperimentEvents.OnTrialSequenceHasCompleted -= BlockDoneRunning;
            ExperimentEvents.OnJumpToBlock -= JumpToBlock;
        }


        public void Start() {

            if (blocks.Count <= 0) {
                throw new InvalidDataException("Runner blocks not created correctly");
            }

            //Debug.Log("Starting to run Blocks");
            StartRunningBlock(blocks[0]);
        }


        void StartRunningBlock(Block block) {

            currentlyRunningBlock = block;
            Debug.Log($"*****\nStarting to run block: {block.Identity}");
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
            Debug.Log($"Done block {blockNum + 1}\n {currentlyRunningBlock.AsString()}");
            currentlyRunningBlock.Complete = true;
            ExperimentEvents.UpdateBlock(blocks, BlockIndex(currentlyRunningBlock));
        }

        void BlockDoneRunning(List<Trial> trials) {


            runner.StartCoroutine(RunPostBlock());

        }

        void DoneBlockSequence() {
            Debug.Log("---------------\nDone all blocks");

            ExperimentEvents.BlockSequenceHasCompleted(blocks);
            ExperimentEvents.EndExperiment();

            OnDisable();
        }

        void JumpToBlock(int jumpToIndex) {
            Debug.Log("Got jump event");
            FinishBlock();
            StartRunningBlock(blocks[jumpToIndex]);
        }

        int BlockIndex(Block block) {
            return blocks.IndexOf(block);
        }

    }
}
