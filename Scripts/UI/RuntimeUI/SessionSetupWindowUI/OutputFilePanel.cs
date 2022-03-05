using System;
using System.IO;
using TMPro;
using UnityEngine;

namespace bmlTUX.Scripts.UI.RuntimeUI.SessionSetupWindowUI {
    public class OutputFilePanel : MonoBehaviour
    {
  
        [SerializeField]
        TMP_InputField OutputFileName = default;
        
        [SerializeField]
        TMP_InputField OutputFolder = default;

     

        public OutputFile GetOutputFile() {
            string folder = FileLocationSettings.SessionFolder;
            folder = Path.Combine(folder, OutputFolder.text);

            OutputFile outputFile = new OutputFile(folder,
                                                   OutputFileName.text);
            return outputFile;
        }

        public void SetupFields(OutputFile sessionOutputFile) {
            string fileName;
            string folder;
            if (sessionOutputFile == null) {
                fileName = "";
                folder = "";
            }
            else {
                fileName = sessionOutputFile.OutputFileName;
                folder = sessionOutputFile.OutputFolder;
            }
            
            OutputFileName.text = fileName;
            OutputFolder.text = folder;
            
        }
    }
}
