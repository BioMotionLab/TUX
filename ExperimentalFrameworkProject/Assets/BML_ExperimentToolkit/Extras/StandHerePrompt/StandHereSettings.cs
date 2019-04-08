using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Extras.StandHerePrompt {
    [CreateAssetMenu]
    public class StandHereSettings : ScriptableObject{
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