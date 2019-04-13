using UnityEngine;

namespace BML_ExperimentToolkit.Extras.StandHerePrompt {
    public class PositionMarker : MoveMarker {

        public Transform MoveDirectionIndicator;
        public DestinationMarker Destination;

        public override void RotateTextToBeReadable() {
            MainText.eulerAngles = new Vector3(0, Hmd.eulerAngles.y, 0);
        }


        void IndicateMoveDirection() {
            MoveDirectionIndicator.LookAt(Destination.transform);
            MoveDirectionIndicator.localEulerAngles = new Vector3(0, MoveDirectionIndicator.localEulerAngles.y, 0);
        }

        void CopyPositionFromHmd() {
            Vector3 hmdPosition = Hmd.position;
            Vector3 newPosition = new Vector3(hmdPosition.x, PromptSettings.PositionMarkerVerticalOffset, hmdPosition.z);
            transform.position = newPosition;
        }

        void Update() {
            CopyPositionFromHmd();
            IndicateMoveDirection();
            RotateTextToBeReadable();
        }

 

    }
}

