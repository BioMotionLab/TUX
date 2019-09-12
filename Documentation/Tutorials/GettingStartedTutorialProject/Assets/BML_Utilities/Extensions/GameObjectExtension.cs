using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BML_Utilities.Extensions {
    public static class GameObjectExtensions
    {

        public static void SetLayerRecursively(this GameObject gameObject, int layer) {
            gameObject.layer = layer;
            SetLayerOfChildren(gameObject.transform, layer);

        }

        static void SetLayerOfChildren(Transform baseTransform, int layer) {
            List<Transform> children = baseTransform.GetComponentsInChildren<Transform>(true)
                .Where(child => child != baseTransform.parent && child != baseTransform).ToList();
            //Debug.Log(children.Count());
            
            foreach (Transform transform in children) {
                transform.gameObject.layer = layer;
                SetLayerOfChildren(transform,layer);
            }
        }

        
    
    }
}
