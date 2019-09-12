using UnityEngine;

namespace BML_Utilities {
    
    [ExecuteInEditMode]
    public class PlaneAspectRatioAndResizer : MonoBehaviour {

        public float Width;
        public float Height;
        public bool LockAspect;

        [Range(0, 1)]
        public float Aspect;

        void Update() {
            transform.localScale = new Vector3(Width, Height, 1);

            if (LockAspect) {
                Height = Width * Aspect;
            }
            
        }


    }

}