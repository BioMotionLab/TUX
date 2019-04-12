using BML_Utilities;
using BML_Utilities.ScriptableObject_Assets;
using UnityEngine;

namespace BML_ExperimentToolkit.Extras.StandHerePrompt {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Create StandHerePrompt PromptSettings")]
    public class StandHerePromptSettings : ScriptableObject{
        public Material    GoodColor;
        public Material    BadColor;
        public float       StandDistanceTolerance;
        public float       CloseDistance;
        public float       PositionMarkerVerticalOffset;
        public StringValue LookUpText;
        public StringValue MoveHereText;
        public StringValue InstructionsForMoving;
    }
}