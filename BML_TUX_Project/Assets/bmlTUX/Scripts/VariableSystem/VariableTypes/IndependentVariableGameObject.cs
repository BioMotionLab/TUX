using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {

    [Serializable]
    public class IndependentVariableGameObject : IndependentVariable<GameObject> {
        public IndependentVariableGameObject() {
            DataType = SupportedDataType.GameObject;
        }
    }
}