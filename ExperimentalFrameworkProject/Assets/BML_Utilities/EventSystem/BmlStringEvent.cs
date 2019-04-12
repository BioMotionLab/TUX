﻿using System.Collections.Generic;
using UnityEngine;

namespace BML_Utilities.EventSystem {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Create BML String Event")]
    public class BmlStringEvent : ScriptableObject {

        readonly List<BmlStringEventListener> listeners = new List<BmlStringEventListener>();

        public void Raise(string text) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised(text);
            }
        }

        public void RegisterListener(BmlStringEventListener listener) {
            listeners.Add(listener);
        }

        public void UnRegisterListener(BmlStringEventListener listener) {
            listeners.Remove(listener);
        }
    }

    
}