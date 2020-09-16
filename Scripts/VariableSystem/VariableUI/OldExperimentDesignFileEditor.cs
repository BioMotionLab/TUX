using System;
using System.Collections.Generic;
using System.IO;
using bmlTUX.Scripts.VariableSystem;
using bmlTUX.Scripts.VariableSystem.VariableUI;
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

            foreach (Variable variable in newFile.Factory.ParticipantVariables) {
                ParticipantVariable participantVariable = variable as ParticipantVariable;
                participantVariable.DataType = ConvertOldDataType(participantVariable.DataType);
                participantVariable.ConvertOldValues();
            }

        }

        

        EditorGUILayout.LabelField("IVs:");
        EditorGUI.indentLevel++;
        foreach (Variable variable in old.Factory.IndependentVariables) {
            EditorGUILayout.LabelField($"{variable.Name}, type {variable.DataType}");
            IndependentVariable iv = variable as IndependentVariable;
            List<string> values = iv.ValuesAsString();
            EditorGUI.indentLevel++;
            foreach (string value in values) {
                EditorGUILayout.LabelField(value);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
        
        EditorGUILayout.LabelField("DVs:");
        EditorGUI.indentLevel++;
        foreach (Variable variable in old.Factory.DependentVariables) {
            EditorGUILayout.LabelField($"{variable.Name}, type {variable.DataType}");
        }
        EditorGUI.indentLevel--;
        
        EditorGUILayout.LabelField("PVs:");
        EditorGUI.indentLevel++;
        foreach (Variable variable in old.Factory.ParticipantVariables) {
            EditorGUILayout.LabelField($"{variable.Name}, type {ConvertOldDataType(variable.DataType)}");
        }
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }

    static SupportedDataType ConvertOldDataType(SupportedDataType supportedDataType) {
        int oldTypeIndex = (int)supportedDataType;
        SupportedDataType dataType = (SupportedDataType)(oldTypeIndex + 1);
        return dataType;
    }


    ExperimentDesignFile2 CreateDesignFile() {
        string oldPath = AssetDatabase.GetAssetPath(target);
        ExperimentDesignFile2 file = CreateInstance<ExperimentDesignFile2>();
        string fileName = Path.GetFileNameWithoutExtension(oldPath) + "_updated" + ".asset";
        string path = Path.GetDirectoryName(oldPath);
        string fullPath = Path.Combine(path, fileName);
        string uniquePath = EditorGuiHelper.GetUniqueName(fullPath);
        Debug.Log(uniquePath);
        AssetDatabase.CreateAsset(file, uniquePath);
        AssetDatabase.SaveAssets();
        return file;
    }
}
