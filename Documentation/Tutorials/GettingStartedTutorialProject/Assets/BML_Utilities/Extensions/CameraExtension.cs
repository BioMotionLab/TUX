using JetBrains.Annotations;
using UnityEngine;

namespace BML_Utilities.Extensions {
    public static class CameraExtension
    {
        
        /// <summary>
        ///
        /// Checks if the layers is rendered (checked).
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        [PublicAPI]
        public static bool IsLayerRendered(this Camera camera, int layer)
        {
            return (camera.cullingMask & (1 << layer)) != 0;
        }


        
        /// <summary>
        /// Turn on the bit using an OR operation
        ///
        /// From: https://forum.unity.com/threads/how-to-toggle-on-or-off-a-single-layer-of-the-cameras-culling-mask.340369/
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="layer"></param>
        [PublicAPI] 
        public static void RenderLayer(this Camera camera, int layer) {
            camera.cullingMask |= 1 << layer;
        }
        
        
        /// <summary>
        /// Turn off the bit using an AND operation with the complement of the shifted int:
        ///
        /// From: https://forum.unity.com/threads/how-to-toggle-on-or-off-a-single-layer-of-the-cameras-culling-mask.340369/
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="layer"></param>
        [PublicAPI]
        public static void CullLayer(this Camera camera, int layer) {
            camera.cullingMask &=  ~(1 << layer);
        }

        
        /// <summary>
        /// Toggle the bit using a XOR operation:
        ///
        /// From: https://forum.unity.com/threads/how-to-toggle-on-or-off-a-single-layer-of-the-cameras-culling-mask.340369/
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="layer"></param>
        [PublicAPI] 
        public static void ToggleLayerCull(this Camera camera, int layer) {
            camera.cullingMask ^= 1 << layer;
        }

        /// <summary>
        /// Renders all layers in a culling mask. Effectively sets it to "everything"
        /// </summary>
        /// <param name="camera"></param>
        [PublicAPI]
        public static void RenderAllLayers(this Camera camera) {
            camera.cullingMask = -1;
        }
        
        /// <summary>
        /// Renders no layers in a culling mask. Effectively sets it to "nothing"
        /// </summary>
        /// <param name="camera"></param>
        [PublicAPI]
        public static void RenderNoLayers(this Camera camera) {
            camera.cullingMask = 0;
        }
        
        
        
        
    }
}
