using UnityEngine;

namespace BML_Utilities {



    /// <inheritdoc />
    /// <summary>
    /// Component to set a camera's aspect ratio
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraAspect : MonoBehaviour {

        [Tooltip("square = 1")]
        public float AspectRatio;

        Camera thisCamera;

        void Start() {
            thisCamera = GetComponent<Camera>();
            thisCamera.aspect = AspectRatio;
        }




    }
}