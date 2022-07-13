using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace bmlTUX.Recorder
{
    [RequireComponent(typeof(RecordingManager))]
    public class PlaybackManager : MonoBehaviour
    {
        public bool IsPlayingBack = false;
        
        public string PlaybackFolder;

        List<RecordingRecord> records;
        int savedFrames;
        
        RecordingManager recordingManager;

        public delegate void ChangePlaybackStateEvent();
        public event ChangePlaybackStateEvent PlaybackStarted;
        public event ChangePlaybackStateEvent PlaybackStopped;
        public event ChangePlaybackStateEvent PlaybackCompleted;

        
        public delegate void UpdatePlaybackStateEvent(float time, int frame);
        public event UpdatePlaybackStateEvent UpdatePlayback;
        
        int currentFrame;
        List<GameObject> instantiatedPrefabs = new List<GameObject>();

        void Awake()
        {
            recordingManager = GetComponent<RecordingManager>();
            recordingManager.RecordingStarted += StopPlayback;
            
        }


        [ContextMenu("Start Playback")]
        public void StartPlayBackFromSelectedFolder()
        {
            string fullPath = Path.Combine(PlaybackFolder, RecordingManager.RecordingFileNameNoExtenstion + ".json");
            StartPlayback(fullPath);
        }
        
        /// <summary>
        /// Starts Playback From the given folder
        /// </summary>
        /// <param name="fullPath"></param>
        public void StartPlayback(string fullPath) {
            if (recordingManager.IsRecording)
            {
                Debug.LogWarning("Can't start playing back while recording in progress");
                return;
            }
            LoadPlayBackFile(fullPath);
            
            if (savedFrames == 0) {
                Debug.LogWarning("No Frames To playback", this);
            }
        
            currentFrame = 0;
            IsPlayingBack = true;
            PlaybackStarted?.Invoke();
        }
    
        /// <summary>
        /// Stops playback
        /// </summary>
        [ContextMenu("Stop Playback")]
        public void StopPlayback() {
            if (!IsPlayingBack) return;
            IsPlayingBack = false;
            currentFrame = 0;
            PlaybackStopped?.Invoke();

            foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
            {
                Destroy(instantiatedPrefab);
            }
        }

        void FixedUpdate()
        {
            if (IsPlayingBack)  return;
            
            
            if (IsPlayingBack) {
                foreach (RecordingRecord record in records) {
                    if (record.ShouldInstantiateThisFrame(currentFrame)) {
                        GameObject instantiatedGhostPrefab = Instantiate(record.Prefab.gameObject);
                        instantiatedPrefabs.Add(instantiatedGhostPrefab);
                        Ghost ghost = instantiatedGhostPrefab.GetComponent<Ghost>();
                        ghost.Init(record);
                    }
                }
                UpdatePlayback?.Invoke(Time.time, currentFrame);

                currentFrame++;
                if (currentFrame >= savedFrames)
                {
                    PlaybackCompleted?.Invoke();
                    StopPlayback();
                }
            }
        }
        
        [ContextMenu("Start Playback")]
        void LoadPlayBackFile(string fullPath) {
            if (!Directory.Exists(PlaybackFolder)) {
                Debug.LogError($"Can't find directory for {nameof(PlaybackFolder)}: {PlaybackFolder}");
                return;;
            }
            
            if (File.Exists(fullPath)) {
                Debug.Log($"Loading File... {fullPath}");
                RecordingSaveData saveData = ReadRecordFile(fullPath);
                Debug.Log($"Done Loading. {fullPath}");
                records = saveData.Records;
                savedFrames = saveData.totalFrames;
            }
            else {
                Debug.LogWarning($"No Save file at path {fullPath}", this);
            }
        }
    
        RecordingSaveData ReadRecordFile(string fullPath) {
            Debug.Log($"Reading...");
            string json = File.ReadAllText(fullPath);
            Debug.Log($"Processing...");
            RecordingSaveData saveData = JsonUtility.FromJson<RecordingSaveData>(json);
            return saveData;
        }
        
    }
}