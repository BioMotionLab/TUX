using System;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {

    public class IndependentVariables {
        public readonly List<IndependentVariable> Balanced    = new List<IndependentVariable>();
        public readonly List<IndependentVariable> Looped      = new List<IndependentVariable>();
        public readonly List<IndependentVariable> Probability = new List<IndependentVariable>();
    }
    
    
    public class Variables
    {
        public readonly IndependentVariables IndependentVariables = new IndependentVariables();
        public readonly IndependentVariables BlockVariables = new IndependentVariables();
        public readonly List<DependentVariable>   DependentVariables              = new List<DependentVariable>();
        public readonly List<ParticipantVariable> ParticipantVariables = new List<ParticipantVariable>();
        
        
        //Sort Independent variables into mixing categories so they go in order
        public Variables(List<Variable> allVariables) {
            int numberOfBlockIvs = 0;
            int numberOfNonBlockIvs = 0;
            foreach (Variable variable in allVariables) {
                
                
                switch (variable) {
                    case IndependentVariable independentVariable:

                        if (!independentVariable.Block) {
                            numberOfNonBlockIvs++;
                            SortIVs(independentVariable, IndependentVariables);
                        }
                        else {
                            numberOfBlockIvs++;
                            SortIVs(independentVariable, BlockVariables);
                        }
                        break;
                    case DependentVariable dependentVariable:
                        DependentVariables.Add(dependentVariable);
                        break;
                    case ParticipantVariable participantVariable: 
                        ParticipantVariables.Add(participantVariable);
                        break; 
                    default:
                        throw new ArgumentOutOfRangeException();
                        

                }
                
                bool thereAreBlockIvsButNoNormalIvs = numberOfBlockIvs > 0 && numberOfNonBlockIvs == 0;
                if (thereAreBlockIvsButNoNormalIvs) {
                    throw new InvalidExperimentDesignException($"You defined {numberOfBlockIvs} block variable(s), " +
                                                               $"when there are {numberOfNonBlockIvs} unblocked independent variables." +
                                                               "You can safely unblock the variable " +
                                                               "to make it a normal variable");
                }
                
            }

            
        }

        void SortIVs(IndependentVariable independentVariable, IndependentVariables container) {
            switch (independentVariable.MixingTypeOfVariable) {
                case VariableMixingType.Balanced:
                    container.Balanced.Add(independentVariable);
                    break;
                case VariableMixingType.Looped:
                    container.Looped.Add(independentVariable);
                    break;
                case VariableMixingType.EvenProbability:
                case VariableMixingType.CustomProbability:
                    container.Probability.Add(independentVariable);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }
}
