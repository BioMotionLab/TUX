using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

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

        [SerializeField]
        public string logFilePath;

        public Session(FileLocationSettings fileLocationSettings){
            saveFilePath = fileLocationSettings.SessionSaveFilePath;
            sessionFolder = fileLocationSettings.SessionFolder;
            logFilePath = fileLocationSettings.SessionLogFilePath;
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


        public static Session LoadSessionData(FileLocationSettings fileLocationSettings) {
            Session session;
            string filePath = fileLocationSettings.SessionSaveFilePath;
            if (File.Exists(filePath)) {
                string dataAsJason = File.ReadAllText(filePath);
                try{
                    session = JsonUtility.FromJson<Session>(dataAsJason);
                    ValidateSessionJsonExport(session);
                }catch (Exception ex) when (ex is ArgumentException || 
                                            ex is NoNullAllowedException){
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
            Session session = new Session(fileLocationSettings);
            Debug.Log($"{TuxLog.Prefix} Previous Session file not found, creating new");
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

        static void ValidateSessionJsonExport(Session session){
            if (session.OutputFile == null)
                throw new NoNullAllowedException();
            if (session.SelectedDesignFilePath == null)
                throw new NoNullAllowedException();
            if (session.saveFilePath == null)
                throw new NoNullAllowedException();
            if (session.sessionFolder == null)
                throw new NoNullAllowedException();
            if (session.logFilePath == null)
                throw new NoNullAllowedException();
        }

    }

}
