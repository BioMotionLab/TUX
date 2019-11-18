using System;
using System.IO;
using bmlTUX.Scripts.Managers;
using UnityEngine;

namespace bmlTUX.Scripts.ExperimentParts {
    
    [Serializable]
    public class Session {
        
        [SerializeField]
        public FileLocationSettings FileLocations;

        [SerializeField]
        public OutputFile OutputFile;
        
        [SerializeField]
        public string SelectedDesignFilePath = "";
        
        [SerializeField]
        public int BlockOrderChosenIndex = 0;

        
        public Session(FileLocationSettings fileLocations) {
            if (fileLocations == null) throw new NullReferenceException("File Locations Null when creating session");
            FileLocations = fileLocations;
        }
        
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


        public static Session LoadSessionData(FileLocationSettings fileLocations) {
            string filePath = fileLocations.LastSessionSaveFilePath;
            Session session;
            if (File.Exists(filePath)) {
                string dataAsJason = File.ReadAllText(filePath);
                session = JsonUtility.FromJson<Session>(dataAsJason);
                session.FileLocations = fileLocations;
                if (!session.ValidSelf()) {
                    Debug.LogWarning("Loaded Session Not valid, creating new");
                    CreateNewSession(fileLocations);
                }
            }
            else {        
                session = CreateNewSession(fileLocations);
            }
            session.Enable();
            return session;
        }

        static Session CreateNewSession(FileLocationSettings fileLocations) {
            Session session;
            Debug.Log("Previous Session file not found, creating new");
            session = new Session(fileLocations);
            return session;
        }

        bool ValidSelf() {
            bool valid = FileLocations != null;
            return valid;
        }

        void SaveSessionData() {
            
            if (FileLocations == null) throw new NullReferenceException("FileLocations null in saveSessionData");
            Directory.CreateDirectory(FileLocations.SessionFolder);
            string filePath = FileLocations.LastSessionSaveFilePath;
            string dataAsJson = JsonUtility.ToJson(this);
            File.WriteAllText(filePath, dataAsJson);
            
            Debug.Log(File.Exists(filePath) ? $"Session data Saved to {filePath}" : $"Session not saved properly: {filePath}");
        }
        
        void Completed() {
            SessionLogger.LogComplete(this);
            Disable();
        }



    }
}