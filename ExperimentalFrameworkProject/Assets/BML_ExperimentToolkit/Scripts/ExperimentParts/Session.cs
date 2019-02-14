using System.IO;
using MyNamespace;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class Session {

        readonly string debugFolder   = Application.dataPath + "/BML_Debug";
        const    string debugFileName = "debugFile";

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

        string outputFileName = "";

        public string OutputFileName {
            get {
                if (DebugMode) {
                    return debugFileName;
                }
                else {
                    return outputFileName;
                }
            }
            set { outputFileName = value; }
        }

        string outputFolder = "";

        public string OutputFolder {
            get {
                if (DebugMode) {
                    return debugFolder;
                }
                else {
                    return outputFolder;
                }
            }
            set { outputFolder = value; }
        }

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

        bool blockChosen = false;

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

        public int    OrderChosenIndex = 0;

        string participantID;
        public string ParticipantId {
            get {
                if (debugMode) {
                    return "debug";
                }
                else {
                    if (participantID == null || participantID.Length == 0)
                    {
                        participantID = "Unnamed";
                    }
                    return participantID;
                }
            }
            set {
                if (debugMode) {
                    participantID = "debug";
                }
                else {
                    participantID = value;
                }
            }
        }
    }
}