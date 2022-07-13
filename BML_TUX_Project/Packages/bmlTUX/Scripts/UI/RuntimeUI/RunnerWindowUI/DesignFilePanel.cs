using System;
using System.IO;
using TMPro;
using UnityEngine;

namespace bmlTUX.UI.RuntimeUI {
    public class DesignFilePanel : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField DesignFileName = default;

        [SerializeField]
        TMP_InputField OutputFolder = default;


        public void Show() {
            gameObject.SetActive(true);
        }
    
        public InputFile GetInputFile(){
            string folder = FileLocationSettings.SessionFolder;
            folder = Path.Combine(folder, OutputFolder.text);
            InputFile inputFile = new InputFile(folder, DesignFileName.text);

            return inputFile;
        }
    
    }
}
