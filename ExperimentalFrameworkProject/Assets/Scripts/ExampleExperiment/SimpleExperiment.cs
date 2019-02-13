using System;

public class SimpleExperiment : Experiment {
    public override Type TrialType => typeof(SimpleTrial);
    public override Type BlockType => typeof(SimpleBlock);

}