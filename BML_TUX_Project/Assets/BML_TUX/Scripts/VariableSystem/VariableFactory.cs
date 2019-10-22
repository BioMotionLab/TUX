using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {


    //TODO Code smell. feel like some kind of strategy pattern needed here...
    //but need to have serialized lists available in unity inspector
    [Serializable]
    public class VariableFactory {
        [SerializeField]
        public SupportedDataType DataTypeToCreate;

        [SerializeField]
        public VariableType VariableTypeToCreate;


        readonly Dictionary<SupportedDataType, Type> supportedIndependentTypes;
        readonly Dictionary<SupportedDataType, Type> supportedDependentTypes;
        readonly Dictionary<SupportedDataType, Type> supportedParticipantTypes;
        

        public VariableFactory() {
            supportedIndependentTypes = GetSupportedTypes(typeof(IndependentVariable));
            supportedDependentTypes = GetSupportedTypes(typeof(DependentVariable));
            supportedParticipantTypes = GetSupportedTypes(typeof(ParticipantVariable));
        }

        static Dictionary<SupportedDataType, Type> GetSupportedTypes(Type baseType) {
            
            IEnumerable<Type> concreteTypes = Assembly.GetAssembly(baseType).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract &&
                                 myType.IsSubclassOf(baseType));
            
            
            Dictionary<SupportedDataType, Type> createdDic = new Dictionary<SupportedDataType, Type>();
            
            foreach (Type concreteType in concreteTypes) {
                dynamic tempVariable = Activator.CreateInstance(concreteType);
                if (!createdDic.ContainsKey(tempVariable.DataType)) createdDic.Add(tempVariable.DataType, concreteType);
            }

            return createdDic;
        }

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


        public Variables Variables {
            get {

                List<Variable> allVariables = new List<Variable>();

                //IVs
                allVariables.AddRange(IntIVs);
                allVariables.AddRange(FloatIVs);
                allVariables.AddRange(StringIVs);
                allVariables.AddRange(BoolIVs);
                allVariables.AddRange(GameObjectIVs);
                allVariables.AddRange(Vector2IVs);
                allVariables.AddRange(Vector3IVs);
                allVariables.AddRange(CustomDataTypeIVs);

                //DVs
                allVariables.AddRange(IntDVs);
                allVariables.AddRange(FloatDVs);
                allVariables.AddRange(StringDVs);
                allVariables.AddRange(BoolDVs);
                allVariables.AddRange(GameObjectDVs);
                allVariables.AddRange(Vector2DVs);
                allVariables.AddRange(Vector3DVs);
                allVariables.AddRange(CustomDataTypeDVs);
                

                //PARTICIPANT VARIABLES
                allVariables.AddRange(IntParticipantVariables);    
                allVariables.AddRange(FloatParticipantVariables);
                allVariables.AddRange(StringParticipantVariables);
                allVariables.AddRange(BoolParticipantVariables);
                allVariables.AddRange(GameObjectParticipantVariables);
                allVariables.AddRange(Vector2ParticipantVariables);
                allVariables.AddRange(Vector3ParticipantVariables);
                allVariables.AddRange(CustomDataParticipantVariables);
                
                
                return new Variables(allVariables);
            }
        }


        // ReSharper disable once CognitiveComplexity
        public void AddNew() {
            //TODO Probably a better way to do this
            switch (VariableTypeToCreate) {
                case VariableType.Independent:
                    if (supportedIndependentTypes.TryGetValue(DataTypeToCreate, out Type ivType)) {
                        IndependentVariable newIv = Activator.CreateInstance(ivType) as IndependentVariable;
                        UpdateSerializedListsWith(newIv);
                    }
                    break;
                case VariableType.Dependent:
                    if (supportedDependentTypes.TryGetValue(DataTypeToCreate, out Type dvType)) {
                        DependentVariable newDv = Activator.CreateInstance(dvType) as DependentVariable;
                        UpdateSerializedListsWith(newDv);
                    }
                    break;
                case VariableType.ChooseType:
                    break;
                case VariableType.Participant:
                    if (supportedParticipantTypes.TryGetValue(DataTypeToCreate, out Type pvType)) {
                        ParticipantVariable newPv = Activator.CreateInstance(pvType) as ParticipantVariable;
                        UpdateSerializedListsWith(newPv);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(VariableTypeToCreate), DataTypeToCreate, null);
            }

        }

        // ReSharper disable once CognitiveComplexity
        void UpdateSerializedListsWith(Variable variable) {
            //TODO Probably a better way to do this.
            if (variable.DataType == SupportedDataType.ChooseType) 
                throw new InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
            
            switch (variable) {
                case DependentVariableBool dependentVariableBool:
                    BoolDVs.Add(dependentVariableBool);
                    break;
                case DependentVariableCustomDataType dependentVariableCustomDataType:
                    CustomDataTypeDVs.Add(dependentVariableCustomDataType);
                    break;
                case DependentVariableFloat dependentVariableFloat:
                    FloatDVs.Add(dependentVariableFloat);
                    break;
                case DependentVariableGameObject dependentVariableGameObject:
                    GameObjectDVs.Add(dependentVariableGameObject);
                    break;
                case DependentVariableInt dependentVariableInt:
                    IntDVs.Add(dependentVariableInt);
                    break;
                case DependentVariableString dependentVariableString:
                    StringDVs.Add(dependentVariableString);
                    break;
                case DependentVariableVector2 dependentVariableVector2:
                    Vector2DVs.Add(dependentVariableVector2);
                    break;
                case DependentVariableVector3 dependentVariableVector3:
                    Vector3DVs.Add(dependentVariableVector3);
                    break;
                case IndependentVariableInt independentVariableInt:
                    IntIVs.Add(independentVariableInt);
                    break;
                case IndependentVariableFloat independentVariableFloat:
                    FloatIVs.Add(independentVariableFloat);
                    break;
                case IndependentVariableString independentVariableString:
                    StringIVs.Add(independentVariableString);
                    break;
                case IndependentVariableBool independentVariableBool:
                    BoolIVs.Add(independentVariableBool);
                    break;
                case IndependentVariableGameObject independentVariableGameObject:
                    GameObjectIVs.Add(independentVariableGameObject);
                    break;
                case IndependentVariableVector2 independentVariableVector2:
                    Vector2IVs.Add(independentVariableVector2);
                    break;
                case IndependentVariableVector3 independentVariableVector3:
                    Vector3IVs.Add(independentVariableVector3);
                    break;
                case ParticipantVariableBool participantVariableBool:
                    BoolParticipantVariables.Add(participantVariableBool);
                    break;
                case ParticipantVariableCustomData participantVariableCustomData:
                    CustomDataParticipantVariables.Add(participantVariableCustomData);
                    break;
                case ParticipantVariableFloat participantVariableFloat:
                    FloatParticipantVariables.Add(participantVariableFloat);
                    break;
                case ParticipantVariableGameObject participantVariableGameObject:
                    GameObjectParticipantVariables.Add(participantVariableGameObject);
                    break;
                case ParticipantVariableInt participantVariableInt:
                    IntParticipantVariables.Add(participantVariableInt);
                    break;
                case ParticipantVariableString participantVariableString:
                    StringParticipantVariables.Add(participantVariableString);
                    break;
                case ParticipantVariableVector2 participantVariableVector2:
                    Vector2ParticipantVariables.Add(participantVariableVector2);
                    break;
                case ParticipantVariableVector3 participantVariableVector3:
                    Vector3ParticipantVariables.Add(participantVariableVector3);
                    break;
                case IndependentVariableCustomDataType independentVariableCustomDataType:
                    CustomDataTypeIVs.Add(independentVariableCustomDataType);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Support for this Type has not yet been defined." +
                                                           "You can customize it yourself in the in the variable system");
            }
        }

    }
}