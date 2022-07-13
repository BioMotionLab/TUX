using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace bmlTUX.Recorder
{
    
    public class RecordingManager : MonoBehaviour {

        [SerializeField] public bool recordOnStartup = false;
        [SerializeField] public RecordingMode recordingMode = RecordingMode.OptimizedJsonForPlayback;
        public bool IsRecording = false;
        
        int currentFrame;
        int savedFrames;

        public string recordingFolderInInspector;
        public string recordingFolder;
        public const string RecordingFileNameNoExtenstion = "MainOutput";
        
        
    
        List<RecordingRecord> records;

        public List<RecordingRecord> Records => records;

        public int CurrentFrame => currentFrame;


        static RecordingManager instance;
    

        void Awake() {
            if (instance != null) Destroy(this.gameObject);
            if (instance == null) instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnEnable()
        {

            Application.wantsToQuit += () =>
            {
                
                if (IsRecording)
                {
                    Debug.Log("Wants to quit... stopping recording?");
                    StopRecording();
                }
                return true;
            };

        }
        
        

        void Start() {
            if (recordOnStartup) {
                StartRecording(recordingFolderInInspector);
            }
        }

        public delegate void ChangeRecordingStateEvent();

        public event ChangeRecordingStateEvent RecordingStarted;
        public event ChangeRecordingStateEvent RecordingStopped;
    
        public delegate void UpdateRecordingStateEvent(float time, int frame);

        public event UpdateRecordingStateEvent UpdateRecording;



        [ContextMenu("Start Recording")]
        public void StartRecordingInFolderSelectedInInspector()
        {
            StartRecording(recordingFolderInInspector);
        }
        
        [PublicAPI]
        public void StartRecording(string recordingFolderPath)
        {
            if (IsRecording) return;
            
            recordingFolder = recordingFolderPath;
            currentFrame = 0;
            IsRecording = true;

            records = new List<RecordingRecord>();
            RecordingStarted?.Invoke();
        }
    
        [ContextMenu("Stop Recording")]
        public void StopRecording() {
            if (!IsRecording) return;
            IsRecording = false;
            savedFrames = CurrentFrame-1;
            currentFrame = 0;
            RecordingStopped?.Invoke();
            SaveRecordingJson();
        }
    
        
        void OnDestroy()
        {
            Debug.Log("recording manager destroyed");
            if (IsRecording)
            {
                Debug.Log("still recording, so stopping");
                StopRecording();
            }
        }

        void OnDisable()
        {
            Debug.Log("recording manager disabled");
            if (IsRecording)
            {
                Debug.Log("still recording, so stopping");
                StopRecording();
            }
        }

        void FixedUpdate() {

            if (IsRecording)
            {
                UpdateRecording?.Invoke(Time.time, CurrentFrame);
                currentFrame++;
            }
            
        }

        [ContextMenu("Save to File")]
        void SaveRecordingJson()
        {

            if (recordingMode == RecordingMode.CsvOnlyNoPlayback) return;
            
            if (records == null || records.Count == 0) {
                Debug.LogWarning("No records to save", this);
                return;
            }
            if (!Directory.Exists(recordingFolder)) {
                Debug.LogError($"Can't find directory for {nameof(recordingFolder)}: {recordingFolder}");
                return;
            }
            string fullPath = Path.Combine(recordingFolder, RecordingFileNameNoExtenstion + ".json");
            if (File.Exists(fullPath)) {
                fullPath = UniqueFileNameGenerator.NextAvailable(fullPath);
            }
            RecordingSaveData saveData = new RecordingSaveData(records, savedFrames);
            bool humanReadable = recordingMode == RecordingMode.HugeReadableJsonForPlayback;
            string json = JsonUtility.ToJson(saveData, humanReadable);
            File.WriteAllText(fullPath, json);
        }

        public enum RecordingMode
        {
            OptimizedJsonForPlayback,
            HugeReadableJsonForPlayback,
            CsvOnlyNoPlayback,
            CsvWithOptimizedJson
        }
    
    }
}