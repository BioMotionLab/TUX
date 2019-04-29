using System;
using UnityEngine;
using UnityEngine.Events;

namespace BML_Utilities.EventSystem {
    public class BmlStringEventListener : MonoBehaviour {

        public BmlStringEvent Event;

        [SerializeField]
        public MyStringEvent     Response;

        void OnEnable() {
            Event.RegisterListener(this);
        }

        void OnDisable() {
            Event.UnRegisterListener(this);
        }

        public void OnEventRaised(string text) {
            Response.Invoke(text);
        }
    }

    [Serializable]
    public class MyStringEvent : UnityEvent<string> {
    }
}