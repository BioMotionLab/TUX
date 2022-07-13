using UnityEngine;

namespace bmlTUX.Recorder
{
    public class Ghost : MonoBehaviour {
    
        PlaybackManager playbackManager;
        GhostComponent[] ghostComponents;
    
        RecordingRecord recordingRecord;

        void OnEnable() {
            playbackManager = FindObjectOfType<PlaybackManager>();
            ghostComponents = GetComponents<GhostComponent>();
            playbackManager.PlaybackStopped += StopPlayback;
            playbackManager.UpdatePlayback += UpdatePlayback;
        }
    

        void StopPlayback() {
            Destroy(this.gameObject);
        }
    
        void OnDisable() {
            playbackManager.PlaybackStopped -= StopPlayback;
            playbackManager.UpdatePlayback -= UpdatePlayback;
        }
    
        public void Init(RecordingRecord record) {
            this.recordingRecord = record;
            foreach (GhostComponent ghostComponent in ghostComponents) {
                ghostComponent.InitGhost(record);
            }
        }

        void UpdatePlayback(float time, int frame) {
        
            if (frame == recordingRecord.DestroyedFrame) {
                Destroy(this.gameObject);
            }
        
            foreach (GhostComponent ghostComponent in ghostComponents) {
                ghostComponent.UpdateGhost(time, frame);
            }
        
        
        }


    
    }
}