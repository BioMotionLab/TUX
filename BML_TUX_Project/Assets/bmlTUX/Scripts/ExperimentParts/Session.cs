using System;
using System.IO;
using bmlTUX.Scripts.Managers;
using UnityEngine;

namespace bmlTUX.Scripts.ExperimentParts {
    
    [Serializable]
    public class Session {
        
        public FileLocationSettings FileLocations;

        [SerializeField]
        public OutputFile OutputFile;
        
        public string SelectedDesignFilePath = "";
        public int BlockOrderChosenIndex = 0;

        
        public Session(FileLocationSettings fileLocations) {
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
            Debug.Log(fileLocations.LastSessionSaveFilePath);
            Session session;
            if (File.Exists(filePath)) {
                string dataAsJason = File.ReadAllText(filePath);
                session = JsonUtility.FromJson<Session>(dataAsJason);
            }
            else {
                Debug.Log("Previous Session file not found, creating new");
                session = new Session(fileLocations);
            }
            session.Enable();
            return session;
        }

        void SaveSessionData() {
            
            Directory.CreateDirectory(FileLocations.SessionFolderWithDocuments);
            string filePath = FileLocations.LastSessionSaveFilePath;
            string dataAsJson = JsonUtility.ToJson(this);
            Debug.Log($"before save {FileLocations.LastSessionSaveFilePath}, {filePath}");
            File.WriteAllText(filePath, dataAsJson);
            
            Debug.Log(File.Exists(filePath) ? $"Session data Saved" : $"Session not saved properly: {filePath}");
        }
        
        void Completed() {
            SessionLogger.LogComplete(this);
            Disable();
        }



    }
}