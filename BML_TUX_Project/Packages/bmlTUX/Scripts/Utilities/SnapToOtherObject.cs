using UnityEngine;

namespace bmlTUX.Scripts.Utilities {

    /// <summary>
    /// CameraLocationComponentBase used to snap position to another GameObject
    /// </summary>
    public class SnapToOtherObject : MonoBehaviour {

        public bool SnapToX;
        public bool SnapToY;
        public bool SnapToZ;

        [Tooltip("Manually choose object to snap to?")]
        public bool SnapToManuallySelectedObject = false;

        [Tooltip("object to manually snap to")]
        public GameObject ManuallySelected;


        /// <summary>
        /// Snaps a GameObject to the coordinates of another gameObject
        /// For example, snap objects to the height (Y) of the VR Headset
        ///
        /// Can snap each axis separately.
        /// </summary>
        /// <param name="otherObject">The object to snap to</param>
        void SnapTo(GameObject otherObject) {
            Vector3 newPosition = transform.position;

            if (SnapToX) {
                newPosition.x = otherObject.transform.position.x;
            }

            if (SnapToY) {
                newPosition.y = otherObject.transform.position.y;
            }

            if (SnapToZ) {
                newPosition.z = otherObject.transform.position.z;
            }

            transform.position = newPosition;
        }

        void Update() {
            if (SnapToManuallySelectedObject) {
                SnapTo(ManuallySelected);
            }
        }
    }
}
