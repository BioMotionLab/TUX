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
    public T DefaultValue;
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
    GameObject,
    Vector3,
    CustomDataType,
    ChooseType,
}

public abstract class CustomMonoBehaviour : MonoBehaviour { }

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

    //IVS

    [SerializeField]
    public List<IndependentVariableInt> IntIVs = new List<IndependentVariableInt>();
    
    [SerializeField]
    public List<IndependentVariableFloat> FloatIVs = new List<IndependentVariableFloat>();

    [SerializeField]
    public List<IndependentVariableString> StringIVs = new List<IndependentVariableString>();

    [SerializeField]
    public List<IndependentVariableGameObject> GameObjectIVs = new List<IndependentVariableGameObject>();

    [SerializeField]
    public List<IndependentVariableVector3> Vector3IVs = new List<IndependentVariableVector3>();

    [SerializeField]
    public List<IndependentVariableCustomDataType> CustomDataTypeIVs = new List<IndependentVariableCustomDataType>();

    //DVS

    [SerializeField]
    public List<DependentVariableInt> IntDVs = new List<DependentVariableInt>();

    [SerializeField]
    public List<DependentVariableFloat> FloatDVs = new List<DependentVariableFloat>();

    [SerializeField]
    public List<DependentVariableString> StringDVs = new List<DependentVariableString>();

    [SerializeField]
    public List<DependentVariableGameObject> GameObjectDVs = new List<DependentVariableGameObject>();

    [SerializeField]
    public List<DependentVariableVector3> Vector3DVs = new List<DependentVariableVector3>();

    [SerializeField]
    public List<DependentVariableCustomDataType> CustomDataTypeDVs = new List<DependentVariableCustomDataType>();



    //[SerializeField]
    //public List<DatumGameObject> GameObjectData = new List<DatumGameObject>();

    //[SerializeField]
    //public List<DatumVector3> Vector3Data = new List<DatumVector3>();

    //[SerializeField]
    //public List<DatumCustom> CustomData = new List<DatumCustom>();

    public List<Variable> AllVariables {
        get {
            List<Variable> variables = new List<Variable>();
            foreach (var variable in IntIVs) {
                variables.Add(variable);
            }

            foreach (var variable in FloatIVs) {
                variables.Add(variable);
            }

            foreach (var variable in StringIVs) {
                variables.Add(variable);
            }

            foreach (var variable in GameObjectIVs) {
                variables.Add(variable);
            }

            foreach (var variable in Vector3IVs) {
                variables.Add(variable);
            }

            foreach(var variable in CustomDataTypeIVs) {
                variables.Add(variable);
            }

            //DVS
            foreach (var variable in IntDVs) {
                variables.Add(variable);
            }

            foreach (var variable in FloatDVs) {
                variables.Add(variable);
            }

            foreach (var variable in StringDVs) {
                variables.Add(variable);
            }

            foreach (var variable in GameObjectDVs) {
                variables.Add(variable);
            }

            foreach (var variable in Vector3DVs) {
                variables.Add(variable);
            }

            foreach (var variable in CustomDataTypeDVs) {
                variables.Add(variable);
            }

            return variables;
        }
    }

    public void AddNew() {
        switch (VariableTypeToCreate) {
            case VariableType.Independent: {


                    switch (DataTypesToCreate) {
                        case SupportedDataTypes.Int:
                            IndependentVariableInt ivInt = new IndependentVariableInt();
                            IntIVs.Add(ivInt);
                            break;
                        case SupportedDataTypes.Float:
                            IndependentVariableFloat ivFloat = new IndependentVariableFloat();
                            FloatIVs.Add(ivFloat);
                            break;
                        case SupportedDataTypes.String:
                            IndependentVariableString ivString = new IndependentVariableString();
                            StringIVs.Add(ivString);
                            break;
                        case SupportedDataTypes.GameObject:
                            IndependentVariableGameObject ivGameObject = new IndependentVariableGameObject();
                            GameObjectIVs.Add(ivGameObject);
                            break;
                        case SupportedDataTypes.Vector3:
                            IndependentVariableVector3 ivVector3 = new IndependentVariableVector3();
                            Vector3IVs.Add(ivVector3);
                            break;
                        case SupportedDataTypes.CustomDataType:
                            IndependentVariableCustomDataType ivCustomDataType = new IndependentVariableCustomDataType();
                            CustomDataTypeIVs.Add(ivCustomDataType);
                            break;
                        case SupportedDataTypes.ChooseType:
                            throw new InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
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
                        IntDVs.Add(newDependentVariableInt);
                        break;
                    case SupportedDataTypes.Float:
                        DependentVariableFloat newDependentVariableFloat = new DependentVariableFloat();
                        FloatDVs.Add(newDependentVariableFloat);
                        break;
                    case SupportedDataTypes.String:
                        DependentVariableString newDependentVariableString = new DependentVariableString();
                        StringDVs.Add(newDependentVariableString);
                        break;
                    case SupportedDataTypes.GameObject:
                        DependentVariableGameObject newDependentVariableGameObject = new DependentVariableGameObject();
                        GameObjectDVs.Add(newDependentVariableGameObject);
                        break;
                    case SupportedDataTypes.Vector3:
                        DependentVariableVector3 newDependentVariableVector3 = new DependentVariableVector3();
                        Vector3DVs.Add(newDependentVariableVector3);
                        break;
                    case SupportedDataTypes.CustomDataType:
                        DependentVariableCustomDataType newDependentVariableCustomDataType = new DependentVariableCustomDataType();
                        CustomDataTypeDVs.Add(newDependentVariableCustomDataType);
                        break;
                    case SupportedDataTypes.ChooseType:
                        throw new InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
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

    public ExperimentDesign ToTable(bool shuffleTrialOrder, int numberRepetitions) {
        //Debug.Log($"ToTable method in IndependentVariable: Alldata.count {AllVariables.Count}");
        return new ExperimentDesign(AllVariables, shuffleTrialOrder, numberRepetitions);
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
[CustomPropertyDrawer(typeof(DependentVariableInt))]
public class DependentVariableIntDrawer : DependentVariableDrawer {
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


[Serializable]
public class DependentVariableFloat : DependentVariable<float> {
    public DependentVariableFloat() {
        DataType = SupportedDataTypes.Float;
    }
}
[CustomPropertyDrawer(typeof(DependentVariableFloat))]
public class DependentVariableFloatDrawer : DependentVariableDrawer {
}


//STRING

[Serializable]
public class IndependentVariableString : IndependentVariable<string> {
    public IndependentVariableString() {
        DataType = SupportedDataTypes.String;
    }
}
[CustomPropertyDrawer(typeof(IndependentVariableString))]
public class IndependentVariableStringDrawer : IndependentVariableDrawer { }


[Serializable]
public class DependentVariableString : DependentVariable<string> {
    public DependentVariableString() {
        DataType = SupportedDataTypes.String;
    }
}
[CustomPropertyDrawer(typeof(DependentVariableString))]
public class DependentVariableStringDrawer : DependentVariableDrawer {
}

// GAMEOBJECT

[Serializable]
public class IndependentVariableGameObject : IndependentVariable<GameObject> {
    public IndependentVariableGameObject() {
        DataType = SupportedDataTypes.GameObject;
    }
}
[CustomPropertyDrawer(typeof(IndependentVariableGameObject))]
public class IndependentVariableGameObjectDrawer : IndependentVariableDrawer { }


[Serializable]
public class DependentVariableGameObject : DependentVariable<GameObject> {
    public DependentVariableGameObject() {
        DataType = SupportedDataTypes.GameObject;
    }
}
[CustomPropertyDrawer(typeof(DependentVariableGameObject))]
public class DependentVariableGameObjectDrawer : DependentVariableDrawer {
}

//VECTOR3

[Serializable]
public class IndependentVariableVector3 : IndependentVariable<Vector3> {
    public IndependentVariableVector3() {
        DataType = SupportedDataTypes.Vector3;
    }
}
[CustomPropertyDrawer(typeof(IndependentVariableVector3))]
public class IndependentVariableVector3Drawer : IndependentVariableDrawer { }


[Serializable]
public class DependentVariableVector3 : DependentVariable<Vector3> {
    public DependentVariableVector3() {
        DataType = SupportedDataTypes.Vector3;
    }
}
[CustomPropertyDrawer(typeof(DependentVariableVector3))]
public class DependentVariableVector3Drawer : DependentVariableDrawer {
}

// CUSTOMDATATYPE

[Serializable]
public class IndependentVariableCustomDataType : IndependentVariable<CustomMonoBehaviour> {
    public IndependentVariableCustomDataType() {
        DataType = SupportedDataTypes.CustomDataType;
    }
}
[CustomPropertyDrawer(typeof(IndependentVariableCustomDataType))]
public class IndependentVariableCustomDataTypeDrawer : IndependentVariableDrawer { }


[Serializable]
public class DependentVariableCustomDataType : DependentVariable<CustomMonoBehaviour> {
    public DependentVariableCustomDataType() {
        DataType = SupportedDataTypes.CustomDataType;
    }
}
[CustomPropertyDrawer(typeof(DependentVariableCustomDataType))]
public class DependentVariableCustomDataTypeDrawer : DependentVariableDrawer {
}



public class VariableDrawer : PropertyDrawer {
    const  float LineHeight = 20f;
    protected float customPropertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float propertyHeight = EditorGUI.GetPropertyHeight(property, GUIContent.none);
        propertyHeight += customPropertyHeight;
        return propertyHeight;
    }
}

public class DependentVariableDrawer : VariableDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        customPropertyHeight = VariableDrawerHelpers.AddAllDependentVariableProperties(position, property);
    }
}

public class IndependentVariableDrawer : VariableDrawer {
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

    public static float AddAllDependentVariableProperties(Rect position, SerializedProperty property) {
        property.serializedObject.Update();
        Rect currentRect = new Rect(position.x, position.y + LineHeight, position.width, LineHeight);
        float oldY = currentRect.y;

        int oldIndentLevel = EditorGUI.indentLevel;

        currentRect = VariableDrawerHelpers.AddVariableProperties(property, currentRect);
        currentRect = VariableDrawerHelpers.AddDependentVariableValueProperties(property, currentRect);

        EditorGUI.indentLevel = oldIndentLevel;
        float propertyHeight = currentRect.y - oldY;
        property.serializedObject.ApplyModifiedProperties();
        return propertyHeight;
    }

    static Rect AddDependentVariableValueProperties(SerializedProperty property, Rect currentRect) {


        var defaultValueProperty = property.FindPropertyRelative("DefaultValue");

        EditorGUI.PropertyField(currentRect, defaultValueProperty);
        

        currentRect.y += LineHeight;
        return currentRect;
    }


    public static Rect AddVariableProperties(SerializedProperty property, Rect currentRect) {

        const float typeWidth = 200f;
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
            Rect valuesRect = new Rect(x + minusWidth, currentRect.y, 0.5f*currentRect.width, currentRect.height);
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
            if (customProb && probabilitiesProperty.arraySize >= 2) {
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



