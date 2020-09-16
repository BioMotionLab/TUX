using System;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExperimentDesignFile))]
public class OldExperimentDesignFileEditor : Editor
{
    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        EditorGUILayout.HelpBox("This Design file was created using an older version. Please Update it now. You may lose some settings", MessageType.Warning);
        ExperimentDesignFile old = target as ExperimentDesignFile;
        if (old == null) throw new NullReferenceException("Can't retrieve old experiment design file");
        if (GUILayout.Button("Update now")) {
            ExperimentDesignFile2 newFile = CreateDesignFile();
            newFile.Factory.IndependentVariables = old.Factory.IndependentVariables;
            newFile.Factory.DependentVariables = old.Factory.DependentVariables;
            newFile.Factory.ParticipantVariables = old.Factory.ParticipantVariables;
            
            newFile.ControlSettings = old.ControlSettings;
            newFile.GuiSettings = old.GuiSettings;
            newFile.ColumnNamesSettings = old.ColumnNamesSettings;
            newFile.FileLocationSettings = old.FileLocationSettings;
            
            newFile.TrialRandomization = old.TrialRandomization;
            
            newFile.TrialRepetitions = old.TrialRepetitions;
            newFile.TrialPermutationType = old.TrialPermutationType;
            newFile.BlockRandomization = old.BlockRandomization;
            newFile.BlockPartialRandomizationSubType = old.BlockPartialRandomizationSubType;
            
            newFile.ExperimentRepetitions = old.ExperimentRepetitions;

        }

        EditorGUILayout.LabelField("IVs:");
        EditorGUI.indentLevel++;
        foreach (Variable variable in old.Factory.IndependentVariables) {
            EditorGUILayout.LabelField(variable.Name);
        }
        EditorGUI.indentLevel--;
        
        EditorGUILayout.LabelField("DVs:");
        EditorGUI.indentLevel++;
        foreach (Variable variable in old.Factory.DependentVariables) {
            EditorGUILayout.LabelField(variable.Name);
        }
        EditorGUI.indentLevel--;
        
        EditorGUILayout.LabelField("PVs:");
        EditorGUI.indentLevel++;
        foreach (Variable variable in old.Factory.ParticipantVariables) {
            EditorGUILayout.LabelField(variable.Name);
        }
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
    
    
    ExperimentDesignFile2 CreateDesignFile() {
        ExperimentDesignFile2 file = CreateInstance<ExperimentDesignFile2>();
        file.name = target.name + "_updated";
        AssetDatabase.CreateAsset(file, "Assets/" + file.name + ".asset");
        AssetDatabase.SaveAssets();
        return file;
    }
}
