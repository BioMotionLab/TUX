using UnityEngine;

namespace BML_Utilities {
    
    [ExecuteInEditMode]
    public class PlaneShaper : MonoBehaviour {

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


        /// <summary>
        /// The Diagonal size of the AlbertiCanvasMount
        /// </summary>
        public float DiagonalSize {
            get {
                Vector2 diagonalVector = new Vector2(Width, Height);
                float diagonalDistance = diagonalVector.magnitude;
                return Mathf.Abs(diagonalDistance);
            }
        }

        
    }

}