using System;
using System.IO;
using UnityEngine;

namespace bmlTUX.Scripts.ExperimentParts {
	
	[CreateAssetMenu(menuName = "BmlTUX/File Location Settings")]
	public class FileLocationSettings : ScriptableObject {
		public string LastSessionSaveFileName = default;
		
		[SerializeField]
		string SessionLogFileName = default;
		
		[SerializeField]
		[Header("this is relative to your documents folder:")]
		// ReSharper disable once InconsistentNaming
		string sessionFolder = default;
		
		
		[SerializeField]
		[Header("this is relative to your documents folder:")]
		// ReSharper disable once InconsistentNaming
		string debugFolder = default;
		
		public string DebugFileName = default;

		public string DebugFolder {
			get {
				string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
				return Path.Combine(path, debugFolder);
			}
		} 
		
		public string SessionFolderWithDocuments {
			get {
				string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
				return Path.Combine(path, sessionFolder);
			}
		}

		public string LastSessionSaveFilePath => Path.Combine(SessionFolderWithDocuments, LastSessionSaveFileName);
		public string SessionLogFilePath => Path.Combine(SessionFolderWithDocuments, SessionLogFileName);
	}
}