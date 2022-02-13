using System.Collections.Generic;
using UnityEngine;

namespace bmlTUX.Recorder
{
    [RequireComponent(typeof(Ghost))]
    public abstract class GhostComponent : MonoBehaviour {
    
        public abstract void UpdateGhost(float time, int frame);
        public abstract void InitGhost(RecordingRecord record);
    }

    public abstract class GhostComponent<T> : GhostComponent where T : Snapshot {
    
        List<T> savedSnapshots;

        public override void UpdateGhost(float time, int frame) {
            foreach (T snapshot in savedSnapshots) {
                if (snapshot.FrameStamp == frame) {
                    ApplySnapshot(snapshot);
                }
            }
        }

        protected abstract void ApplySnapshot(T savedSnapshot);


        public override void InitGhost(RecordingRecord record) {
            savedSnapshots = new List<T>();
            foreach (Snapshot genericSnapshot in record.Snapshots) {
                if (genericSnapshot is T castedSnapshot) {
                    savedSnapshots.Add(castedSnapshot);
                }
            }
        }
    }
}