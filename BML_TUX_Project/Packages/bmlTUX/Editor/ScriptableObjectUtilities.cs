using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Editor
{
    public class ScriptableObjectUtilities : MonoBehaviour
    {
        public static List<T> GetAllInstancesInProject<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            List<T> loadedAssets = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
            return loadedAssets;
        }
    }
}
