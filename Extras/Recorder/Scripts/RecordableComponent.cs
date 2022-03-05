using System;
using System.IO;
using UnityEngine;

namespace bmlTUX.Recorder
{
    [RequireComponent(typeof(Recorder))]
    public abstract class RecordableComponent : MonoBehaviour {
        public abstract Snapshot CreateSnapShot(float time, int frame);

        public abstract bool ShouldUpdate(int frame);
    }

    public abstract class RecordableComponent<T> : RecordableComponent where T : Snapshot {
        
        bool initializedFile = false;
        protected abstract T BuildSnapShot(float time, int frame);
        
        protected RecordingManager RecordingManager;
        
        protected abstract string CsvFileNameSuffix { get; }
        protected abstract string GetHeader();

        protected abstract string DataToCsvLine(float time, int frame);
        
        string filePath;
        
        public sealed override Snapshot CreateSnapShot(float time, int frame) {
            if (RecordingManager.recordingMode == RecordingManager.RecordingMode.CsvOnlyNoPlayback
                || RecordingManager.recordingMode == RecordingManager.RecordingMode.CsvWithOptimizedJson)
            {
                SaveSnapshotToCsv(time, frame);
            }
            return BuildSnapShot(time, frame);
        }
        
        
        void InitializeFile() {
            if (!Directory.Exists(RecordingManager.recordingFolder)) {
                Debug.LogError($"No directory {RecordingManager.recordingFolder} ", this);
                return;
            }

            filePath = Path.Combine(RecordingManager.recordingFolder, $"{gameObject.name}_{CsvFileNameSuffix}.csv");
            filePath = UniqueFileNameGenerator.NextAvailable(filePath);
            string header = GetHeader();
            File.AppendAllText(filePath, header + Environment.NewLine);
            initializedFile = true;
        }
        
        void SaveSnapshotToCsv(float time, int frame) {
            if (!initializedFile) {
                InitializeFile();
            }
            string line = DataToCsvLine(time, frame);
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
    }
}