using System;
using System.IO;
using UnityEditor;
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

        [SerializeField]
        public string saveFilePath;

        [SerializeField]
        public string sessionFolder;

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


        public static Session LoadSessionData(FileLocationSettings fileLocationSettings) {
            Session session;
            string filePath = fileLocationSettings.SessionSaveFilePath;
            if (File.Exists(filePath)) {
                string dataAsJason = File.ReadAllText(filePath);
                try{
                    session = JsonUtility.FromJson<Session>(dataAsJason);
                }catch (ArgumentException){
                    File.Delete(filePath);
                    Debug.Log($"{TuxLog.Prefix} Previous Session file corrupt, deleting");
                    session = CreateNewSession(fileLocationSettings);
                }
                
            }
            else {        
                session = CreateNewSession(fileLocationSettings);
            }

            session.Enable();
            return session;
        }

        static Session CreateNewSession(FileLocationSettings fileLocationSettings) {
            Session session = new Session();
            Debug.Log($"{TuxLog.Prefix} Previous Session file not found, creating new");
            session.saveFilePath = fileLocationSettings.SessionSaveFilePath;
            session.sessionFolder = fileLocationSettings.SessionFolder;
            return session;
        }

        void SaveSessionData() {
            Directory.CreateDirectory(sessionFolder);
            string filePath = saveFilePath;
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
