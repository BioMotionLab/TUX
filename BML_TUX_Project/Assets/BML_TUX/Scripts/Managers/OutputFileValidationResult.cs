using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BML_TUX.Scripts.UI.Runtime;

namespace BML_TUX.Scripts.Managers {
	public class OutputFileValidationResult : InputValidator {
		readonly string outputFolder;
		readonly string outputFileName;
		
		bool valid;
		List<String> errors;
		
		public List<string> Errors => errors;
		public bool Valid => valid;
		
		OutputFile file;
		string fullPath;

		public OutputFileValidationResult(OutputFile file) {
			errors = new List<string>();
			valid = true;
			this.file = file;
			this.outputFolder = file.outputFolder;
			this.outputFileName = file.outputFileName;

			string outputFullPath = Path.Combine(outputFolder, outputFileName);
			
			if (!Path.HasExtension(outputFolder)) {
				string extension = ".csv";
				outputFullPath += extension;
			}

			this.fullPath = outputFullPath;
			
			ValidateFolder();
			ValidateFileName ();
			ValidateFileDoesNotExist();
			
		}

		void ValidateFolder() {
			if (string.IsNullOrEmpty(outputFolder)) {
				errors.Add($"Output Folder name not set or too short. {outputFolder}");
				valid = false;
			}
		}
        
		void ValidateFileName() {
			if (string.IsNullOrEmpty(outputFileName)) {
				errors.Add($"Output File name not set or too short. {outputFileName}");
				valid = false;
				return;
			}
            
			if (!IsAllNumbersAndLetters(outputFileName)) {
				errors.Add($"Output File name contains invalid characters. {outputFileName}");
				valid = false;
			}
		}

		static bool IsAllNumbersAndLetters(string text) {
			return Regex.IsMatch(text, @"^[a-zA-Z0-9_]+$");
		}
        
		void ValidateFileDoesNotExist() {
			if (!File.Exists(fullPath)) return;
			errors.Add($"Output File Already Exists @ {fullPath}");
			valid = false;
		}
		
		
		public OutputFile GetValidFile {
			get {
				if (!valid)
					throw new CallingInvalidFileException("File not valid. Always check that file is valid first");
				return file;
			}
		}

	
	}
}