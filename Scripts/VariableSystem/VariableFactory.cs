using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {


    //TODO Code smell. feel like some kind of strategy pattern needed here...
    //but need to have serialized lists available in unity inspector
    [Serializable]
    public class VariableFactory {
        [SerializeField]
        public SupportedDataType DataTypeToCreate = SupportedDataType.ChooseType;

        [SerializeField]
        public VariableType VariableTypeToCreate = VariableType.ChooseType;


        readonly Dictionary<SupportedDataType, Type> supportedIndependentTypes;
        readonly Dictionary<SupportedDataType, Type> supportedDependentTypes;
        readonly Dictionary<SupportedDataType, Type> supportedParticipantTypes;

        [SerializeReference] public List<Variable> IndependentVariables;
        [SerializeReference] public List<Variable> DependentVariables;
        [SerializeReference] public List<Variable> ParticipantVariables;
        
        public VariableFactory() {
            supportedIndependentTypes = GetSupportedTypes(typeof(IndependentVariable));
            supportedDependentTypes = GetSupportedTypes(typeof(DependentVariable));
            supportedParticipantTypes = GetSupportedTypes(typeof(ParticipantVariable));
            
            IndependentVariables = new List<Variable>();
            DependentVariables = new List<Variable>();
            ParticipantVariables = new List<Variable>();
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

        public Variables Variables {
            get {
                List<Variable> allVariables = new List<Variable>();
                allVariables.AddRange(IndependentVariables);
                allVariables.AddRange(DependentVariables);
                allVariables.AddRange(ParticipantVariables);
                return new Variables(allVariables);
            }
        }



        // ReSharper disable once CognitiveComplexity
        public Variable AddNew() {
            //TODO Probably a better way to do this
            Debug.Log($"DatatypeToCreate {DataTypeToCreate}");
            Debug.Log($"VartypeToCreate {VariableTypeToCreate}");
            Variable newVar;
            switch (VariableTypeToCreate) {
                case VariableType.Independent:
                    Type ivType = supportedIndependentTypes[DataTypeToCreate];
                    IndependentVariable newIv = Activator.CreateInstance(ivType) as IndependentVariable;
                    IndependentVariables.Add(newIv);
                    newVar = newIv;
                    break;
                case VariableType.Dependent:
                    Type dvType = supportedDependentTypes[DataTypeToCreate];
                    DependentVariable newDv = Activator.CreateInstance(dvType) as DependentVariable;
                    DependentVariables.Add(newDv);
                    newVar = newDv;
                    break;
               
                case VariableType.Participant:
                    Type pvType = supportedParticipantTypes[DataTypeToCreate];
                    ParticipantVariable newPv = Activator.CreateInstance(pvType) as ParticipantVariable;
                    ParticipantVariables.Add(newPv);
                    newVar = newPv;
                    break;
                case VariableType.ChooseType:
                    newVar = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(VariableTypeToCreate), DataTypeToCreate, null);
            }

            DataTypeToCreate = SupportedDataType.ChooseType;
            VariableTypeToCreate = VariableType.ChooseType;
            
            return newVar;

        }

        // ReSharper disable once CognitiveComplexity
        public void RemoveVariable(Variable variable) {
            //TODO Probably a better way to do this.
            if (variable.DataType == SupportedDataType.ChooseType) 
                throw new InvalidEnumArgumentException("Trying to create new variable, but not types not yet chosen");

            switch (variable.TypeOfVariable) {
                case VariableType.Independent:
                    IndependentVariables.Remove(variable);
                    break;
                case VariableType.Dependent:
                    DependentVariables.Remove(variable);
                    break;
                case VariableType.Participant:
                    ParticipantVariables.Remove(variable);
                    break;
                case VariableType.ChooseType:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}