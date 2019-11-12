using System;
using System.IO;
using UnityEngine;

namespace BML_TUX.Scripts.Managers {
	[Serializable]
	public class OutputFile {
		public string outputFolder;
		public string outputFileName;
		public string fullPath;

		public OutputFile(string outputFolder, string outputFileName) {
			this.outputFolder = outputFolder;
			this.outputFileName = outputFileName;
		}
		
		public void OutputToFile(Outputtable output) {
			Debug.Log($"Writing output file: {fullPath}");
			string folder = Path.GetDirectoryName(outputFolder);
			Directory.CreateDirectory(folder ?? throw new NullReferenceException("Folder could not be created"));

			using (StreamWriter streamWriter = new StreamWriter(fullPath)) {
				streamWriter.Write(output.AsString);
			}
		}
		
		
    
        
	}


}