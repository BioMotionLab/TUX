using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using bmlTUX.Scripts.UI.RuntimeUI.UIUtilities;

namespace bmlTUX.Scripts.Managers {
	public class OutputFileValidationResult : InputValidator {
		readonly string outputFolder;
		readonly string outputFileName;

		public List<string> Errors { get; }

		public bool Valid { get; private set; }

		readonly string fullPath;

		public OutputFileValidationResult(OutputFile file) {
			Errors = new List<string>();
			Valid = true;
			outputFolder = file.OutputFolder;
			outputFileName = file.OutputFileName;

			string outputFullPath = Path.Combine(outputFolder, outputFileName);
			
			if (!Path.HasExtension(outputFolder)) {
				string extension = ".csv";
				outputFullPath += extension;
			}

			fullPath = outputFullPath;
			
			ValidateFolder();
			ValidateFileName ();
			ValidateFileDoesNotExist();
			
		}

		void ValidateFolder() {
			if (string.IsNullOrEmpty(outputFolder)) {
				Errors.Add($"Output Folder name not set or too short. {outputFolder}");
				Valid = false;
			}
		}
        
		void ValidateFileName() {
			if (string.IsNullOrEmpty(outputFileName)) {
				Errors.Add($"Output File name not set or too short. {outputFileName}");
				Valid = false;
				return;
			}
            
			if (!IsAllNumbersAndLetters(outputFileName)) {
				Errors.Add($"Output File name contains invalid characters. {outputFileName}");
				Valid = false;
			}
		}

		static bool IsAllNumbersAndLetters(string text) {
			return Regex.IsMatch(text, @"^[a-zA-Z0-9_]+$");
		}
        
		void ValidateFileDoesNotExist() {
			if (!File.Exists(fullPath)) return;
			Errors.Add($"Output File Already Exists @ {fullPath}");
			Valid = false;
		}
	}
}