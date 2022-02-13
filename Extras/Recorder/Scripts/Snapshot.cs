using System;
using UnityEngine;

namespace bmlTUX.Recorder
{
    [Serializable]
    public abstract class Snapshot {
    
        [SerializeField] public float TimeStamp;
        [SerializeField] public int FrameStamp;

        protected Snapshot(float timeStamp, int frameStamp) {
            TimeStamp = timeStamp;
            FrameStamp = frameStamp;
        }
    
    }

    [Serializable]
    public abstract class Snapshot<T> : Snapshot {
    
        protected Snapshot(float timeStamp, int frameStamp) : base(timeStamp, frameStamp) { }
    
    }
}