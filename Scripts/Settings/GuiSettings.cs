﻿using bmlTUX.Scripts.UI.Runtime;
using UnityEngine;

namespace bmlTUX.Scripts.Settings {
    [CreateAssetMenu(menuName = TUXMenuNames.AssetCreationMenu + "GUI Settings File")]
    public class GuiSettings : ScriptableObject {
        
        public ExperimentGui GuiPrefab;

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        DisplaySelection targetDisplay = default;

        // ReSharper disable once ConvertToAutoProperty
        public int TargetDisplay => (int)targetDisplay;

    }
}
