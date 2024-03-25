using System;
using System.IO;
using UnityEngine;

namespace bmlTUX {
  [Serializable]
	public class FileLocationSettings  {
        [Tooltip("Not every device supports absolute paths. Only advised for Builds on Windows and Mac.\nIf unchecked, Document Folder is used as root.")]
        public bool dataFolderAsAbsolutePath = false;
        public string dataFolderName = "bmlTUX_Data";                
        public string SessionSaveFileName = "lastSession.json";  
        public string SessionLogFileName = "sessionLog.txt";

        public string BaseTuxDocumentsFolderPath => dataFolderAsAbsolutePath ?
            dataFolderName :
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dataFolderName);

		public string SessionFolder => BaseTuxDocumentsFolderPath;
		public string DebugFolder => BaseTuxDocumentsFolderPath;
        public string DebugFileName = "debugSaveFile";

        public string SessionSaveFilePath => Path.Combine(SessionFolder, SessionSaveFileName);
        public string SessionLogFilePath => Path.Combine(SessionFolder, SessionLogFileName);

        public static string TuxFolderPath
        {
            get
            {
                string TuxFolderName = "bmlTux";
                string PackagePath = "Packages/com.biomotionlab.tux";

                string tuxPath;
                string tuxPackagePath = "";
                string tuxAssetPath = Path.Combine(Application.dataPath, TuxFolderName);
                tuxPath = tuxAssetPath;
                if (!Directory.Exists(tuxPath))
                {
                    tuxPackagePath = Path.GetFullPath(PackagePath);
                    tuxPath = tuxPackagePath;
                }

                if (!Directory.Exists(tuxPath))
                {
                    throw new DirectoryNotFoundException($"Can't find bmlTUX directory. Tried: {tuxAssetPath}, and {tuxPackagePath}");
                }

                return tuxPath;
            }
        }
        const string ScriptingTemplatesFolderName = "ScriptingTemplates";
        public static string TemplatePath => Path.Combine(TuxFolderPath, ScriptingTemplatesFolderName);

    }
}