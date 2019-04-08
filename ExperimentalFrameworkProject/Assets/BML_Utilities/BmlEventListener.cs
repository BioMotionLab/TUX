using UnityEngine;
using UnityEngine.Events;

namespace BML_Utilities {
    public class BmlEventListener : MonoBehaviour {
        public BmlEvent   Event;
        public UnityEvent Response;

        void OnEnable() {
            Event.RegisterListener(this);
        }

        void OnDisable() {
            Event.UnRegisterListener(this);
        }

        public void OnEventRaised() {
            Response.Invoke();
        }
    }
}