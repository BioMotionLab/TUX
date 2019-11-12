using System;
using System.IO;
using bmlTUX.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace bmlTUX.Scripts.UI.Runtime {
    public class OutputFilePanel : MonoBehaviour
    {
  
        [SerializeField]
        TMP_InputField OutputFileName = default;
        
        [SerializeField]
        TMP_InputField OutputFolder = default;

     

        public OutputFile GetOutputFile() {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
