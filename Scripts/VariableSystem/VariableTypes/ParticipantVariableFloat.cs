using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableFloat : ParticipantVariable<float> {
        public ParticipantVariableFloat() {
            DataType = SupportedDataType.Float;
        }

        public override void SelectValue(string value) {
            Value = Convert.ToSingle(value);
        }
        

        protected override float DefaultValue => default;
    }
}