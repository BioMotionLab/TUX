using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace bmlTUX {
    [Serializable]
    public class GuiSettings  {

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        DisplaySelection targetDisplay = DisplaySelection.Display1;

        // ReSharper disable once ConvertToAutoProperty
        public int TargetDisplay => (int)targetDisplay;
        
        [SerializeField]
        public bool WarnUserIfNotDisplayOne = true;

        [FormerlySerializedAs("ShowRunnerInterface")]
        [SerializeField]
        public bool ShowRunnerInterfaceAfterStart = true;
    }
}
