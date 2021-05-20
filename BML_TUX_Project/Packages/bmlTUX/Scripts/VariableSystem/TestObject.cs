using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {
    [CreateAssetMenu]
    public class TestObject : ScriptableObject {

        public float test;

        public ExperimentDesignFile2 DesignFile2 = default;

    }
}