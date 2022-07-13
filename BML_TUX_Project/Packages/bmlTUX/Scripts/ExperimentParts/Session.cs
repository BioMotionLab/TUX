using System;
using System.IO;
using UnityEngine;

namespace bmlTUX {
    
    [Serializable]
    public class Session {
        

        [SerializeField]
        public OutputFile OutputFile;
        
        [SerializeField]
        public string SelectedDesignFilePath = "";
        
        [SerializeField]
        public int BlockOrderChosenIndex = 0;

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
            string filePath = FileLocationSettings.LastSessionSaveFilePath;
            Session session;
            if (File.Exists(filePath)) {
                string dataAsJason = File.ReadAllText(filePath);
                session = JsonUtility.FromJson<Session>(dataAsJason);
            }
            else {        
                session = CreateNewSession();
            }
            session.Enable();
            return session;
        }

        static Session CreateNewSession() {
            Session session;
            Debug.Log($"{TuxLog.Prefix} Previous Session file not found, creating new");
            session = new Session();
            return session;
        }

        void SaveSessionData() {
            
            Directory.CreateDirectory(FileLocationSettings.SessionFolder);
            string filePath = FileLocationSettings.LastSessionSaveFilePath;
            string dataAsJson = JsonUtility.ToJson(this);
            File.WriteAllText(filePath, dataAsJson);

            string message = File.Exists(filePath) ? $"Session logged to {filePath}" : $"Error logging session to: {filePath}";
            Debug.Log($"{TuxLog.Prefix} {message}");
        }
        
        void Completed() {
            SessionLogger.LogComplete(this);
            Disable();
        }



    }

}
