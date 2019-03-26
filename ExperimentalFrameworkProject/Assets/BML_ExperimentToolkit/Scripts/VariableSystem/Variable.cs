using System;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.VariableSystem.ValueAddingStrategies;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {


    [Serializable]
    public abstract class Variable {
        public string Name;

        public          VariableType       TypeOfVariable;
        public          SupportedDataTypes DataType;
        public abstract Type               Type { get; }
        public abstract DataTable AddValuesTo(DataTable table);
    }

    [Serializable]
    public abstract class IndependentVariable : Variable {
        public VariableMixingType MixingTypeOfVariable;
        public bool               Block;
    }

    [Serializable]
    public abstract class DependentVariable : Variable {
    }

    [Serializable]
    public abstract class IndependentVariable<T> : IndependentVariable {

        public override Type Type => typeof(T);
        public override DataTable AddValuesTo(DataTable table) {

            if (Values.Count == 0) {
                throw new ArgumentNullException($"Can't add values, none defined for variable: {Name}");
            }

            IndependentValuesStrategy<T> independentValuesStrategy = IndependentValuesStrategyFactory.Create<T>(MixingTypeOfVariable);
            return independentValuesStrategy.AddValuesToTable(table, this);
        }


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

}

    [Serializable]
    public abstract class DependentVariable<T> : DependentVariable {
        public          T    Value;
        public          T    DefaultValue;
        public override Type Type => typeof(T);

        protected DependentVariable() {
            Name = $"Unnamed DependentVariable Variable (type:{typeof(T)})";
            TypeOfVariable = VariableType.Dependent;

        }

        public override DataTable AddValuesTo(DataTable table) {
            return valuesStrategy.AddValuesToCopyOf(table, this);
        }

        readonly DependentValuesStrategy<T> valuesStrategy = new DependentValuesStrategy<T>();

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
        Bool,
        GameObject,
        Vector3,
        CustomDataType,
        ChooseType,
    }

    public interface CustomSupportedDataType {
    }

    [Serializable]
    public enum VariableMixingType {
        Balanced,
        Looped,
        EvenProbability,
        CustomProbability
    }

//INT
    [Serializable]
    public class IndependentVariableInt : IndependentVariable<int> {
        public IndependentVariableInt() {
            DataType = SupportedDataTypes.Int;
        }
    }

    [CustomPropertyDrawer(typeof(IndependentVariableInt))]
    public class IndependentVariableIntDrawer : IndependentVariableDrawer {
    }


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
    public class IndependentVariableFloatDrawer : IndependentVariableDrawer {
    }


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
    public class IndependentVariableStringDrawer : IndependentVariableDrawer {
    }


    [Serializable]
    public class DependentVariableString : DependentVariable<string> {
        public DependentVariableString() {
            DataType = SupportedDataTypes.String;
        }
    }

    [CustomPropertyDrawer(typeof(DependentVariableString))]
    public class DependentVariableStringDrawer : DependentVariableDrawer {
    }

    //BOOL

    [Serializable]
    public class IndependentVariableBool : IndependentVariable<bool> {
        public IndependentVariableBool() {
            DataType = SupportedDataTypes.Bool;
            Values.Add(true);
            Values.Add(false);
        }

        
    }

    [CustomPropertyDrawer(typeof(IndependentVariableBool))]
    public class IndependentVariableBoolDrawer : IndependentVariableDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CustomPropertyHeight = VariableDrawerHelpers.AddAllBoolVariableProperties(position, property);
        }
    }


    [Serializable]
    public class DependentVariableBool : DependentVariable<bool> {
        public DependentVariableBool() {
            DataType = SupportedDataTypes.Bool;
        }
    }

    [CustomPropertyDrawer(typeof(DependentVariableBool))]
    public class DependentVariableBoolDrawer : DependentVariableDrawer {
    }

    // GAME OBJECT

    [Serializable]
    public class IndependentVariableGameObject : IndependentVariable<GameObject> {
        public IndependentVariableGameObject() {
            DataType = SupportedDataTypes.GameObject;
        }
    }

    [CustomPropertyDrawer(typeof(IndependentVariableGameObject))]
    public class IndependentVariableGameObjectDrawer : IndependentVariableDrawer {
    }


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
    public class IndependentVariableVector3Drawer : IndependentVariableDrawer {
    }


    [Serializable]
    public class DependentVariableVector3 : DependentVariable<Vector3> {
        public DependentVariableVector3() {
            DataType = SupportedDataTypes.Vector3;
        }
    }

    [CustomPropertyDrawer(typeof(DependentVariableVector3))]
    public class DependentVariableVector3Drawer : DependentVariableDrawer {
    }

// CUSTOM DATA TYPE

    [Serializable]
    public class IndependentVariableCustomDataType : IndependentVariable<CustomSupportedDataType> {
        public IndependentVariableCustomDataType() {
            DataType = SupportedDataTypes.CustomDataType;
        }
    }

    [CustomPropertyDrawer(typeof(IndependentVariableCustomDataType))]
    public class IndependentVariableCustomDataTypeDrawer : IndependentVariableDrawer {
    }


    [Serializable]
    public class DependentVariableCustomDataType : DependentVariable<CustomSupportedDataType> {
        public DependentVariableCustomDataType() {
            DataType = SupportedDataTypes.CustomDataType;
        }
    }

    [CustomPropertyDrawer(typeof(DependentVariableCustomDataType))]
    public class DependentVariableCustomDataTypeDrawer : DependentVariableDrawer {
    }

}