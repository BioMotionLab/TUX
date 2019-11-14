using JetBrains.Annotations;
using UnityEngine;

namespace bmlTUX.Scripts.Utilities.Extensions {
    public static class ProjectorExtension
    {
        
        public static bool IsLayerIgnored(this Projector projector, int layer)
        {
            return (projector.ignoreLayers & (1 << layer)) != 0;
        }


        /// <summary>
        /// Turn on the bit using an OR operation
        /// 
        /// From: https://forum.unity.com/threads/how-to-toggle-on-or-off-a-single-layer-of-the-cameras-culling-mask.340369/
        /// </summary>
        /// <param name="projector"></param>
        /// <param name="layer"></param>
        [PublicAPI]
        public static void IgnoreLayer(this Projector projector, int layer) {
            projector.ignoreLayers |= 1 << layer;
        }


        /// <summary>
        /// Turn off the bit using an AND operation with the complement of the shifted int:
        /// 
        /// From: https://forum.unity.com/threads/how-to-toggle-on-or-off-a-single-layer-of-the-cameras-culling-mask.340369/
        /// </summary>
        /// <param name="projector"></param>
        /// <param name="layer"></param>
        public static void TargetLayer(this Projector projector, int layer) {
            projector.ignoreLayers &=  ~(1 << layer);
        }

        /// <summary>
        /// Toggle the bit using a XOR operation:
        /// 
        /// From: https://forum.unity.com/threads/how-to-toggle-on-or-off-a-single-layer-of-the-cameras-culling-mask.340369/
        /// </summary>
        /// <param name="projector"></param>
        /// <param name="layer"></param>
        public static void ToggleLayerIgnore(this Projector projector, int layer) {
            projector.ignoreLayers ^= 1 << layer;
        }

        
        /// <summary>
        /// Ignores all layers (all checked)
        /// </summary>
        /// <param name="projector"></param>
        public static void IgnoreAllLayers(this Projector projector) {
            projector.ignoreLayers = -1;
        }
        
        
        
    }
}
