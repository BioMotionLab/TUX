﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace bmlTUX.Scripts.VariableSystem {

    public class IndependentVariables {
        public readonly List<IndependentVariable> Balanced    = new List<IndependentVariable>();
        public readonly List<IndependentVariable> Looped      = new List<IndependentVariable>();
        public readonly List<IndependentVariable> Probability = new List<IndependentVariable>();
        
        public static implicit operator List<IndependentVariable>(IndependentVariables variables) {
            List<IndependentVariable> combined = new List<IndependentVariable>();
            combined.AddRange(variables.Balanced);
            combined.AddRange(variables.Looped);
            combined.AddRange(variables.Probability);
            return combined;
        }
    }
    
    
    public class Variables
    {
        public readonly IndependentVariables IndependentVariables = new IndependentVariables();
        public readonly IndependentVariables BlockVariables = new IndependentVariables();
        public readonly List<DependentVariable>   DependentVariables              = new List<DependentVariable>();
        public readonly List<ParticipantVariable> ParticipantVariables = new List<ParticipantVariable>();
        readonly List<Variable> all;
        
        //Sort Independent variables into mixing categories so they go in order
        public Variables(List<Variable> allVariables) {

            all = allVariables;
            
            int numberOfBlockIvs = 0;
            int numberOfNonBlockIvs = 0;
            foreach (Variable variable in allVariables) {
                numberOfNonBlockIvs = SortVariable(variable, numberOfNonBlockIvs, ref numberOfBlockIvs);
            }
            CheckIfBlocksButNoNormalVariables(numberOfBlockIvs, numberOfNonBlockIvs);
            
        }

        int SortVariable(Variable variable, int numberOfNonBlockIvs, ref int numberOfBlockIvs) {
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

            return numberOfNonBlockIvs;
        }

        static void CheckIfBlocksButNoNormalVariables(int numberOfBlockIvs, int numberOfNonBlockIvs) {
            bool thereAreBlockIvsButNoNormalIvs = numberOfBlockIvs > 0 && numberOfNonBlockIvs == 0;
            if (thereAreBlockIvsButNoNormalIvs) {
                throw new InvalidExperimentDesignException($"You defined {numberOfBlockIvs} block variable(s), " +
                                                           $"when there are {numberOfNonBlockIvs} unblocked independent variables." +
                                                           "You can safely unblock the variable " +
                                                           "to make it a normal variable");
            }
        }

        void SortIVs(IndependentVariable independentVariable, IndependentVariables container) {
            switch (independentVariable.MixingType) {
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


        public Variable GetVariableWithName(string name) {
            return all.FirstOrDefault(variable => variable.Name == name);
        }
    }
}
