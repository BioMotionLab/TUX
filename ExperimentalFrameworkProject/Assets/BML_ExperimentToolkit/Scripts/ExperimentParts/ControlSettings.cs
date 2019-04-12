using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Create Control Settings Asset")]
    public class ControlSettings : ScriptableObject {
        public KeyCode InterruptKey;
        public KeyCode BackKey;
        public KeyCode NextKey;
    }
}