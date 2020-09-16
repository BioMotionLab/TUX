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
        public VerticalLayoutGroup tableLayoutGroup;
        public ContentSizeFitter SizeFitter;
        
        int[]            ColumnLengths; 
    
        int              rowLength;
        bool ended = false;
        TextMeshProUGUI textMeshProUgui;
        TextMeshProUGUI currentTrialHeaderEntry;
        TextMeshProUGUI currentTrialRow;
        TextMeshProUGUI runnerHeader;
        TextMeshProUGUI[] rowEntries;
        GameObject[] rowObjects;
        Trial currentTrial;

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
            var rowObject = rowObjects[currentTrialIndex];
            SetColor(rowObject, Color.green);
            
        }

        public void ShowPanel() {
            
            table = GetExperimentTable();
            
            CalculateColumnWidths();
            DisplayHeader();
            CreateRows(ContentContainer.transform);

            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
                InitRow(rowIndex);
            }
            
            MainPanel.gameObject.SetActive(true);
            
        }

        void ExperimentStarted() {
            currentBlockIndex = 0;
            currentTrialIndex = 0;
            started = true;
        }
        
        
        void UpdatePanel() {
            
            tableLayoutGroup.enabled = false;
            SizeFitter.enabled = false;
          
            tableLayoutGroup.GetComponent<VerticalLayoutGroup>().enabled = false;

            if (!started) return;
            UpdateProgressPanel();
            
            UpdateDisplay();
        }
        
        
        void UpdateDisplay() {
            CalculateColumnWidths();

            DisplayCurrentTrialHeader();
            DisplayCurrentTrialRow();
            
            UpdateRowNow(currentTrialIndex);
            
        }

        void CalculateColumnWidths() {
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
        }

        void DisplayCurrentTrialHeader() {

            if (currentTrialHeaderEntry == null) {
                var currentTrialHeader = Instantiate(HeaderRowPrefab, CurrentTrialContainer.transform);
                LayoutElement entryLayout = currentTrialHeader.GetComponent<LayoutElement>();
                entryLayout.minWidth = rowLength * EntryPixelMultiplier;
                currentTrialHeader.name = "CurrentTrialHeader";
                currentTrialHeaderEntry = Instantiate(EntryPrefab, currentTrialHeader.transform);
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                string columnName = column.ColumnName;
                string paddedName = AddPadding(columnName, ColumnLengths[columnIndex]);
                stringBuilder.Append(paddedName);
            }
            
            TextMeshProUGUI entryTextObject = currentTrialHeaderEntry.GetComponent<TextMeshProUGUI>();
            entryTextObject.text = stringBuilder.ToString();
        }
        
        void DisplayCurrentTrialRow() {

            if (currentTrialRow == null) {
                GameObject newRow = Instantiate(RowPrefab, CurrentTrialContainer.transform);
                LayoutElement entryLayout = newRow.GetComponent<LayoutElement>();
                entryLayout.minWidth = rowLength * EntryPixelMultiplier;
                newRow.name = "Row {rowIndex}";
        
                currentTrialRow = Instantiate(EntryPrefab, newRow.transform);
            }
            
            DataRow row = table.Rows[currentTrialIndex];
            
            StringBuilder stringBuilder = new StringBuilder();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                string rowValue = row[column.ColumnName].ToString();
                string paddedValue = AddPadding(rowValue, ColumnLengths[columnIndex]);
                stringBuilder.Append(paddedValue);
            }
    
            TextMeshProUGUI entryTextObject = currentTrialRow.GetComponent<TextMeshProUGUI>();
            entryTextObject.text = stringBuilder.ToString();
            
        }

        void DisplayHeader() {

            if (runnerHeader == null) {
                GameObject header = Instantiate(HeaderRowPrefab, ContentContainer.transform);
                LayoutElement entryLayout = header.GetComponent<LayoutElement>();
                entryLayout.minWidth = rowLength * EntryPixelMultiplier;
                header.name = "Header";
                runnerHeader = Instantiate(EntryPrefab, header.transform);
            }
            
            StringBuilder stringBuilder = new StringBuilder();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                string columnName = column.ColumnName;
                string paddedName = AddPadding(columnName, ColumnLengths[columnIndex]);
                stringBuilder.Append(paddedName);
            }
            
            TextMeshProUGUI entryTextObject = runnerHeader.GetComponent<TextMeshProUGUI>();
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

        
        void InitRow(int rowIndex) {
            DataRow rowData = table.Rows[rowIndex];
            
            var rowEntry = rowEntries[rowIndex];
            var rowObject = rowObjects[rowIndex];
            
            bool isCurrentlyRunningTrial = currentTrialIndex == rowIndex;
            bool trialIsComplete = (bool)rowData[runner.DesignFile.GetColumnNamesSettings.Completed];
            
            if (isCurrentlyRunningTrial && !trialIsComplete) SetColor(rowObject, Color.yellow);
            else SetColor(rowObject, Color.red);
                
            StringBuilder stringBuilder = new StringBuilder();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                string rowValue = rowData[column.ColumnName].ToString();
                string paddedValue = AddPadding(rowValue, ColumnLengths[columnIndex]);
                stringBuilder.Append(paddedValue);
            }
        
            TextMeshProUGUI entryTextObject = rowEntry.GetComponent<TextMeshProUGUI>();
            entryTextObject.text = stringBuilder.ToString();
        }
        
        void UpdateRowNow(int rowIndex) {
            if (currentTrial == null) return;
            DataRow rowData = table.Rows[rowIndex];
            
            var rowEntry = rowEntries[rowIndex];
            var rowObject = rowObjects[rowIndex];
            
            bool isCurrentlyRunningTrial = currentTrialIndex == rowIndex;
            bool trialIsComplete = (bool)rowData[runner.DesignFile.GetColumnNamesSettings.Completed];
            
            if (isCurrentlyRunningTrial && !trialIsComplete) SetColor(rowObject, Color.yellow);
            else SetColor(rowObject, Color.red);
                
            StringBuilder stringBuilder = new StringBuilder();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                string rowValue = currentTrial.Data[column.ColumnName].ToString();
                string paddedValue = AddPadding(rowValue, ColumnLengths[columnIndex]);
                stringBuilder.Append(paddedValue);
            }
        
            TextMeshProUGUI entryTextObject = rowEntry.GetComponent<TextMeshProUGUI>();
            entryTextObject.text = stringBuilder.ToString();
        }

        void CreateRows(Transform contentContainerTransform) {
            
            if (rowEntries == null) rowEntries = new TextMeshProUGUI[table.Rows.Count];
            if (rowObjects == null) rowObjects = new GameObject[table.Rows.Count];
            
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
                
                if (rowEntries[rowIndex] == null) {
                    GameObject newestRow = Instantiate(RowPrefab, contentContainerTransform);
                    rowObjects[rowIndex] = newestRow;
                    newestRow.name = "Row {rowIndex}";
                    TextMeshProUGUI newestRowEntry = Instantiate(EntryPrefab, newestRow.transform);
                    rowEntries[rowIndex] = newestRowEntry;
                }
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

        void TrialStarted(Trial trial) {
            currentTrialIndex = trial.Index;
            currentTrial = trial;
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


