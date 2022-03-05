using System;
using System.Collections.Generic;
using UnityEngine;

namespace bmlTUX.Recorder
{
    [Serializable]
    public class RecordingSaveData {

        public int totalFrames;
    
        [SerializeReference]
        public List<RecordingRecord> Records;

        public RecordingSaveData(List<RecordingRecord> records, int totalFrames) {
            Records = records;
            this.totalFrames = totalFrames;
        }
    }
}