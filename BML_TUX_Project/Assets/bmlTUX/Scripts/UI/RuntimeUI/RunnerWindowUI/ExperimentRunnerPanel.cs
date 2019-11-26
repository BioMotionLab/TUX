using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace bmlTUX.Scripts.UI.RuntimeUI.RunnerWindowUI {
    public class ExperimentRunnerPanel : MonoBehaviour {
        [SerializeField]
        TextMeshProUGUI RunningStatusText = default;

        [SerializeField]
        TextMeshProUGUI CurrentTrialText = default;

        [SerializeField]
        RectTransform MainPanel = default;

        [SerializeField]
        RectTransform CurrentTrialContainer = default;

        [SerializeField]
        RectTransform ProgressPanel = default;
        
        bool             started           = false;
        int              currentBlockIndex = -1;
        int              currentTrialIndex = -1;
        ExperimentRunner runner;
        
        
        public GameObject      ContentContainer;
        public TextMeshProUGUI EntryPrefab;
        public TextMeshProUGUI HeaderEntryPrefab;
        public GameObject      RowPrefab;
        public GameObject      HeaderRowPrefab;

        List<GameObject>     entries;
        Dictionary<int, int> columnIndexToMaxLength;
        DataTable            table;
        GameObject currentTrialRowObject;
        const int            EntryPixelMultiplier  = 12;
        
        
        int[]            ColumnLengths; 
    
        int              rowLength;
        bool ended = false;

        const int        paddingChars          = 4;
        const string     ASpace                = " ";
        
        void OnEnable() {
            MainPanel.gameObject.SetActive(false);
            ExperimentEvents.OnInitExperiment += Init;
            ExperimentEvents.OnBlockUpdated += BlockCompleted;
            ExperimentEvents.OnExperimentStarted += ExperimentStarted;
            ExperimentEvents.OnTrialHasStarted += TrialStarted;
            ExperimentEvents.OnTrialHasCompleted += TrialCompleted;
            ExperimentEvents.OnEndExperiment += ExperimentCompleted;
        }

        void ExperimentCompleted() {
            ended = true;
            UpdatePanel();
        }

        void OnDisable() {
            ExperimentEvents.OnInitExperiment -= Init;
            ExperimentEvents.OnBlockUpdated -= BlockCompleted;
            ExperimentEvents.OnExperimentStarted -= ExperimentStarted;
            ExperimentEvents.OnTrialHasStarted -= TrialStarted;
            ExperimentEvents.OnTrialHasCompleted -= TrialCompleted;
            ExperimentEvents.OnEndExperiment -= ExperimentCompleted;
        }

        void TrialCompleted() {
            UpdatePanel();
        }

        public void ShowPanel() {
            MainPanel.gameObject.SetActive(true);
        }

        void ExperimentStarted() {
            currentBlockIndex = 0;
            currentTrialIndex = 0;
            started = true;

            
        }
        
        
        void UpdatePanel() {
            //TODO this can probably be optimized so that instantiation happens at start of experiment once only. Since components don't change, just text content and width.
            
            if (!started) return;
            UpdateProgressPanel();
            
            Clear();
            table = GetExperimentTable();
            
            Display();
        }

        
        
        void Display() {
            Clear();
            ColumnLengths = new int[table.Columns.Count];
            for (int index = 0; index < table.Columns.Count; index++) {
                DataColumn column = table.Columns[index];
                ColumnLengths[index] = Math.Max(ColumnLengths[index], column.ColumnName.Length);
                foreach (DataRow row in table.Rows) {
                    ColumnLengths[index] = Math.Max(ColumnLengths[index], row[column.ColumnName].ToString().Length);
                }

                ColumnLengths[index] += paddingChars;
            }

            rowLength = ColumnLengths.Sum();

            DisplayHeader(CurrentTrialContainer.transform);
            DisplayCurrentRow(CurrentTrialContainer.transform);
            
            DisplayHeader(ContentContainer.transform);
            DisplayRows(ContentContainer.transform);
        }
        

        void DisplayHeader(Transform contentContainer) {
            GameObject header = Instantiate(HeaderRowPrefab, contentContainer);
            header.name = "Header";
            
            var newEntry = Instantiate(EntryPrefab, header.transform);
            LayoutElement entryLayout = newEntry.GetComponent<LayoutElement>();
            entryLayout.minWidth = rowLength * EntryPixelMultiplier;
            
            StringBuilder stringBuilder = new StringBuilder();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                string columnName = column.ColumnName;
                string paddedName = AddPadding(columnName, ColumnLengths[columnIndex]);
                stringBuilder.Append(paddedName);
            }
            
            TextMeshProUGUI entryTextObject = newEntry.GetComponent<TextMeshProUGUI>();
            entryTextObject.text = stringBuilder.ToString();
        }

        string AddPadding(string s, int columnLength) {
            StringBuilder paddedString = new StringBuilder(s);
            int diff = columnLength - s.Length;
            for (int i = 0; i < diff; i++) {
                paddedString.Append(ASpace);
            }
            return paddedString.ToString();
        }

        void DisplayRows(Transform contentContainerTransform) {
            
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
                DataRow row = table.Rows[rowIndex];
                
                GameObject newRow = Instantiate(RowPrefab, contentContainerTransform);
                newRow.name = "Row {rowIndex}";
        
                var newRowEntry = Instantiate(EntryPrefab, newRow.transform);
                LayoutElement entryLayout = newRowEntry.GetComponent<LayoutElement>();
                entryLayout.minWidth = rowLength * EntryPixelMultiplier;
        
                
                bool isCurrentlyRunningTrial = currentTrialIndex == rowIndex;
                bool trialIsComplete = (bool)row[runner.DesignFile.ColumnNamesSettings.Completed];
                
                if (isCurrentlyRunningTrial && !trialIsComplete) SetColor(newRow, Color.yellow);
                else if (trialIsComplete) SetColor(newRow, Color.green);
                else SetColor(newRow, Color.red);
                
                StringBuilder stringBuilder = new StringBuilder();
                for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                    DataColumn column = table.Columns[columnIndex];
                    string rowValue = row[column.ColumnName].ToString();
                    string paddedValue = AddPadding(rowValue, ColumnLengths[columnIndex]);
                    stringBuilder.Append(paddedValue);
                }
        
                TextMeshProUGUI entryTextObject = newRowEntry.GetComponent<TextMeshProUGUI>();
                entryTextObject.text = stringBuilder.ToString();
                
            }
        }

        void DisplayCurrentRow(Transform currentTrialContainer) {
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
                
                bool isCurrentlyRunningTrial = currentTrialIndex == rowIndex;

                if (!isCurrentlyRunningTrial) continue;
                
                DataRow row = table.Rows[rowIndex];
                
                GameObject newRow = Instantiate(RowPrefab, currentTrialContainer);
                newRow.name = "Row {rowIndex}";
        
                var newRowEntry = Instantiate(EntryPrefab, newRow.transform);
                LayoutElement entryLayout = newRowEntry.GetComponent<LayoutElement>();
                entryLayout.minWidth = rowLength * EntryPixelMultiplier;
                
                StringBuilder stringBuilder = new StringBuilder();
                for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                    DataColumn column = table.Columns[columnIndex];
                    string rowValue = row[column.ColumnName].ToString();
                    string paddedValue = AddPadding(rowValue, ColumnLengths[columnIndex]);
                    stringBuilder.Append(paddedValue);
                }
        
                TextMeshProUGUI entryTextObject = newRowEntry.GetComponent<TextMeshProUGUI>();
                entryTextObject.text = stringBuilder.ToString();
                
            }
        }

        
        
        DataTable GetExperimentTable() {
            DataTable newTable = runner.RunnableDesign.Blocks[0].TrialTable.Clone();
            foreach (var block in runner.RunnableDesign.Blocks) {
                DataTable trialTable = block.TrialTable;
                foreach (DataRow row in trialTable.Rows) {
                    newTable.ImportRow(row);
                }
            }

            return newTable;
        }

        void TrialStarted(Trial trial, int indexInBlock) {
            currentTrialIndex = trial.Index;
            UpdatePanel();
        }

        void BlockCompleted(List<Block> blocks, int index) {
            currentBlockIndex = index + 1;
        }

        void Init(ExperimentRunner runnerToInit) {
            runner = runnerToInit;
            currentBlockIndex = -1;
            currentTrialIndex = -1;
            started = false;

        }

        void UpdateProgressPanel() {

            string runningText = runner.Running ? "Running" : "Not Running";
            runningText = !runner.Ended ? runningText : "Ended";
            RunningStatusText.text = $"Runner is {runningText}.";

            var design = runner.RunnableDesign;
            int currentTrial = (design.TotalTrials / design.BlockCount) * (currentBlockIndex) + currentTrialIndex + 1;
            if (currentTrial > design.TotalTrials) currentTrial = design.TotalTrials;

            string blockText = "";
            if (design.HasBlocks) blockText = $" (Block {currentBlockIndex} / {design.BlockCount})";
            CurrentTrialText.text = ($"Running Trial: {currentTrial} of {design.TotalTrials} total{blockText}.");

            Image progressPanelImage = ProgressPanel.GetComponent<Image>();
            if (ended) {
                progressPanelImage.color = Color.red;
            }
            else {
                progressPanelImage.color = Color.green;
            }
        }
        

        void Clear() {
            DestroyAllContent();
        }
        

        void SetColor(GameObject newRowObject, Color rowColor) {
            Image rowImage = newRowObject.GetComponent<Image>();
            Color mutedRowColor = rowColor;
            mutedRowColor.a = .1f;
            rowImage.color = mutedRowColor;
        }

        TextMeshProUGUI NewEntryInRow(GameObject newRowObject, int width, string entryName, bool header=false) {
            TextMeshProUGUI entryToInstantiate = header ? HeaderEntryPrefab : EntryPrefab;
            TextMeshProUGUI newEntry = Instantiate(entryToInstantiate, newRowObject.transform);
            newEntry.gameObject.name = entryName;
            LayoutElement entryLayout = newEntry.GetComponent<LayoutElement>();
            entryLayout.minWidth = width;
            TextMeshProUGUI entryTextObject = newEntry.GetComponent<TextMeshProUGUI>();
            return entryTextObject;
        }

        void DestroyAllContent() {
            foreach (Transform child in ContentContainer.transform) {
                Destroy(child.gameObject);
            }

            foreach (Transform child in CurrentTrialContainer.transform) {
                Destroy(child.gameObject);
            }
        }
    }

}


