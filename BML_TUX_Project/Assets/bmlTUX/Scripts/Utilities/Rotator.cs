using UnityEngine;

namespace BML_Utilities {
    public class Rotator : MonoBehaviour {

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        float speed = default;

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        bool x = default;
        
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        bool y = default;
        
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        bool z = default;

        Vector3 eulers;
        
        void Start() {
            eulers = transform.localEulerAngles;
        }

        // Update is called once per frameInterface
        void Update() {
            float amt = speed * Time.deltaTime;
            
            if (x) {
                eulers += new Vector3(amt, 0, 0);
            }
            
            if (y) {
                eulers += new Vector3(0, amt, 0);
            }
            
            if (z) {
                eulers += new Vector3(0, 0, amt);
            }

            transform.localEulerAngles = eulers;
        }
    }
}
