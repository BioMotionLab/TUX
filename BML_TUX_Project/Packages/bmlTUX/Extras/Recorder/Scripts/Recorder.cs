using System;
using UnityEngine;

namespace bmlTUX.Recorder
{
    public class Recorder : MonoBehaviour {

        [SerializeField] Resolution resolution;
        [SerializeField] Ghost ghostPrefab;
    
        RecordingManager recordingManager;
        RecordingRecord recordingRecord;

        RecordableComponent[] recordables;

        void OnEnable() {
            recordingManager = FindObjectOfType<RecordingManager>();
        
            recordables = GetComponents<RecordableComponent>();
        
            if (recordingManager != null) {
                recordingManager.RecordingStarted += StartRecording;
                recordingManager.RecordingStopped += StopRecording;
                recordingManager.UpdateRecording += UpdateRecording;
            
            
                //if instantiated from elsewhere and missed initial recording start.
                if (recordingManager.IsRecording) {
                    StartRecording();
                }
            }
            else {
                this.enabled = false;
                foreach (RecordableComponent recordableComponent in recordables) {
                    recordableComponent.enabled = false;
                }
            }

        
        
        }

        void OnDisable() {
            if (recordingManager != null) {
                recordingManager.RecordingStarted -= StartRecording;
                recordingManager.RecordingStopped -= StopRecording;
                recordingManager.UpdateRecording -= UpdateRecording;
            }
        }

        void OnDestroy() {
            if (recordingManager != null && recordingManager.IsRecording) {
                recordingRecord.ObjectWasDestroyed(recordingManager.CurrentFrame);
            }
        }

        void UpdateRecording(float time, int frame) {
            bool updateThisFrame = false;
            switch (resolution) {
                case Resolution.EveryFrame:
                    updateThisFrame = true;
                    break;
                case Resolution.OddFrames:
                    updateThisFrame = frame % 2 == 1;
                    break;
                case Resolution.EvenFrames:
                    updateThisFrame = frame % 2 == 0;
                    break;
                case Resolution.Every5Frames:
                    updateThisFrame = frame % 5 == 0;
                    break;
                case Resolution.Every10Frames:
                    updateThisFrame = frame % 10 == 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (frame == 0 || updateThisFrame) {
                foreach (RecordableComponent recordableComponent in recordables) {
                    if (recordableComponent.ShouldUpdate(frame)) {
                        Snapshot snapshot = recordableComponent.CreateSnapShot(time, frame);
                        recordingRecord.AddSnapShot(snapshot);
                    }
                }
            }
        
        }
    
        void StartRecording() {
            recordingRecord = new RecordingRecord(recordingManager.CurrentFrame, ghostPrefab);
            recordingManager.Records.Add(recordingRecord);
        }

        void StopRecording() {
        
        }

        enum Resolution {
            EveryFrame,
            OddFrames,
            EvenFrames,
            Every5Frames,
            Every10Frames,
        }
    }
}

