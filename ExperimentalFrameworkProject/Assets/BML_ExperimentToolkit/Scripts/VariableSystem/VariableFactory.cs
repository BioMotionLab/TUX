using System;
using System.Collections.Generic;
using System.ComponentModel;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {


    [Serializable]
    public class VariableFactory {

        [SerializeField]
        public SupportedDataTypes DataTypesToCreate;

        [SerializeField]
        public VariableType VariableTypeToCreate;

        //IVS

        [SerializeField]
        public List<IndependentVariableInt> IntIVs = new List<IndependentVariableInt>();

        [SerializeField]
        public List<IndependentVariableFloat> FloatIVs = new List<IndependentVariableFloat>();

        [SerializeField]
        public List<IndependentVariableString> StringIVs = new List<IndependentVariableString>();

        [SerializeField]
        public List<IndependentVariableBool> BoolIVs = new List<IndependentVariableBool>();

        [SerializeField]
        public List<IndependentVariableGameObject> GameObjectIVs = new List<IndependentVariableGameObject>();

        [SerializeField]
        public List<IndependentVariableVector3> Vector3IVs = new List<IndependentVariableVector3>();

        [SerializeField]
        public List<IndependentVariableCustomDataType>
            CustomDataTypeIVs = new List<IndependentVariableCustomDataType>();

        //DVS

        [SerializeField]
        public List<DependentVariableInt> IntDVs = new List<DependentVariableInt>();

        [SerializeField]
        public List<DependentVariableFloat> FloatDVs = new List<DependentVariableFloat>();

        [SerializeField]
        public List<DependentVariableString> StringDVs = new List<DependentVariableString>();

        [SerializeField]
        public List<DependentVariableBool> BoolDVs = new List<DependentVariableBool>();

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

                foreach (var variable in BoolIVs) {
                    variables.Add(variable);
                }

                foreach (var variable in GameObjectIVs) {
                    variables.Add(variable);
                }

                foreach (var variable in Vector3IVs) {
                    variables.Add(variable);
                }

                foreach (var variable in CustomDataTypeIVs) {
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

                foreach (var variable in BoolDVs) {
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
                        case SupportedDataTypes.Bool:
                            IndependentVariableBool ivBool = new IndependentVariableBool();
                            BoolIVs.Add(ivBool);
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
                            IndependentVariableCustomDataType
                                ivCustomDataType = new IndependentVariableCustomDataType();
                            CustomDataTypeIVs.Add(ivCustomDataType);
                            break;
                        case SupportedDataTypes.ChooseType:
                            throw new
                                InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
                        default:
                            throw new NotImplementedException("Support for this BlockData types has not yet been defined." +
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
                        case SupportedDataTypes.Bool:
                            DependentVariableBool newDependentVariableBool = new DependentVariableBool();
                            BoolDVs.Add(newDependentVariableBool);
                            break;
                        case SupportedDataTypes.GameObject:
                            DependentVariableGameObject newDependentVariableGameObject =
                                new DependentVariableGameObject();
                            GameObjectDVs.Add(newDependentVariableGameObject);
                            break;
                        case SupportedDataTypes.Vector3:
                            DependentVariableVector3 newDependentVariableVector3 = new DependentVariableVector3();
                            Vector3DVs.Add(newDependentVariableVector3);
                            break;
                        case SupportedDataTypes.CustomDataType:
                            DependentVariableCustomDataType newDependentVariableCustomDataType =
                                new DependentVariableCustomDataType();
                            CustomDataTypeDVs.Add(newDependentVariableCustomDataType);
                            break;
                        case SupportedDataTypes.ChooseType:
                            throw new
                                InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
                        default:
                            throw new NotImplementedException("Support for this BlockData types has not yet been defined." +
                                                              "You can customize it yourself in the IndependentVariable.cs class");
                    }

                    break;
                case VariableType.ChooseType:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(VariableTypeToCreate), DataTypesToCreate, null);
            }

        }

        public ExperimentDesign ToTable(Experiment experiment, bool shuffleTrialOrder, int numberRepetitions) {
            //Debug.Log($"ToTable method in IndependentVariable: Alldata.count {AllVariables.Count}");
            return new ExperimentDesign(experiment, AllVariables, shuffleTrialOrder, numberRepetitions);
        }


    }
}