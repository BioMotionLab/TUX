using System;
using System.IO;

namespace bmlTUX {
	[Serializable]
	public class OutputFile {
		public string OutputFolder;
		public string OutputFileName;
		public string FullPath;

		public OutputFile(string outputFolder, string outputFileName) {
			OutputFolder = outputFolder;
			OutputFileName = outputFileName;
			FullPath = Path.Combine(outputFolder, outputFileName);
			FullPath += ".csv";
		}

		public static OutputFile DebugFile(string debugFolder, string debugFileName) {
			return new OutputFile(debugFolder, debugFileName);
		}

		public void OutputToFile(Outputtable output) {
			
			Directory.CreateDirectory(OutputFolder ?? throw new NullReferenceException("Folder could not be created"));

			using (StreamWriter streamWriter = new StreamWriter(FullPath)) {
				streamWriter.Write(output.AsString);
			}

			ExperimentEvents.OutputSuccessfullyUpdated(FullPath);

		}
		
		
    
        
	}


}