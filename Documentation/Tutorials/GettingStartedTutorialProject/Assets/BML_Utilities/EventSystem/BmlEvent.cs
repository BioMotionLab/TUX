using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BML_Utilities.EventSystem {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu+"Create BML Event")]
    public class BmlEvent : ScriptableObject {
        
        readonly List<BmlEventListener> listeners = new List<BmlEventListener>();

        public delegate void Listener();

        public event Listener OnRaise;
        
        public void Raise() {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised();
            }
            OnRaise?.Invoke();
        }

        public void RegisterListener(BmlEventListener listener) {
            listeners.Add(listener);
        }

        public void UnRegisterListener(BmlEventListener listener) {
            listeners.Remove(listener);
        }
    }

    [CustomEditor(typeof(BmlEvent))]
    public class BmlEventEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            BmlEvent bmlEvent = (BmlEvent) target;
            if (GUILayout.Button("Raise Event")) { 
                bmlEvent.Raise();
            }
        }
    }
}