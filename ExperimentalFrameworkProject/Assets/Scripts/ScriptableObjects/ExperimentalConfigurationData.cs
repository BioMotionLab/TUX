using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine.Serialization;

/// <summary>
/// Data container to store permutations of variables for each trial
/// </summary>
[Serializable]
public class ExperimentalConfigurationData : ScriptableObject {

    public IndependentVariableFactory fact = new IndependentVariableFactory();

    //public List<Variable> IndependentVariables = new List<Variable>();
    //public IndependentVariableFloat FloatLevels = new IndependentVariableFloat();
    //public IndependentVariableInt IntLevels = new IndependentVariableInt();


    //[Header("Debug settings")] [Tooltip("Turn on debug mode")]
    //public bool DebugMode;
    //public bool DebugWithNoVr;


    
}






#if UNITY_EDITOR
/// <summary>
/// Creates a menu item in editor to create a ExperimentalConfigurationData Object
/// Menu item under:
/// Assets > Create > ExperimentalConfigurationData
/// </summary>
public class CreateExperimentalSetupData {
	[MenuItem ("Assets/Create/ExperimentalConfigurationData")]
	public static ExperimentalConfigurationData  Create () {
		ExperimentalConfigurationData asset = ScriptableObject.CreateInstance<ExperimentalConfigurationData> ();

		AssetDatabase.CreateAsset (asset, "Assets/ExperimentalConfigurationData.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}

#endif