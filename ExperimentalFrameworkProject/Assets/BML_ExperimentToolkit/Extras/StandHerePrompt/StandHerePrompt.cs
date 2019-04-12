using BML_Utilities.EventSystem;
using BML_Utilities.ScriptableObject_Assets;
using UnityEngine;

namespace BML_ExperimentToolkit.Extras.StandHerePrompt {

    public class StandHerePrompt : MonoBehaviour {

        public PositionMarker    PositionMarker;
        public DestinationMarker DestinationMarker;
    
    
        public BoolValue      StandingInCorrectSpot;
        public BmlStringEvent ShowInstructionEvent;
        public BmlEvent       HideInstructionEvent;

        public StandHerePromptSettings PromptSettings;

        bool alreadyHidden = false;

        void Update() {
  
            CheckStandingSpot();

            SetColor(StandingInCorrectSpot ? PromptSettings.GoodColor : PromptSettings.BadColor);
            if (!StandingInCorrectSpot) {
                DestinationMarker.ShowMoveToText();
                ShowInstructionEvent.Raise(PromptSettings.InstructionsForMoving);
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
            return positionDeviation.magnitude < PromptSettings.StandDistanceTolerance;
        }
    }
}
