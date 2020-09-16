using UnityEngine;

namespace Packages.bmlTUX.Extras.Tracker {
    [CreateAssetMenu]
    public class TrackerManager : ScriptableObject {
        public TrackerState state;

        public string saveFolderPath;
    }
}