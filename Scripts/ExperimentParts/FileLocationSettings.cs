using System;
using System.IO;
using UnityEngine;

namespace bmlTUX.Scripts.ExperimentParts {
	
	[CreateAssetMenu(menuName = "bmlTUX/File Location Settings")]
	public class FileLocationSettings : ScriptableObject {

		const string TuxProjectFolderPath = "/bmlTux/ScriptingTemplates/";
		
		public static string TemplatePath => Application.dataPath + TuxProjectFolderPath;
		
		
		
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