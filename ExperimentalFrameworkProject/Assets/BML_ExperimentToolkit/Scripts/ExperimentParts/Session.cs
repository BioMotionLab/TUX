using System;
using System.IO;
using BML_ExperimentToolkit.Scripts.Managers;
using MyNamespace;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    [Serializable]
    public class Session {
        const string DebugFolder = "/BML_Debug/";
        const string DebugFileName = "debugFile";

        string outputFullPath;

        /// <summary>
        /// stores the output path
        /// </summary>
        public string OutputFullPath {
            get {
                outputFullPath = Path.Combine(OutputFolder, OutputFileName);
                return outputFullPath;
            }
        }

        [SerializeField]
        string outputFileName = "";

        public string OutputFileName {
            get {
                if (DebugMode) {
                    return DebugFileName;
                }
                else {
                    return outputFileName;
                }
            }
            set { outputFileName = value; }
        }

        [SerializeField]
        string outputFolder = "";

        public string OutputFolder {
            get {
                if (DebugMode) {
                    return Path.Combine(Application.dataPath + DebugFolder);
                }
                else {
                    return outputFolder;
                }
            }
            set { outputFolder = value; }
        }

        [SerializeField]
        bool debugMode;

        public bool DebugMode {
            get { return debugMode; }
            set {
                if (value == debugMode) return;
                debugMode = value;

                
                if (!debugMode) {
                    ParticipantId = "Unnamed";
                }
            }
        }


        bool blockChosen;

        public bool BlockChosen {
            get { return blockChosen; }
            set {

                if (value == blockChosen) return;

                blockChosen = value;
                if (blockChosen) {
                    Debug.Log($"Block order chosen: {OrderChosenIndex}");
                    ExperimentEvents.BlockOrderSelected(OrderChosenIndex);
                }
            }
        }


        public int    OrderChosenIndex;

        [SerializeField]
        string participantId;


        public string ParticipantId {
            get {
                if (debugMode) {
                    return "debug";
                }
                else {
                    if (string.IsNullOrEmpty(participantId))
                    {
                        participantId = "Unnamed";
                    }
                    return participantId;
                }
            }
            set {
                participantId = debugMode ? "debug" : value;
            }
        }


        const string SessionDataFileName = "BML_last_experiment_session.json";
        const string SessionLocation = "BML_ExperimentToolkit/Data";

       void Enable() {
            ExperimentEvents.OnEndExperiment += Completed;
            ExperimentEvents.OnExperimentStarted += Started;
        }

        void Disable() {
            ExperimentEvents.OnEndExperiment -= Completed;
            ExperimentEvents.OnExperimentStarted -= Started;
        }

        void Started() {
            SaveSessionData();
            SessionLogger.Log(this);
        }


        public static Session LoadSessionData() {
            Debug.Log("Loading session data");
            string filePath = Path.Combine(Application.dataPath, SessionLocation, SessionDataFileName);
            Session session;
            if (File.Exists(filePath)) {
                string dataAsJason = File.ReadAllText(filePath);
                session = JsonUtility.FromJson<Session>(dataAsJason);
                Debug.Log($"Session loaded: {filePath}");
                }
            else {
                Debug.Log("Session file not found, not loaded, creating new");
                session = new Session();
            }
            session.Enable();
            return session;
        }

        public void SaveSessionData() {
            Debug.Log("Saving session data");
            string fileFolder = Path.Combine(Application.dataPath, SessionLocation);

            Directory.CreateDirectory(fileFolder);

            string filePath = Path.Combine(fileFolder, SessionDataFileName);
            string dataAsJson = JsonUtility.ToJson(this);
            File.WriteAllText(filePath, dataAsJson);
            
            Debug.Log(File.Exists(filePath) ? $"session saved: {filePath}" : $"session not saved: {filePath}");
        }

        /// <summary>
        /// Mark Session as completed
        /// </summary>
        public void Completed() {

            SessionLogger.LogComplete(this);
            Disable();
        }


        



    }
}