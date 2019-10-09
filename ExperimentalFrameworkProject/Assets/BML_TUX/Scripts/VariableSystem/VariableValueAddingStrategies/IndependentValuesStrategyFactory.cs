using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableValueAddingStrategies {

    internal class IndependentValuesStrategyFactory {

        public static IndependentVariableValuesAdderStrategy<T> Create<T>(VariableMixingType mixingTypeOfVariable) {

            switch (mixingTypeOfVariable) {
                case VariableMixingType.Balanced:
                    return new BalancedAdderStrategy<T>();
                case VariableMixingType.Looped:
                    return new LoopedAdderStrategy<T>();
                case VariableMixingType.EvenProbability:
                    return new EvenProbabilityAdderStrategy<T>();
                case VariableMixingType.CustomProbability:
                    return new CustomProbabilityAdderStrategy<T>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mixingTypeOfVariable), mixingTypeOfVariable, null);
            }
        }
    }
}