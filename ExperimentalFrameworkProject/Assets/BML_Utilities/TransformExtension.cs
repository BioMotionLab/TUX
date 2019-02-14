using UnityEngine;

namespace BML_Utilities {


    /// <summary>
    /// Extensions to the transform object in Unity
    /// </summary>
    public static class TransformExtension {

        /// <summary>
        /// Copies local position, angles, and scale from another object
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="other">The other object to copy from</param>
        public static void CopyFromLocal(this Transform thisTransform, Transform other) {
            thisTransform.localPosition = other.localPosition;
            thisTransform.localEulerAngles = other.localEulerAngles;
            thisTransform.localScale = other.localScale;
        }

        /// <summary>
        /// Copies world position, angles and scale from another object
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="other">The other object to copy from</param>
        public static void CopyFromWorld(this Transform thisTransform, Transform other) {
            thisTransform.position = other.position;
            thisTransform.eulerAngles = other.eulerAngles;
            thisTransform.localScale = other.localScale;
        }



    }
}
