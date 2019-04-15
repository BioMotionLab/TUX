using UnityEngine;

namespace BML_Utilities.Extensions {


    /// <summary>
    /// Extensions to the transform object in Unity
    /// </summary>
    public static class TransformExtension {

        /// <summary>
        /// A custom extension method to Copy local position, angles, and scale from another transform
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="other">The other object to copy from</param>
        public static void CopyLocalFrom(this Transform thisTransform, Transform other) {
            thisTransform.localPosition = other.localPosition;
            thisTransform.localEulerAngles = other.localEulerAngles;
            thisTransform.localScale = other.localScale;
        }

        /// <summary>
        /// A custom extension method to copy the world position, angles, and scale from another transform
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="other">The other object to copy from</param>
        public static void CopyWorldFrom(this Transform thisTransform, Transform other) {
            thisTransform.position = other.position;
            thisTransform.eulerAngles = other.eulerAngles;
            thisTransform.localScale = other.localScale;
        }

        /// <summary>
        /// A custom extension method to set the transform's
        /// localPosition and localEuler angles back to Vector3.zero (0,0,0).
        /// </summary>
        /// <param name="thisTransform"></param>
        public static void ResetLocal(this Transform thisTransform) {
            thisTransform.localPosition = Vector3.zero;
            thisTransform.localEulerAngles = Vector3.zero;
        }

    }
}
