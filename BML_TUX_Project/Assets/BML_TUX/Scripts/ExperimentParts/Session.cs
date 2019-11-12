using System;
using System.IO;
using System.Text.RegularExpressions;
using BML_TUX.Scripts.Managers;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace BML_TUX.Scripts.ExperimentParts {
    
    [Serializable]
    public class Session {
        
        [SerializeField]
        public FileLocationSettings FileLocations;

        [SerializeField]
        public OutputFile OutputFile;
        
        public string SelectedDesignFilePath = "";

        public bool DebugMode;
        
        public int BlockOrderChosenIndex = 0;

        
        public Session(FileLocationSettings fileLocations) {
            this.FileLocations = fileLocations;
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
            string filePath = Path.Combine(Application.dataPath, fileLocations.SessionFolder, fileLocations.SessionDataFileName);
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
            string fileFolder = Path.Combine(Application.dataPath, FileLocations.SessionFolder);
            Directory.CreateDirectory(fileFolder);
            string filePath = Path.Combine(fileFolder, FileLocations.SessionDataFileName);
            string dataAsJson = JsonUtility.ToJson(this);
            File.WriteAllText(filePath, dataAsJson);
            
            Debug.Log(File.Exists(filePath) ? $"Saving Session data" : $"Session not saved properly: {filePath}");
        }
        
        void Completed() {
            SessionLogger.LogComplete(this);
            Disable();
        }



    }
}