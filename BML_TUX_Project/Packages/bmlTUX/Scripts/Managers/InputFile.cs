using System.IO;

namespace bmlTUX{
    public class InputFile{
        public string InputFolder;
        public string InputFilename;
        public string FullPath;

        public InputFile(string inputFolder, string inputFilename){
            InputFolder = inputFolder;
            InputFilename = inputFilename;
            FullPath = Path.Combine(inputFolder, inputFilename);
        }
    }
}
