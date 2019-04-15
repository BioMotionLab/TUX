

namespace BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts {



    /// <inheritdoc />
    /// <summary>
    /// This is the simplest Runner possible, and is used when no custom Runner is specified.
    /// </summary>
    public class SimpleExperiment : Experiment {
        public SimpleExperiment(ExperimentRunner runner, ExperimentDesign design) 
            : base(runner, design) {
        }
    }
}