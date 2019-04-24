using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BML_Utilities.EventSystem {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu+"Create BML Event")]
    public class BmlEvent : ScriptableObject {
        readonly List<BmlEventListener> listeners = new List<BmlEventListener>();

        public void Raise() {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(BmlEventListener listener) {
            listeners.Add(listener);
        }

        public void UnRegisterListener(BmlEventListener listener) {
            listeners.Remove(listener);
        }
    }

    [CustomEditor(typeof(BmlEvent))]
    public class BmlEventEditor : Editor {
        public override void OnInspectorGUI() {
            BmlEvent bmlEvent = (BmlEvent) target;
            if (GUILayout.Button("Raise Event")) { 
                bmlEvent.Raise();
            }
        }
    }
}