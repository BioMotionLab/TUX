using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Serialization;

namespace Packages.bmlTUX.Extras.Tracker {
    public class TrackedObject : MonoBehaviour {

        bool tracking;
    
        Transform cachedTransform;
        int frame = 0;
        int playBackIndex = 0;
        List<TransformSnapshot> records = new List<TransformSnapshot>();

        [SerializeField] TrackerManager trackerManager = default;
        
        [Range(1,100)]
        [SerializeField] int lossyCompressionFactor = 1;
        

        [SerializeField] bool beginStopped = false;
        
        void Awake() {
            cachedTransform = GetComponent<Transform>();
            switch (trackerManager.state) {
                case TrackerState.Recording:
                    Debug.Log($"Tracking object {gameObject.name} every {lossyCompressionFactor} frame(s)");
                    tracking = !beginStopped;
                    break;
                case TrackerState.Playback:
                    records = LoadRecordsFromTrackFile();
                    Debug.Log($"Playing back tracked object {gameObject.name} every {lossyCompressionFactor} frame(s), {records.Count} recorded frames");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        List<TransformSnapshot> LoadRecordsFromTrackFile() {
            string fullPath = GetFullFilePath();
            if (!File.Exists(fullPath)) throw new NullReferenceException($"Track file does not exist {fullPath}");
            List<TransformSnapshot> loadedRecords = new List<TransformSnapshot>();
            using (StreamReader reader = new StreamReader(fullPath)) {
                bool firstLine = true;
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    if (firstLine) {
                        firstLine = false;
                        continue; // skip header
                    }
                    TransformSnapshot snapshot = TransformSnapshot.FromCSVLine(line);
                    loadedRecords.Add(snapshot);
                }
            }
            return loadedRecords;
        }

        void LateUpdate() {
            switch (trackerManager.state) {
                case TrackerState.Recording:
                    UpdateTrackingRecording();
                    break;
                case TrackerState.Playback:
                    UpdateTrackingPlayback();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            frame++;
        }

        void UpdateTrackingPlayback() {
            if (IsAnUpdateFrame) {
                if (playBackIndex >= records.Count) return;
                TransformSnapshot snapshot = records[playBackIndex];
                playBackIndex++;
                snapshot.CopyValuesToTransform(cachedTransform);
            }

        }

        bool IsAnUpdateFrame => frame % lossyCompressionFactor == 0 || frame == 0;
        

        void UpdateTrackingRecording() {
            if (!tracking || !IsAnUpdateFrame) return;
            TransformSnapshot snapshot = new TransformSnapshot(cachedTransform, frame, Time.time);
            records.Add(snapshot);
        }

        void OnDestroy() {
            if (trackerManager.state == TrackerState.Recording) {
                WriteToRecordFile();
            }
            
        }

        void WriteToRecordFile() {
            string fullFilePath = GetFullFilePath();
            Debug.Log($"Writing tracking log {fullFilePath}");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(TransformSnapshot.CsvHeader);
            foreach (TransformSnapshot record in records) {
                sb.AppendLine(record.ToCsvRow);
            }
            File.WriteAllText(fullFilePath, sb.ToString());
        }

        string GetFullFilePath() {
            string fileName = string.Concat(gameObject.name.Where(c => !char.IsWhiteSpace(c))) + "_track.csv";
            string fullFilePath = Application.dataPath + "/" + fileName;
            return fullFilePath;
        }

        public void StartTracking() {
            tracking = true;
        }

        public void StopTracking() {
            tracking = false;
        }

        [ContextMenu("Set State to Recording")]
        public void TurnOnRecording() {
            trackerManager.state = TrackerState.Recording;
        }

        [ContextMenu("Set State to Playback")]
        public void TurnOnPlayback() {
            trackerManager.state = TrackerState.Playback;
        }
    }

  
}