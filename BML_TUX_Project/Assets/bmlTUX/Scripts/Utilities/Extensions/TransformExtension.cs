using JetBrains.Annotations;
using UnityEngine;

namespace bmlTUX.Scripts.Utilities.Extensions {


    /// <summary>
    /// Extensions to the transform object in Unity
    /// </summary>
    public static class TransformExtension {

        /// <summary>
        /// A custom extension method to Copy local position, angles, and scale from another transform
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="other">The other object to copy from</param>
        [PublicAPI]
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
        [PublicAPI]
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
        
        [PublicAPI]
        public static void ResetLocal(this Transform thisTransform) {
            thisTransform.localPosition = Vector3.zero;
            thisTransform.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// Sets the parent of the transform, while resetting its
        /// localPosition and localEuler angles back to Vector3.zero (0,0,0) in the new local space
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="parent"></param>
        [PublicAPI]
        public static void SetParentAndReset(this Transform thisTransform, Transform parent) {
            thisTransform.SetParent(parent);
            thisTransform.ResetLocal();
        }
        
        /// <summary>
        /// Converts a world rotation to the transform's local space.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="worldRotationToConvert"></param>
        /// <returns></returns>
        [PublicAPI]
        public static Quaternion ConvertWorldRotationToThisLocalSpace(this Transform target, Quaternion worldRotationToConvert) {
            return Quaternion.Inverse(target.rotation) * worldRotationToConvert;
        }

        /// <summary>
        /// Converts a rotation in the transform's local space to world space
        /// </summary>
        /// <param name="target"></param>
        /// <param name="localRotationToConvert"></param>
        /// <returns></returns>
        [PublicAPI]
        public static Quaternion ConvertLocalRotationToWorldSpace(this Transform target, Quaternion localRotationToConvert) {
            return target.rotation * localRotationToConvert;
        }

        public static Vector3 ConvertPositionToParentLocalSpace(this Transform target) {
            Transform parent = target.parent; // Cached for optimization
            Vector3 targetPosition = target.position; // Cached for optimization
            
            return parent != null ? 
                parent.InverseTransformPoint(targetPosition) : 
                targetPosition;
        }

        public static Vector3 ConvertPositionInParentLocalSpaceToWorld(this Transform target, Vector3 position) {
            Transform parent = target.parent; // Cached for optimization
            return parent != null ? 
                parent.TransformPoint(position) : 
                position;
        }
        
    }
}
