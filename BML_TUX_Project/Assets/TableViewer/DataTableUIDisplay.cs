using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class DataTableUIDisplay : MonoBehaviour {
    
    public GameObject ContentContainer;
    public TextMeshProUGUI EntryPrefab;
    public TextMeshProUGUI HeaderEntryPrefab;
    public GameObject RowPrefab;
    public GameObject HeaderRowPrefab;
    
    List<GameObject> entries;
    Dictionary<int, int> columnIndexToMaxLength;
    DataTable table;
    const int HeaderPixelMultiplier = 10;
    const int EntryPixelMultiplier = 8;
    
    
    public void Display(DataTable tableToDisplay) {
        table = tableToDisplay;
        columnIndexToMaxLength = new Dictionary<int, int>();
        for (int index = 0; index < table.Columns.Count; index++) {
            columnIndexToMaxLength.Add(index,0);
            DataColumn column = table.Columns[index];
            foreach (DataRow row in table.Rows) {
                columnIndexToMaxLength[index] =
                    Math.Max(columnIndexToMaxLength[index], row[column.ColumnName].ToString().Length * EntryPixelMultiplier);
                columnIndexToMaxLength[index] =
                    Math.Max(columnIndexToMaxLength[index], column.ColumnName.Length * HeaderPixelMultiplier);
            }
        }

        DestroyAllContent();
        
        DisplayHeader();
        DisplayRows();
    }

    public void Clear() {
        DestroyAllContent();
    }

    void DisplayHeader() {
        GameObject header = Instantiate(HeaderRowPrefab, ContentContainer.transform);
        header.name = "Header";
        for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
            DataColumn column = table.Columns[columnIndex];
            var newEntry = Instantiate(HeaderEntryPrefab, header.transform);

            LayoutElement entryLayout = newEntry.GetComponent<LayoutElement>();
            entryLayout.minWidth = columnIndexToMaxLength[columnIndex];

            TextMeshProUGUI entryTextObject = newEntry.GetComponent<TextMeshProUGUI>();
            entryTextObject.text = column.ColumnName;
        }
    }

    void DisplayRows() {
        for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
            DataRow row = table.Rows[rowIndex];
            var newRowObject = Instantiate(RowPrefab, ContentContainer.transform);
            newRowObject.name = $"Row {rowIndex}";
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                DataColumn column = table.Columns[columnIndex];
                TextMeshProUGUI newEntry = Instantiate(EntryPrefab, newRowObject.transform);
                newEntry.gameObject.name = $"Column {columnIndex}";
                LayoutElement entryLayout = newEntry.GetComponent<LayoutElement>();
                entryLayout.minWidth = columnIndexToMaxLength[columnIndex];
                TextMeshProUGUI entryTextObject = newEntry.GetComponent<TextMeshProUGUI>();
                entryTextObject.text = row[column.ColumnName].ToString();
            }
        }
    }

    void DestroyAllContent() {
        foreach (Transform child in ContentContainer.transform) {
            DestroyImmediate(child.gameObject);
        }
    }
}
