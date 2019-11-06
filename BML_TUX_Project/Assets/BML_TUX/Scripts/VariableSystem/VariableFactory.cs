using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BML_TUX.Scripts.VariableSystem.VariableTypes;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem {


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
                allVariables.AddRange(IndependentVariables);
                allVariables.AddRange(DependentVariables);
                allVariables.AddRange(ParticipantVariables);
                return new Variables(allVariables);
            }
        }

        public List<Variable> ParticipantVariables { 
            get {
                List<Variable> participantVariables = new List<Variable>();
                    participantVariables.AddRange(IntParticipantVariables);
                    participantVariables.AddRange(FloatParticipantVariables);
                    participantVariables.AddRange(StringParticipantVariables);
                    participantVariables.AddRange(BoolParticipantVariables);
                    participantVariables.AddRange(GameObjectParticipantVariables);
                    participantVariables.AddRange(Vector2ParticipantVariables);
                    participantVariables.AddRange(Vector3ParticipantVariables);
                    participantVariables.AddRange(CustomDataParticipantVariables);
                    return participantVariables;
                }
            }

        public List<Variable> DependentVariables { get {
            List<Variable> dependentVariables = new List<Variable>();
            dependentVariables.AddRange(IntDVs);
            dependentVariables.AddRange(FloatDVs);
            dependentVariables.AddRange(StringDVs);
            dependentVariables.AddRange(BoolDVs);
            dependentVariables.AddRange(GameObjectDVs);
            dependentVariables.AddRange(Vector2DVs);
            dependentVariables.AddRange(Vector3DVs);
            dependentVariables.AddRange(CustomDataTypeDVs);
            return dependentVariables;
        }}

        public List<Variable> IndependentVariables {
            get {
                List<Variable> independentVariables = new List<Variable>();
                independentVariables.AddRange(IntIVs);
                independentVariables.AddRange(FloatIVs);
                independentVariables.AddRange(StringIVs);
                independentVariables.AddRange(BoolIVs);
                independentVariables.AddRange(GameObjectIVs);
                independentVariables.AddRange(Vector2IVs);
                independentVariables.AddRange(Vector3IVs);
                independentVariables.AddRange(CustomDataTypeIVs);
                return independentVariables;
            }
        }


        // ReSharper disable once CognitiveComplexity
        public Variable AddNew() {
            //TODO Probably a better way to do this
            Variable newVar;
            switch (VariableTypeToCreate) {
                case VariableType.Independent:
                    Type ivType = supportedIndependentTypes[DataTypeToCreate];
                    IndependentVariable newIv = Activator.CreateInstance(ivType) as IndependentVariable;
                    UpdateSerializedListsWith(newIv);
                    newVar = newIv;
                    break;
                case VariableType.Dependent:
                    Type dvType = supportedDependentTypes[DataTypeToCreate];
                    DependentVariable newDv = Activator.CreateInstance(dvType) as DependentVariable;
                    UpdateSerializedListsWith(newDv);
                    newVar = newDv;
                    break;
                case VariableType.ChooseType:
                    newVar = null;
                    break;
                case VariableType.Participant:
                    Type pvType = supportedParticipantTypes[DataTypeToCreate];
                    ParticipantVariable newPv = Activator.CreateInstance(pvType) as ParticipantVariable;
                    UpdateSerializedListsWith(newPv);
                    newVar = newPv;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(VariableTypeToCreate), DataTypeToCreate, null);
            }

            return newVar;

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

        public void RemoveVariable(Variable variable) {
            //TODO Probably a better way to do this.
            if (variable.DataType == SupportedDataType.ChooseType) 
                throw new InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");
            
            switch (variable) {
                case DependentVariableBool dependentVariableBool:
                    BoolDVs.Remove(dependentVariableBool);
                    break;
                case DependentVariableCustomDataType dependentVariableCustomDataType:
                    CustomDataTypeDVs.Remove(dependentVariableCustomDataType);
                    break;
                case DependentVariableFloat dependentVariableFloat:
                    FloatDVs.Remove(dependentVariableFloat);
                    break;
                case DependentVariableGameObject dependentVariableGameObject:
                    GameObjectDVs.Remove(dependentVariableGameObject);
                    break;
                case DependentVariableInt dependentVariableInt:
                    IntDVs.Remove(dependentVariableInt);
                    break;
                case DependentVariableString dependentVariableString:
                    StringDVs.Remove(dependentVariableString);
                    break;
                case DependentVariableVector2 dependentVariableVector2:
                    Vector2DVs.Remove(dependentVariableVector2);
                    break;
                case DependentVariableVector3 dependentVariableVector3:
                    Vector3DVs.Remove(dependentVariableVector3);
                    break;
                case IndependentVariableInt independentVariableInt:
                    IntIVs.Remove(independentVariableInt);
                    break;
                case IndependentVariableFloat independentVariableFloat:
                    FloatIVs.Remove(independentVariableFloat);
                    break;
                case IndependentVariableString independentVariableString:
                    StringIVs.Remove(independentVariableString);
                    break;
                case IndependentVariableBool independentVariableBool:
                    BoolIVs.Remove(independentVariableBool);
                    break;
                case IndependentVariableGameObject independentVariableGameObject:
                    GameObjectIVs.Remove(independentVariableGameObject);
                    break;
                case IndependentVariableVector2 independentVariableVector2:
                    Vector2IVs.Remove(independentVariableVector2);
                    break;
                case IndependentVariableVector3 independentVariableVector3:
                    Vector3IVs.Remove(independentVariableVector3);
                    break;
                case ParticipantVariableBool participantVariableBool:
                    BoolParticipantVariables.Remove(participantVariableBool);
                    break;
                case ParticipantVariableCustomData participantVariableCustomData:
                    CustomDataParticipantVariables.Remove(participantVariableCustomData);
                    break;
                case ParticipantVariableFloat participantVariableFloat:
                    FloatParticipantVariables.Remove(participantVariableFloat);
                    break;
                case ParticipantVariableGameObject participantVariableGameObject:
                    GameObjectParticipantVariables.Remove(participantVariableGameObject);
                    break;
                case ParticipantVariableInt participantVariableInt:
                    IntParticipantVariables.Remove(participantVariableInt);
                    break;
                case ParticipantVariableString participantVariableString:
                    StringParticipantVariables.Remove(participantVariableString);
                    break;
                case ParticipantVariableVector2 participantVariableVector2:
                    Vector2ParticipantVariables.Remove(participantVariableVector2);
                    break;
                case ParticipantVariableVector3 participantVariableVector3:
                    Vector3ParticipantVariables.Remove(participantVariableVector3);
                    break;
                case IndependentVariableCustomDataType independentVariableCustomDataType:
                    CustomDataTypeIVs.Remove(independentVariableCustomDataType);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Support for this Type has not yet been defined." +
                                                           "You can customize it yourself in the in the variable system");
            }
        }
    }
}