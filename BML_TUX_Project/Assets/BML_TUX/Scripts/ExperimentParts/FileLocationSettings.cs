using UnityEngine;

namespace BML_TUX.Scripts.ExperimentParts {
	public class FileLocationSettings : ScriptableObject {
		public string SessionDataFileName = "LastSessionData.json";
		public string SessionFolder = "bmlTUX/Data";
		public string DebugFolder = "/bmlTUX/Data/Debug";
		public string DebugFileName = "debugSaveFile";
	}
}