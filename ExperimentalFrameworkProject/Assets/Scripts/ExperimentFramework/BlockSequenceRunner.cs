using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class BlockSequenceRunner {

    Experiment experiment;

    public Config ConfigFile;

    List<Block> blocks = new List<Block>();
    Block currentlyRunningBlock;

    public BlockSequenceRunner(Experiment experiment, List<Block> blocks) {
        OnEnable();
        this.experiment = experiment;
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
            throw new InvalidDataException("Experiment blocks not created correctly");
        }

        int i = 1;
        foreach (Block block in blocks) {
            foreach (DataRow blockRow in block.table.Rows) {
                blockRow[Config.BlockIndexColumnName] = i;
            }
            i++;
        }

        Debug.Log("Starting to run Blocks");
        StartRunningBlock(blocks[0]);
    }

    void StartRunningBlock(Block block) {

        currentlyRunningBlock = block;
        Debug.Log($"Starting to run block {block.Identity}");
        ExperimentEvents.BlockHasStarted(block);

        List<Trial> blockTrials = new List<Trial>();

        int i = 1;

        //configure block index
        foreach (DataRow row in block.table.Rows) {
            row[Config.TrialIndexColumnName] = i;
            Trial newTrial = new TestTrial(row, ConfigFile);
            //Debug.Log("Adding Trial to list");
            blockTrials.Add(newTrial);
            i++;
        }

        TrialSequenceRunner trialSequenceRunner = new TrialSequenceRunner(experiment, blockTrials);
        trialSequenceRunner.Start();

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

        ExperimentEvents.UpdateBlockList(blocks, BlockIndex(currentlyRunningBlock));
    }

    void BlockDoneRunning(List<Trial> trials) {
        FinishBlock();
        GoToNextBlock();
    }

    void DoneBlockSequence() {
        Debug.Log("---------------\nDone all blocks");

        ExperimentEvents.BlockSequenceHasCompleted(blocks);

        OnDisable();
    }

    void JumpToBlock(int jumpToIndex) {
        Debug.Log("Got jump event");
        FinishBlock();
        StartRunningBlock(blocks[jumpToIndex]);
    }

    int BlockIndex(Block Block) {
        return blocks.IndexOf(Block);
    }

}

