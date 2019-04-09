using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Extras.StandHerePrompt {
    public class DestinationMarker : MoveMarker {

        public PositionMarker PositionMarker;
        public TextAssetToUi  StandHereText;

        public override void RotateTextToBeReadable() {

            float distanceFromDestination = (transform.position - PositionMarker.transform.position).magnitude;

            if (distanceFromDestination < PromptSettings.CloseDistance) {
                MainText.eulerAngles = new Vector3(0, Hmd.eulerAngles.y, 0);
            }
            else {
                MainText.LookAt(Hmd);
                MainText.eulerAngles = new Vector3(0, 180 + MainText.eulerAngles.y, 0);
            }

        }

        public void ShowMoveToText() {
            StandHereText.Text = PromptSettings.MoveHereText;
        
        }

        public void ShowOkText() {
            StandHereText.Text = PromptSettings.LookUpText;
        }

    }
}