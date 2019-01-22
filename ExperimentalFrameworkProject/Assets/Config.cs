using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Config : ScriptableObject
{
    [SerializeField]
    public DatumFactory Factory = new DatumFactory();

    public void PrintTrials() {
        throw new System.NotImplementedException();
    }
}


public class MakeScriptableObject {
    [MenuItem("Assets/Create/Config File")]
    public static void CreateMyAsset() {
        Config asset = ScriptableObject.CreateInstance<Config>();

        AssetDatabase.CreateAsset(asset, "Assets/New Config File.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}