using System;
using System.Collections.Generic;
using System.Data;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace bmlTUX.Scripts.UI.Runtime {
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
        Trial currentTrial;
        GameObject currentTrialRowObject;
        const int            RunningIndicatorWidth = 60;
        const int            HeaderPixelMultiplier = 10;
        const int            EntryPixelMultiplier  = 8;
        
        
        void OnEnable() {
            MainPanel.gameObject.SetActive(false);
            ExperimentEvents.OnInitExperiment += Init;
            ExperimentEvents.OnBlockUpdated += BlockCompleted;
            ExperimentEvents.OnExperimentStarted += ExperimentStarted;
            ExperimentEvents.OnTrialHasStarted += TrialStarted;
        }



        void OnDisable() {
            ExperimentEvents.OnInitExperiment -= Init;
            ExperimentEvents.OnBlockUpdated -= BlockCompleted;
            ExperimentEvents.OnExperimentStarted -= ExperimentStarted;
            ExperimentEvents.OnTrialHasStarted -= TrialStarted;
        }

        public void ShowPanel() {
            MainPanel.gameObject.SetActive(true);
        }

        void ExperimentStarted() {
            currentBlockIndex = 0;
            currentTrialIndex = 0;
            started = true;
        }

        void Update() {
            if (!started) return;
            UpdateProgressPanel();

            Clear();
            table = GetExperimentTable();
            columnIndexToMaxLength = new Dictionary<int, int>();



            for (int index = 0; index < table.Columns.Count; index++) {
                columnIndexToMaxLength.Add(index, 0);
                DataColumn column = table.Columns[index];
                foreach (DataRow row in table.Rows) {
                    columnIndexToMaxLength[index] =
                        Math.Max(columnIndexToMaxLength[index],
                                 row[column.ColumnName].ToString().Length * EntryPixelMultiplier);
                    columnIndexToMaxLength[index] =
                        Math.Max(columnIndexToMaxLength[index], column.ColumnName.Length * HeaderPixelMultiplier);
                }
            }

            DisplayHeader(CurrentTrialContainer.transform, false);
            currentTrialRowObject = Instantiate(RowPrefab, CurrentTrialContainer.transform);
            
            
            DisplayHeader(ContentContainer.transform);
            DisplayRows();
            
            
        }

        DataTable GetExperimentTable() {
            DataTable table = runner.RunnableDesign.Blocks[0].TrialTable.Clone();
            foreach (var block in runner.RunnableDesign.Blocks) {
                DataTable trialTable = block.TrialTable;
                foreach (DataRow row in trialTable.Rows) {
                    table.ImportRow(row);
                }
            }

            return table;
        }

        void TrialStarted(Trial trial, int indexInBlock) {
            currentTrialIndex = trial.Index;
            currentTrial = trial;
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

            if (runner.Ended) {
                Image progressPanelImage = ProgressPanel.GetComponent<Image>();
                progressPanelImage.color = Color.red;
            }
        }
        

        void Clear() {
            DestroyAllContent();
        }

        void DisplayHeader(Transform container, bool displayRunningIndicator = true) {
            GameObject headerRow = Instantiate(HeaderRowPrefab, container);
            headerRow.name = "Header";

            if (displayRunningIndicator) {
                TextMeshProUGUI runningIndicatorHeader =
                    NewEntryInRow(headerRow, RunningIndicatorWidth, "RunningIndicator", true);
                runningIndicatorHeader.text = "";
            }

            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                TextMeshProUGUI entryTextObject = NewEntryInRow(headerRow, columnIndexToMaxLength[columnIndex],
                                                                   $"HeaderColumn {columnIndex}", true);
                entryTextObject.text = column.ColumnName;
            }
        }

        void DisplayRows() {
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
                DataRow row = table.Rows[rowIndex];
                
                bool isCurrentlyRunningTrial = currentTrialIndex == rowIndex;

                GameObject newRowObject = Instantiate(RowPrefab, ContentContainer.transform);
                newRowObject.name = $"Row {rowIndex}";

                TextMeshProUGUI runningIndicator =
                    NewEntryInRow(newRowObject, RunningIndicatorWidth, "RunningIndicator");
                
                
                runningIndicator.text = isCurrentlyRunningTrial ? "Running" : "";
                if (isCurrentlyRunningTrial) SetColor(newRowObject, Color.yellow);
                else if ((bool)row[runner.DesignFile.ColumnNamesSettings.Completed]) SetColor(newRowObject, Color.green);
                else SetColor(newRowObject, Color.red);

                for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                    DataColumn column = table.Columns[columnIndex];

                    if (isCurrentlyRunningTrial) {
                        TextMeshProUGUI currentTrialEntryTextObject =
                                NewEntryInRow(currentTrialRowObject, columnIndexToMaxLength[columnIndex], $"Column{columnIndex}");
                            currentTrialEntryTextObject.text = row[column.ColumnName].ToString();
                        
                    }
                    
                    TextMeshProUGUI entryTextObject =
                        NewEntryInRow(newRowObject, columnIndexToMaxLength[columnIndex], $"Column{columnIndex}");
                    entryTextObject.text = row[column.ColumnName].ToString();
                }
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


