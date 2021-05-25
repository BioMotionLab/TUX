using System;
using System.IO;
using UnityEngine;

namespace bmlTUX.Scripts.ExperimentParts {
	
	public static class FileLocationSettings  {

		public const string TuxFolderName = "bmlTux";
		public const string ScriptingTemplatesFolderName =	"ScriptingTemplates";
		public const string PackagePath = "Packages/com.biomotionlab.tux";
		

		public const string LastSessionSaveFileName = "lastSession.json";
		public const string SessionLogFileName = "sessionLog.txt";
		public const  string DebugFileName = "debugSaveFile";
		public const string tuxFolderName = "bmlTUX_Data";
		
		
		public static string TemplatePath => Path.Combine(TuxFolderPath, ScriptingTemplatesFolderName);

		public static string TuxFolderPath {
			get {
				string tuxPath;
				string tuxPackagePath = "";
				string tuxAssetPath = Path.Combine(Application.dataPath, TuxFolderName);
				tuxPath = tuxAssetPath;
				if (!Directory.Exists(tuxPath)) {
					tuxPackagePath = Path.GetFullPath(PackagePath);
					tuxPath = tuxPackagePath;
				}
				
				if (!Directory.Exists(tuxPath)) {
					throw new DirectoryNotFoundException($"Can't find bmlTUX directory. Tried: {tuxAssetPath}, and {tuxPackagePath}");
				}

				return tuxPath;
			}
		}

		
		
		public static string BaseTuxDocumentsFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), tuxFolderName);
		public static string SessionFolder => BaseTuxDocumentsFolderPath;
		public static string DebugFolder => BaseTuxDocumentsFolderPath;
		
		public static string LastSessionSaveFilePath => Path.Combine(SessionFolder, LastSessionSaveFileName);
		public static string SessionLogFilePath => Path.Combine(SessionFolder, SessionLogFileName);
	}
}