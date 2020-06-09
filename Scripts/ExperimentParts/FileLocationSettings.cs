using System;
using System.IO;
using UnityEngine;

namespace bmlTUX.Scripts.ExperimentParts {
	
	[CreateAssetMenu(menuName = "bmlTUX/File Location Settings")]
	public class FileLocationSettings : ScriptableObject {

		const string TuxFolderName = "bmlTux";
		const string ScriptingTemplatesFolderName =	"ScriptingTemplates";
		const string PackagePath = "Packages/com.biomotionlab.tux";
		const string ExperimentRunnerTemplateFileName = "ExperimentRunnerTemplate.cs";
		const string TrialTemplateFileName = "TrialScriptTemplate.cs";
		const string BlockTemplateFileName = "BlockScriptTemplate.cs";
		const string ExperimentTemplateFileName = "ExperimentScriptTemplate.cs";

		public static string ExperimentRunnerTemplatePath => Path.Combine(TemplatePath , ExperimentRunnerTemplateFileName);
		public static string TrialTemplatePath => Path.Combine(TemplatePath , TrialTemplateFileName);
		public static string BlockTemplatePath => Path.Combine(TemplatePath , BlockTemplateFileName);
		public static string ExperimentTemplatePath => Path.Combine(TemplatePath , ExperimentTemplateFileName);
		
		static string TemplatePath => Path.Combine(TuxFolderPath, ScriptingTemplatesFolderName);

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

		

		[SerializeField]
		public string LastSessionSaveFileName = default;
		
		[SerializeField]
		string SessionLogFileName = default;
		
		[SerializeField]
		public string DebugFileName = default;
		
		[SerializeField]
		[Header("This folder will show up in your documents folder:")]
		// ReSharper disable once InconsistentNaming
		string tuxFolderName = "bmlTUX_Data";

		
		string BaseTuxFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), tuxFolderName);
		public string SessionFolder => BaseTuxFolderPath;
		public string DebugFolder => BaseTuxFolderPath;
		
		public string LastSessionSaveFilePath => Path.Combine(SessionFolder, LastSessionSaveFileName);
		public string SessionLogFilePath => Path.Combine(SessionFolder, SessionLogFileName);
	}
}