using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Extras.StandHerePrompt {

    public class StandHerePrompt : MonoBehaviour {

        public PositionMarker    PositionMarker;
        public DestinationMarker DestinationMarker;
    
    
        public BoolValue      StandingInCorrectSpot;
        public BmlStringEvent ShowInstructionEvent;
        public BmlEvent       HideInstructionEvent;

        public StandHereSettings Settings;

        bool alreadyHidden = false;

        void Update() {
  
            CheckStandingSpot();

            SetColor(StandingInCorrectSpot ? Settings.GoodColor : Settings.BadColor);
            if (!StandingInCorrectSpot) {
                DestinationMarker.ShowMoveToText();
                ShowInstructionEvent.Raise(Settings.InstructionsForMoving);
                alreadyHidden = false;
            }
            else {
                DestinationMarker.ShowOkText();
                if (!alreadyHidden) {
                    HideInstructionEvent.Raise();
                    alreadyHidden = true;
                }
            
            }
        }
    

        void SetColor(Material material) {
            DestinationMarker.SetMaterial(material);
            PositionMarker.SetMaterial(material);
        }

        void CheckStandingSpot() {
            StandingInCorrectSpot.Value = IsPositionCorrect() ;
        }

    
        bool IsPositionCorrect() {
            Vector3 positionDeviation = PositionMarker.transform.position - DestinationMarker.transform.position;
            return positionDeviation.magnitude < Settings.StandDistanceTolerance;
        }
    }
}
