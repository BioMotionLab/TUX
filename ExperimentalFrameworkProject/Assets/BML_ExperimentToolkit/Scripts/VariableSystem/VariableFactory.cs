using System;
using System.Collections.Generic;
using System.ComponentModel;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {


    [Serializable]
    public class VariableFactory {

        [SerializeField]
        public SupportedDataTypes DataTypesToCreate;

        [SerializeField]
        public VariableType VariableTypeToCreate;

        #region IndependentVariables

        public List<IndependentVariableInt> IntIVs = new List<IndependentVariableInt>();
        public List<IndependentVariableFloat> FloatIVs = new List<IndependentVariableFloat>();
        public List<IndependentVariableString> StringIVs = new List<IndependentVariableString>();
        public List<IndependentVariableBool> BoolIVs = new List<IndependentVariableBool>();
        public List<IndependentVariableGameObject> GameObjectIVs = new List<IndependentVariableGameObject>();
        public List<IndependentVariableVector2> Vector2IVs = new List<IndependentVariableVector2>();
        public List<IndependentVariableVector3> Vector3IVs = new List<IndependentVariableVector3>();
        public List<IndependentVariableCustomDataType> CustomDataTypeIVs = new List<IndependentVariableCustomDataType>();

        #endregion

        #region DependentVariables

        public List<DependentVariableInt> IntDVs = new List<DependentVariableInt>();
        public List<DependentVariableFloat> FloatDVs = new List<DependentVariableFloat>();
        public List<DependentVariableString> StringDVs = new List<DependentVariableString>();
        public List<DependentVariableBool> BoolDVs = new List<DependentVariableBool>();
        public List<DependentVariableGameObject> GameObjectDVs = new List<DependentVariableGameObject>();
        public List<DependentVariableVector2> Vector2DVs = new List<DependentVariableVector2>();
        public List<DependentVariableVector3> Vector3DVs = new List<DependentVariableVector3>();
        public List<DependentVariableCustomDataType> CustomDataTypeDVs = new List<DependentVariableCustomDataType>();

        #endregion

        #region ParticipantVariables

        public List<ParticipantVariableInt> IntParticipantVariables = new List<ParticipantVariableInt>();
        public List<ParticipantVariableFloat> FloatParticipantVariables = new List<ParticipantVariableFloat>();
        public List<ParticipantVariableString> StringParticipantVariables = new List<ParticipantVariableString>();
        public List<ParticipantVariableBool> BoolParticipantVariables = new List<ParticipantVariableBool>();
        public List<ParticipantVariableGameObject> GameObjectParticipantVariables = new List<ParticipantVariableGameObject>();
        public List<ParticipantVariableVector2> Vector2ParticipantVariables = new List<ParticipantVariableVector2>();
        public List<ParticipantVariableVector3> Vector3ParticipantVariables = new List<ParticipantVariableVector3>();
        public List<ParticipantVariableCustomData> CustomDataParticipantVariables = new List<ParticipantVariableCustomData>();

        #endregion


        public List<Variable> AllVariables {
            get {

                List<Variable> variables = new List<Variable>();

                //IVs
                variables.AddRange(IntIVs);
                variables.AddRange(FloatIVs);
                variables.AddRange(StringIVs);
                variables.AddRange(BoolIVs);
                variables.AddRange(GameObjectIVs);
                variables.AddRange(Vector2IVs);
                variables.AddRange(Vector3IVs);
                variables.AddRange(CustomDataTypeIVs);

                //DVs
                variables.AddRange(IntDVs);
                variables.AddRange(FloatDVs);
                variables.AddRange(StringDVs);
                variables.AddRange(BoolDVs);
                variables.AddRange(GameObjectDVs);
                variables.AddRange(Vector2DVs);
                variables.AddRange(Vector3DVs);
                variables.AddRange(CustomDataTypeDVs);
                

                //PARTICIPANT VARIABLES
                variables.AddRange(IntParticipantVariables);    
                variables.AddRange(FloatParticipantVariables);
                variables.AddRange(StringParticipantVariables);
                variables.AddRange(BoolParticipantVariables);
                variables.AddRange(GameObjectParticipantVariables);
                variables.AddRange(Vector2ParticipantVariables);
                variables.AddRange(Vector3ParticipantVariables);
                variables.AddRange(CustomDataParticipantVariables);
                
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
                        case SupportedDataTypes.Vector2:
                            IndependentVariableVector2 ivVector2 = new IndependentVariableVector2();
                            Vector2IVs.Add(ivVector2);
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
                        case SupportedDataTypes.Vector2:
                            DependentVariableVector2 newDependentVariableVector2 = new DependentVariableVector2();
                            Vector2DVs.Add(newDependentVariableVector2);
                            Debug.Log(Vector2DVs.Count);
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
                case VariableType.Participant:

                    switch (DataTypesToCreate) {
                        case SupportedDataTypes.Int:
                            ParticipantVariableInt newVariableInt = new ParticipantVariableInt();
                            IntParticipantVariables.Add(newVariableInt);
                            break;
                        case SupportedDataTypes.Float:
                            ParticipantVariableFloat newVariableFloat = new ParticipantVariableFloat();
                            FloatParticipantVariables.Add(newVariableFloat);
                            break;
                        case SupportedDataTypes.String:
                            ParticipantVariableString newVariableString = new ParticipantVariableString();
                            StringParticipantVariables.Add(newVariableString);
                            break;
                        case SupportedDataTypes.Bool:
                            ParticipantVariableBool newVariableBool = new ParticipantVariableBool();
                            BoolParticipantVariables.Add(newVariableBool);
                            break;
                        case SupportedDataTypes.GameObject:
                            ParticipantVariableGameObject newVariableGameObject = new ParticipantVariableGameObject();
                            GameObjectParticipantVariables.Add(newVariableGameObject);
                            break;
                        case SupportedDataTypes.Vector2:
                            ParticipantVariableVector2 newVariableVector2 = new ParticipantVariableVector2();
                            Vector2ParticipantVariables.Add(newVariableVector2);
                            break;
                        case SupportedDataTypes.Vector3:
                            ParticipantVariableVector3 newVariableVector3 = new ParticipantVariableVector3();
                            Vector3ParticipantVariables.Add(newVariableVector3);
                            break;
                        case SupportedDataTypes.CustomDataType:
                            ParticipantVariableCustomData newVariableCustomData = new ParticipantVariableCustomData();
                            CustomDataParticipantVariables.Add(newVariableCustomData);
                            break;
                        case SupportedDataTypes.ChooseType:
                            throw new
                                InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
                        default:
                            throw new NotImplementedException("Support for this BlockData types has not yet been defined." +
                                                              "You can customize it yourself in the IndependentVariable.cs class");
                    }


                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(VariableTypeToCreate), DataTypesToCreate, null);
            }

        }

        public ExperimentDesign ToTable(ExperimentRunner experimentRunner, bool shuffleTrialOrder, int numberRepetitions, bool shuffleTrialsBetweenBlocks) {
            //Debug.Log($"ToTable method in IndependentVariable: Alldata.count {AllVariables.Count}");
            return new ExperimentDesign(experimentRunner, AllVariables, shuffleTrialOrder, numberRepetitions, shuffleTrialsBetweenBlocks );
        }


    }
}