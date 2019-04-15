using System;
using System.IO;
using System.Text;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.Managers {
    public static class SessionLogger {

        const string OutputLocation = "BML_ExperimentToolkit/Data";
        const string OutputFileName = "BML_session_log.txt";
        
        /// <summary>
        /// Logs a session to file
        /// </summary>
        /// <param name="session"></param>
        public static void Log(Session session) {
            //Debug.Log("Logging session");

            //Set up file location/reference
            string fileFolder = Path.Combine(Application.dataPath, OutputLocation);
            Directory.CreateDirectory(fileFolder);
            string outputPath = Path.Combine(fileFolder, OutputFileName);

            //create log string
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("---------------------");
            sb.AppendLine($"Session Logged: {DateTime.Now:yyyy/MM/dd hh:mm}");
            sb.AppendLine(JsonUtility.ToJson(session));

            //output log string
            using (StreamWriter streamWriter = new StreamWriter(outputPath, true)) {
                streamWriter.Write(sb.ToString());
            }
        }

        /// <summary>
        /// Adds a line to log to mark session as complete
        /// </summary>
        /// <param name="session"></param>
        public static void LogComplete(Session session) {

            string outputPath = Path.Combine(Application.dataPath, OutputLocation, OutputFileName);

            //ensure log exists
            if (File.Exists(outputPath)) {
                using (StreamWriter streamWriter = new StreamWriter(outputPath, true)) {
                    streamWriter.Write($"Confirmed Complete at: {DateTime.Now:hh:mm}");
                }
            }
            else {
                throw new FileNotFoundException("Session log file not found");
            }
        }
    }
}