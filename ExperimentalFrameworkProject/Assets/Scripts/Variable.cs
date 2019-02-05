using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class Variable {
    public string Name;

    public VariableType TypeOfVariable;
    public SupportedDataTypes DataType;
    public abstract Type               Type           { get; }
}

[Serializable]
public abstract class IndependentVariable : Variable {
    public          VariableMixingType MixingTypeOfVariable;
    public          bool               Block;
}

[Serializable]
public abstract class DependentVariable : Variable {
}

[Serializable]
public abstract class IndependentVariable<T> : IndependentVariable {

    public override Type Type => typeof(T);
    
    [SerializeField]
    public List<T> Values;

    [SerializeField]
    public List<float> Probabilities; 

    protected IndependentVariable() {
        Values = new List<T>();
        Probabilities = new List<float>();
        Name = $"Unnamed Variable (types:{typeof(T)})";
        TypeOfVariable = VariableType.Independent;
    }

    

   //public virtual string ValueAsString()

}

[Serializable]
public abstract class DependentVariable<T> : DependentVariable {
    public          T            Value;
    public override Type         Type           => typeof(T);

    protected DependentVariable() {
        Name = $"Unnamed DependentVariable Variable (type:{typeof(T)})";
        TypeOfVariable = VariableType.Dependent;
    }
}

[Serializable]
public enum VariableType {
    Independent,
    Dependent,
    ChooseType
}

[Serializable]
public enum SupportedDataTypes {
    Int,
    Float,
    String,
    //GameObject,
    //Vector3,
    //Vector2,
    //CustomDatum,
    ChooseType,
}

[Serializable]
public enum VariableMixingType {
    Balanced,
    Looped,
    EvenProbability,
    CustomProbability
}

[Serializable]
public class VariableFactory {

    [SerializeField]
    public SupportedDataTypes DataTypesToCreate;

    [SerializeField]
    public VariableType       VariableTypeToCreate;


    [SerializeField]
    public List<IndependentVariableInt> intIVs = new List<IndependentVariableInt>();
    [SerializeField]
    public List<DependentVariableInt> intDVs = new List<DependentVariableInt>();


    [SerializeField]
    public List<IndependentVariableFloat> floatIVs = new List<IndependentVariableFloat>();

    [SerializeField] [HideInInspector]
    public List<IndependentVariableString> stringIVs = new List<IndependentVariableString>();

    

    //[SerializeField]
    //public List<DatumGameObject> GameObjectData = new List<DatumGameObject>();

    //[SerializeField]
    //public List<DatumVector3> Vector3Data = new List<DatumVector3>();

    //[SerializeField]
    //public List<DatumCustom> CustomData = new List<DatumCustom>();

    public List<Variable> AllVariables {
        get {
            List<Variable> variables = new List<Variable>();
            foreach (var variable in intIVs) {
                variables.Add(variable);
            }

            foreach (var variable in floatIVs) {
                variables.Add(variable);
            }

            //TODO add others
            return variables;
        }
    }

    public void AddNew() {
        switch (VariableTypeToCreate) {
            case VariableType.Independent: {


                    switch (DataTypesToCreate) {
                        case SupportedDataTypes.Int:
                            IndependentVariableInt ivInt = new IndependentVariableInt();
                            intIVs.Add(ivInt);
                            break;
                        case SupportedDataTypes.Float:
                            IndependentVariableFloat ivFloat = new IndependentVariableFloat();
                            floatIVs.Add(ivFloat);
                            break;
                        //case SupportedDataTypes.String:
                        //    stringData.Add(new IndependentVariableString());
                        //    break;
                        //case SupportedDataTypes.GameObject:
                        //    GameObjectData.Add(new DatumGameObject());
                        //    break;
                        //case SupportedDataTypes.Vector3:
                        //    Vector3Data.Add(new DatumVector3());
                        //    break;
                        //case SupportedDataTypes.Vector2:
                        //    Vector2Data.Add(new DatumVector2());
                        //    break;
                        //case SupportedDataTypes.CustomDatum:
                        //    CustomData.Add(new DatumCustom());
                        //    break;
                        case SupportedDataTypes.ChooseType:
                            throw new InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
                            
                        case SupportedDataTypes.String:
                            throw new NotImplementedException();
                        default:
                            throw new NotImplementedException("Support for this data types has not yet been defined." +
                                                              "You can customize it yourself in the IndependentVariable.cs class");
                    }
                }
                break;

            case VariableType.Dependent:

                switch (DataTypesToCreate) {
                    case SupportedDataTypes.Int:
                        DependentVariableInt newDependentVariableInt = new DependentVariableInt();
                        intDVs.Add(newDependentVariableInt);
                        break;
                    //case SupportedDataTypes.Float:
                    //    IndependentVariableFloat newIndependentVariableFloat = new IndependentVariableFloat();
                    //    return newIndependentVariableFloat;
                    //case SupportedDataTypes.String:
                    //    stringData.Add(new IndependentVariableString());
                    //    break;
                    //case SupportedDataTypes.GameObject:
                    //    GameObjectData.Add(new DatumGameObject());
                    //    break;
                    //case SupportedDataTypes.Vector3:
                    //    Vector3Data.Add(new DatumVector3());
                    //    break;
                    //case SupportedDataTypes.Vector2:
                    //    Vector2Data.Add(new DatumVector2());
                    //    break;
                    //case SupportedDataTypes.CustomDatum:
                    //    CustomData.Add(new DatumCustom());
                    //    break;
                    case SupportedDataTypes.ChooseType:
                        throw new InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
                    case SupportedDataTypes.Float:
                        break;
                    case SupportedDataTypes.String:
                        break;
                    default:
                        throw new NotImplementedException("Support for this data types has not yet been defined." +
                                                          "You can customize it yourself in the IndependentVariable.cs class");
                }

                break;
            case VariableType.ChooseType:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(VariableTypeToCreate), DataTypesToCreate, null);
        }

    }

    public DataTable ToTable(bool shuffleTrialOrder, int numberRepetitions) {
        Debug.Log($"ToTable method in IndependentVariable: Alldata.count {AllVariables.Count}");
        return ExperimentTable.GetTable(AllVariables, shuffleTrialOrder, numberRepetitions);
    }

   
}

//INT
[Serializable]
public class IndependentVariableInt : IndependentVariable<int> {
    public IndependentVariableInt() {
        DataType = SupportedDataTypes.Int;
    }
}
[CustomPropertyDrawer(typeof(IndependentVariableInt))]
public class IndependentVariableIntDrawer : IndependentVariableDrawer { }


[Serializable]
public class DependentVariableInt : DependentVariable<int> {
    public DependentVariableInt() {
        DataType = SupportedDataTypes.Int;
    }
}


//FLOAT

[Serializable]
public class IndependentVariableFloat : IndependentVariable<float> {
    public IndependentVariableFloat() {
        DataType = SupportedDataTypes.Float;
    }
}
[CustomPropertyDrawer(typeof(IndependentVariableFloat))]
public class IndependentVariableFloatDrawer : IndependentVariableDrawer { }


public class IndependentVariableDrawer : PropertyDrawer {

    const float LineHeight = 20f;
    public float customPropertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float propertyHeight = EditorGUI.GetPropertyHeight(property, GUIContent.none);
        propertyHeight += customPropertyHeight;
        return propertyHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        customPropertyHeight = VariableDrawerHelpers.AddAllIndependentVariableProperties(position, property); 
    }

}


public static class VariableDrawerHelpers {

    const float LineHeight = 20f;

    public static float AddAllIndependentVariableProperties(Rect position, SerializedProperty property) {
        property.serializedObject.Update();
        Rect currentRect = new Rect(position.x, position.y + LineHeight, position.width, LineHeight);
        float oldY = currentRect.y;

        int oldIndentLevel = EditorGUI.indentLevel;
        currentRect = VariableDrawerHelpers.AddVariableProperties(property, currentRect);
        currentRect = VariableDrawerHelpers.AddIndependentVariableProperties(property, currentRect);
        currentRect = VariableDrawerHelpers.AddIndependentVariableValueProperties(property, currentRect);
        EditorGUI.indentLevel = oldIndentLevel;
        float propertyHeight = currentRect.y - oldY;
        property.serializedObject.ApplyModifiedProperties();
        return propertyHeight;
    }

    public static Rect AddVariableProperties(SerializedProperty property, Rect currentRect) {

        const float typeWidth = 100f;
        const float nameWidth = 200f;
        const float namePad = 30f;

        Rect nameRect = new Rect(currentRect.x, currentRect.y, nameWidth, currentRect.height);
        var name = property.FindPropertyRelative(nameof(Variable.Name));
        EditorGUI.PropertyField(nameRect, name, GUIContent.none);

        Rect dataTypeRect = new Rect(nameWidth + namePad, currentRect.y, typeWidth, currentRect.height);
        var variableDataType = property.FindPropertyRelative(nameof(Variable.DataType));
        SupportedDataTypes dataType = (SupportedDataTypes)variableDataType.enumValueIndex;
        EditorGUI.LabelField(dataTypeRect, $"Data Type: {dataType.ToString()}");

        currentRect.y += LineHeight;
        
        EditorGUI.indentLevel++;

        var variableType = property.FindPropertyRelative(nameof(Variable.TypeOfVariable));
        VariableType varType = (VariableType)variableType.enumValueIndex;
        EditorGUI.LabelField(currentRect, $"Variable Type: {varType.ToString()}");
        currentRect.y += LineHeight;
        return currentRect;
    }

    public static Rect AddIndependentVariableProperties(SerializedProperty property, Rect currentRect) {
        var block = property.FindPropertyRelative(nameof(IndependentVariable.Block));
        EditorGUI.PropertyField(currentRect, block);
        currentRect.y += LineHeight;

        var mixType = property.FindPropertyRelative(nameof(IndependentVariable.MixingTypeOfVariable));
        EditorGUI.PropertyField(currentRect, mixType);
        currentRect.y += LineHeight;
        return currentRect;
    }

    public static Rect AddIndependentVariableValueProperties(SerializedProperty property, Rect currentRect) {
        var valuesProperty = property.FindPropertyRelative("Values");
        var probabilitiesProperty = property.FindPropertyRelative("Probabilities");


        EditorGUI.LabelField(currentRect, "Values");

        float indentAmt = 40f;
        float minusWidth = 20f;
        float minusHeight = 14f;
        float customProbsWidth = 180f;
        float x = indentAmt + currentRect.x;
        float ypad = (LineHeight - minusHeight) / 2;

        //Debug.Log($"enum value : {mixType.enumValueIndex} {(VariableMixingType)mixType.enumValueIndex}");

        float probValuesWidth = 0;
        var mixType = property.FindPropertyRelative(nameof(IndependentVariable.MixingTypeOfVariable));
        bool customProb = (VariableMixingType)mixType.enumValueIndex == VariableMixingType.CustomProbability;
        if (customProb) {
            //Debug.Log("custom probs");
            probValuesWidth = customProbsWidth;
            Rect probLabel = new Rect(currentRect.width - probValuesWidth, currentRect.y, probValuesWidth,
                                      currentRect.height);
            EditorGUI.LabelField(probLabel, "Probability");
        }

        currentRect.y += LineHeight;

        for (int i = 0; i < valuesProperty.arraySize; i++) {
            Rect minusRect = new Rect(x, currentRect.y + ypad, minusWidth, minusHeight);
            Rect valuesRect = new Rect(x + minusWidth, currentRect.y, currentRect.width - x - minusWidth - probValuesWidth,
                                       currentRect.height);
            Rect customProbsValuesRect = new Rect(currentRect.width - probValuesWidth, currentRect.y, probValuesWidth,
                                                  currentRect.height);

            //Minus button
            if (GUI.Button(minusRect, "-")) {
                valuesProperty.DeleteArrayElementAtIndex(i);
                probabilitiesProperty.DeleteArrayElementAtIndex(i);
                //Debug.Log($"Deleted element. Size now {valuesProperty.arraySize}");
                break;
            }

            SerializedProperty value = valuesProperty.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(valuesRect, value, GUIContent.none);

            SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);
            if (prob != null && probabilitiesProperty.arraySize >= 2) {
                if (i == valuesProperty.arraySize - 1) {
                    float runningTotalWithoutLast = GetRunningTotal(probabilitiesProperty, true);
                    float remainder = 1 - runningTotalWithoutLast;
                    if (remainder >= 0) {
                        prob.floatValue = remainder;
                    }
                    else {
                        prob.floatValue = 0;
                    }

                    EditorGUI.LabelField(customProbsValuesRect, prob.floatValue + " (Auto)");
                }
                else {
                    EditorGUI.PropertyField(customProbsValuesRect, prob, GUIContent.none);
                }
            }

            currentRect.y += LineHeight;
        }

        Rect plusRect = new Rect(x, currentRect.y + ypad, minusWidth, minusHeight);
        //plus button
        if (GUI.Button(plusRect, "+")) {
            int lastIndex = valuesProperty.arraySize;
            if (lastIndex < 0) lastIndex = 0;
            probabilitiesProperty.InsertArrayElementAtIndex(lastIndex);
            valuesProperty.InsertArrayElementAtIndex(lastIndex);

            //make last one input equal to zero;
            if (probabilitiesProperty.arraySize > 1)
                probabilitiesProperty.GetArrayElementAtIndex(probabilitiesProperty.arraySize - 2).floatValue = 0;
            //Debug.Log($"Added element, size now {valuesProperty.arraySize}");
        }

        if (customProb) {
            
            Rect totalProbRect = new Rect(currentRect.width - probValuesWidth, currentRect.y, probValuesWidth,
                                          currentRect.height);
            if (probabilitiesProperty.arraySize != 0) {
                float runningTotal = GetRunningTotal(probabilitiesProperty);
                string direction = "";
                float remainder = 1 - runningTotal;
                if (Math.Abs(remainder) > 0.01f && probabilitiesProperty.arraySize > 0) {
                    if (runningTotal > 1) direction = " (too high)";
                    if (runningTotal < 1) direction = " (too low)";

                    EditorGUI.HelpBox(totalProbRect, $"Total = {runningTotal}{direction}", MessageType.Error);
                }
                else {
                    EditorGUI.LabelField(totalProbRect, $"Total = {runningTotal}{direction}");
                }
            }


        }

        if (probabilitiesProperty.arraySize == 0) {
            Rect noValueWarningRect = new Rect(x + 15f + minusWidth, currentRect.y,
                                               currentRect.width - x - 15 - minusWidth - probValuesWidth,
                                               currentRect.height);
            EditorGUI.HelpBox(noValueWarningRect, $"No values", MessageType.Error);
        }
    
    

        currentRect.y += LineHeight;

        return currentRect;
    }

    static float GetRunningTotal(SerializedProperty probabilitiesProperty, bool skipLast = false) {
        float runningTotal = 0;

        int n = probabilitiesProperty.arraySize;
        if (skipLast) n--;

        for (int i = 0; i < n; i++) {

            SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);
            if (prob.floatValue < 0 || prob.floatValue > 1) {
                throw new ArgumentOutOfRangeException("Can't have a probability outside of range 0-1");
            }
            runningTotal += prob.floatValue;
        }
        return runningTotal;
    }


}



[Serializable] public class IndependentVariableString : IndependentVariable<string> {
    public IndependentVariableString() {
        DataType = SupportedDataTypes.String;
    } 
}

//[Serializable]
//public class DatumGameObject : IndependentVariable<GameObject> {
//    //TODO
//}

//[Serializable]
//public class DatumVector3 : IndependentVariable<Vector3> {
//    //TODO
//}

//[Serializable]
//public class DatumVector2 : IndependentVariable<Vector2> {
//    //TODO
//}

//[Serializable]
//public class DatumCustom : IndependentVariable<CustomDatum> {
//    //TODO
//}

//public interface CustomDatum {
//    //TODO
//}

