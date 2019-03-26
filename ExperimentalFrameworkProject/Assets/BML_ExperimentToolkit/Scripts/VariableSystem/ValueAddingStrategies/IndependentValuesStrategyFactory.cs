using System;
using BML_ExperimentToolkit.Scripts.ExperimentParts;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.ValueAddingStrategies {
    internal class IndependentValuesStrategyFactory {

        public static IndependentValuesStrategy<T> Create<T>(VariableMixingType mixingTypeOfVariable) {

            switch (mixingTypeOfVariable) {
                case VariableMixingType.Balanced:
                    return new BalancedStrategy<T>();
                case VariableMixingType.Looped:
                    return new LoopedStrategy<T>();
                case VariableMixingType.EvenProbability:
                    return new EvenProbabilityStrategy<T>();
                case VariableMixingType.CustomProbability:
                    return new CustomProbabilityStrategy<T>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mixingTypeOfVariable), mixingTypeOfVariable, null);
            }
        }
    }
}