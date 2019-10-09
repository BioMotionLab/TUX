using UnityEngine;

namespace BML_Utilities {



    /// <summary>
    /// CameraLocationComponentBase to set a camera's aspect ratio
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class CameraAspect : MonoBehaviour {

        public float Aspect = 1;

        private Camera thisCamera;

        void Start() {
            thisCamera = GetComponent<Camera>();
            thisCamera.aspect = Aspect;
        }




    }
}