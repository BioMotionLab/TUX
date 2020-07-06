using UnityEngine;
using UnityEngine.Serialization;

namespace Packages.bmlTUX.Extras.Tracker {
    [CreateAssetMenu]
    public class TrackerManager : ScriptableObject {
        public TrackerState state;

        public string saveFolderPath;
    }
}