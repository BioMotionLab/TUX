using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace bmlTUX.UI.RuntimeUI {
    public class TableViewer : MonoBehaviour {
    
        public GameObject      ContentContainer;
        public TextMeshProUGUI EntryPrefab;
        public TextMeshProUGUI HeaderEntryPrefab;
        public GameObject      RowPrefab;
        public GameObject      HeaderRowPrefab;
    
        List<GameObject>     entries;
        int[] ColumnLengths;
        DataTable            table;
        int rowLength;
        const int            HeaderPixelMultiplier = 10;
        const int            EntryPixelMultiplier  = 8;
        const int paddingChars = 4;
        const string ASpace = " ";
        
        
        public void Display(DataTable tableToDisplay) {
            Clear();
            table = tableToDisplay;
            
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

            DisplayHeader();
            DisplayRows();
        }

        public void Clear() {
            DestroyAllContent();
        }

        void DisplayHeader() {
            GameObject header = Instantiate(HeaderRowPrefab, ContentContainer.transform);
            header.name = "Header";
            LayoutElement entryLayout = header.GetComponent<LayoutElement>();
            entryLayout.minWidth = rowLength * EntryPixelMultiplier;
            
            
            var newEntry = Instantiate(EntryPrefab, header.transform);
            
            
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

        void DisplayRows() {
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
                DataRow row = table.Rows[rowIndex];
                
                GameObject newRow = Instantiate(RowPrefab, ContentContainer.transform);
                newRow.name = "Row {rowIndex}";
                LayoutElement entryLayout = newRow.GetComponent<LayoutElement>();
                entryLayout.minWidth = rowLength * EntryPixelMultiplier;
                
                var newRowEntry = Instantiate(EntryPrefab, newRow.transform);
                
        
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

        void DestroyAllContent() {
            foreach (Transform child in ContentContainer.transform) {
                Destroy(child.gameObject);
            }
        }
    }
}
