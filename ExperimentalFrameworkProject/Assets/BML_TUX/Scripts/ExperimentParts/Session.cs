using System;
using System.IO;
using System.Text.RegularExpressions;
using BML_ExperimentToolkit.Scripts.Managers;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    [Serializable]
    public class Session {
        const string DebugFolder = "/BML_TUX/Data/Debug";
        const string DebugFileName = "debugFile";

        string outputFullPath;
        
        /// <summary>
        /// stores the output path
        /// </summary>
        public string OutputFullPath {
            get {
                outputFullPath = Path.Combine(OutputFolder, OutputFileName).Replace(@"\", @"/");
                return outputFullPath;
            }
        }

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        string outputFileName = "";

        public string OutputFileName {
            get => DebugMode ? DebugFileName : outputFileName;
            set => outputFileName = value;
        }

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        string outputFolder = "";

        public string OutputFolder {
            get => DebugMode ? Path.Combine(Application.dataPath + DebugFolder) : outputFolder;
            set => outputFolder = value;
        }

        public string SelectedDesignFilePath = "";

        [SerializeField]
        public bool DebugMode;
        
        public int    BlockOrderChosenIndex = 0;

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
            string filePath = Path.Combine(Application.dataPath, SessionLocation, SessionDataFileName);
            Session session;
            if (File.Exists(filePath)) {
                string dataAsJason = File.ReadAllText(filePath);
                session = JsonUtility.FromJson<Session>(dataAsJason);
                //Debug.Log($"Session loaded: {filePath}");
            }
            else {
                Debug.Log("Previous Session file not found, creating new");
                session = new Session();
            }
            session.Enable();
            return session;
        }

        void SaveSessionData() {
            //Debug.Log("Saving Session data");
            string fileFolder = Path.Combine(Application.dataPath, SessionLocation);

            Directory.CreateDirectory(fileFolder);

            string filePath = Path.Combine(fileFolder, SessionDataFileName);
            string dataAsJson = JsonUtility.ToJson(this);
            File.WriteAllText(filePath, dataAsJson);
            
            Debug.Log(File.Exists(filePath) ? $"Saving Session data" : $"Session not saved properly: {filePath}");
        }

        /// <summary>
        /// Mark Session as completed
        /// </summary>
        void Completed() {

            SessionLogger.LogComplete(this);
            Disable();
        }


        public void ValidateFilePath(ref string errorLog, ref bool isValid) {
            ValidateFolder(ref errorLog, ref isValid);
            ValidateFileName (ref errorLog, ref isValid);
            ValidateFileDoesNotExist(ref errorLog, ref isValid);
        }
        
        void ValidateFolder(ref string errorLog, ref bool isValid ) {
            if (string.IsNullOrEmpty(OutputFolder)) {
                string errorString = $"Output Folder name not set or too short. {OutputFolder}";
                errorLog = LogErrorIntoString(errorLog, errorString);
                isValid = false;
            }
        }
        
        void ValidateFileName(ref string errorLog, ref bool isValid ) {
            if (string.IsNullOrEmpty(outputFileName)) {
                string errorString = $"Output File name not set or too short. {outputFileName}";
                errorLog = LogErrorIntoString(errorLog, errorString);
                isValid = false;
                return;
            }
            
            if (!IsAllNumbersAndLetters(outputFileName)) {
                string errorString = $"Output File name contains invalid characters. {outputFileName}";
                errorLog = LogErrorIntoString(errorLog, errorString);
                isValid = false;
            }
        }

        static bool IsAllNumbersAndLetters(string text) {
            return Regex.IsMatch(text, @"^[a-zA-Z0-9_]+$");
        }
        
        static string LogErrorIntoString(string errorLog, string errorString) {
            Debug.LogWarning(errorString);
            errorLog += errorString + "\n";
            return errorLog;
        }
        
        void ValidateFileDoesNotExist(ref string errorLog, ref bool isValid) {
            string outputFullPathWithExtension = Path.Combine(OutputFolder, outputFileName) + ".csv";
            Debug.Log(outputFullPathWithExtension);
            if (!File.Exists(outputFullPathWithExtension)) return;
            string errorString = $"Output File Already Exists @ {outputFullPath}";
            errorLog = LogErrorIntoString(errorLog, errorString);
            isValid = false;
        }
        

    }
}