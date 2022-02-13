using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using bmlTUX.Scripts.UI.RuntimeUI.UIUtilities;

namespace bmlTUX
{
    public class DesignFileValidationResult : InputValidator
    {
        readonly string inputFileName;
        public List<string> Errors { get; }
        
        public bool Valid { get; private set; }

        readonly string fullPath;


        public DesignFileValidationResult(InputFile file){
            Errors = new List<string>();
            Valid = true;

            inputFileName = file.InputFilename;
            fullPath = file.FullPath;

            ValidateFileDoesExist();
            ValidateExtension();
        }


        void ValidateFileDoesExist() {
            if (!File.Exists(fullPath)){
                Errors.Add($"Design File does not exist @ {fullPath}");
                Valid = false;
            }
        }

        void ValidateExtension(){
            if (!Path.HasExtension(fullPath)){
                Errors.Add($"Design File has no extension. Requires '.csv'. {inputFileName}");
                Valid = false;
                return;
            }
            if (Path.GetExtension(fullPath) != ".csv"){
                Errors.Add($"Design File has the wrong extension. Requires '.csv'. {inputFileName}");
                Valid = false;
            }
        }
    }
}
