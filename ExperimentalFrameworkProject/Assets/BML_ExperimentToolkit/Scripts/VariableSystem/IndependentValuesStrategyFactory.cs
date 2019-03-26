using System;
using BML_ExperimentToolkit.Scripts.ExperimentParts;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    internal class IndependentValuesStrategyFactory {

        public static AddIndependentValuesStrategy<T> Create<T>(VariableMixingType mixingTypeOfVariable) {

            switch (mixingTypeOfVariable) {
                case VariableMixingType.Balanced:
                    return new AddBalancedValuesStrategy<T>();
                case VariableMixingType.Looped:
                    return new AddLoopedValuesStrategy<T>();
                case VariableMixingType.EvenProbability:
                    return new AddEvenProbabilityValuesStrategy<T>();
                case VariableMixingType.CustomProbability:
                    return new AddCustomProbabilityValuesStrategy<T>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mixingTypeOfVariable), mixingTypeOfVariable, null);
            }
        }
    }
}