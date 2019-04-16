﻿using UnityEngine;
using UnityEngine.UI;

namespace BML_Utilities.ScriptableObject_Assets {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    public class TextAssetToUi : MonoBehaviour {

        public StringValue Text;
        const  string      MissingWarning = "Missing Text Reference";
        Text               text;
        // Start is called before the first frame update
        void Start() {
            text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update() {
            text.text = Text == null ? MissingWarning : Text;
        }
    }
}
