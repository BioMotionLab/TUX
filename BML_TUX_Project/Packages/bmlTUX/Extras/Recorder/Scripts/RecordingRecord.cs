using System;
using System.Collections.Generic;
using UnityEngine;

namespace bmlTUX.Recorder
{
    [Serializable]
    public class RecordingRecord {
    
        [SerializeField] int instantiatedFrame = -1;
        [SerializeField] int destroyedFrame = -1;
    
        public int InstantiatedFrame => instantiatedFrame;
        public int DestroyedFrame => destroyedFrame;
    
        public Ghost Prefab => GhostPrefab;
        public Ghost GhostPrefab;

        [SerializeReference]
        public List<Snapshot> Snapshots;
    

        public RecordingRecord(int startingFrame, Ghost ghostPrefab) {
            instantiatedFrame = startingFrame;
            this.GhostPrefab = ghostPrefab;
            Snapshots = new List<Snapshot>();
        }
    
        public void AddSnapShot(Snapshot snapshot) {
            Snapshots.Add(snapshot);
        }

        public bool ShouldInstantiateThisFrame(int frame) {
            return frame == instantiatedFrame;
        }

        public bool ShouldDestroyThisFrame(int frame) {
            return frame == destroyedFrame;
        }
    
        public void ObjectWasDestroyed(int frame) {
            destroyedFrame = frame;
        }
    
    }
}

