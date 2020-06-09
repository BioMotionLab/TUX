

namespace bmlTUX.Scripts.ExperimentParts.SimpleExperimentParts {



    /// <inheritdoc />
    /// <summary>
    /// This is the simplest Runner possible, and is used when no custom Runner is specified.
    /// </summary>
    public class SimpleExperiment : Experiment {
        public SimpleExperiment(ExperimentRunner runner, RunnableDesign design) 
            : base(runner, design) {
        }
    }
}