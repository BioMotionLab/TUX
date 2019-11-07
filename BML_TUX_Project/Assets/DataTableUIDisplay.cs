using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DataTableUIDisplay : MonoBehaviour {
    public BasicDataTable basicTable;
    public GameObject content;
    DataTable table;
    
    public TextMeshProUGUI entryPrefab;
    public TextMeshProUGUI headerEntryPrefab;
    public GameObject rowPrefab;

    List<GameObject> entries;
    Dictionary<int, int> columnIndexToMaxLength;
    int headerPixelMultiplier = 14;
    int entryPixelMultiplier = 8;

    void Start() {
        DoUpdate();
    }

    [ContextMenu("Do Update")]
    void DoUpdate() {
        basicTable.Generate();
        table = basicTable.table;

        columnIndexToMaxLength = new Dictionary<int, int>();
        for (int index = 0; index < table.Columns.Count; index++) {
            columnIndexToMaxLength.Add(index,0);
            DataColumn column = table.Columns[index];
            foreach (DataRow row in table.Rows) {
                columnIndexToMaxLength[index] =
                    Math.Max(columnIndexToMaxLength[index], row[column.ColumnName].ToString().Length * entryPixelMultiplier);
                columnIndexToMaxLength[index] =
                    Math.Max(columnIndexToMaxLength[index], column.ColumnName.Length * headerPixelMultiplier);
            }
        }

        DestroyAllChildren(content.transform);
        
        
        DisplayHeader();
        DisplayRows();
    }

    void DisplayHeader() {
        GameObject header = Instantiate(rowPrefab, content.transform);
        header.name = "Header";
        for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
            DataColumn column = table.Columns[columnIndex];
            var newEntry = Instantiate(headerEntryPrefab, header.transform);

            LayoutElement entryLayout = newEntry.GetComponent<LayoutElement>();
            entryLayout.minWidth = columnIndexToMaxLength[columnIndex];

            TextMeshProUGUI entryTextObject = newEntry.GetComponent<TextMeshProUGUI>();
            entryTextObject.text = column.ColumnName;
        }
    }

    void DisplayRows() {
        for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
            DataRow row = table.Rows[rowIndex];
            var newRowObject = Instantiate(rowPrefab, content.transform);
            newRowObject.name = $"Row {rowIndex}";
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                TextMeshProUGUI newEntry = Instantiate(entryPrefab, newRowObject.transform);
                newEntry.gameObject.name = $"Column {columnIndex}";
                LayoutElement entryLayout = newEntry.GetComponent<LayoutElement>();
                entryLayout.minWidth = columnIndexToMaxLength[columnIndex];
                TextMeshProUGUI entryTextObject = newEntry.GetComponent<TextMeshProUGUI>();
                entryTextObject.text = row[column.ColumnName].ToString();
            }
        }
    }

    void DestroyAllChildren(Transform parent) {
        foreach (Transform child in parent.transform) {
            DestroyImmediate(child.gameObject);
        }
    }
}
