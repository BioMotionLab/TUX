using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BML_ExperimentToolkit.Scripts.VariableSystem;

using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public static class DataTableCsvUtility {
        
        public static DataTable DataTableFromCsv(VariableConfig config, string fullPath) {
            
            
            DataTable loadedDataTable = new DataTable();
            
            using (StreamReader streamReader = new StreamReader(fullPath)) {
                string[] headerNames = streamReader.ReadLine().Split(',');
                Debug.Log($"headers : {string.Join(" ", headerNames)}");
                int rowNumber = 0;

                List<DataColumn> columns = new List<DataColumn>();
                
                for (int headerIndex = 0; headerIndex < headerNames.Length; headerIndex++) {
                    string header = headerNames[headerIndex];
                    Variable variableWithName = config.Variables.GetVariableWithName(header);
                    DataColumn newColumn = variableWithName == null ? 
                        config.ColumnNamesSettings.GetColumnWithName(header) : 
                        new DataColumn(variableWithName.Name, variableWithName.Type);
                    loadedDataTable.Columns.Add(newColumn);
                    columns.Add(newColumn);
                }

                rowNumber++;
                while (!streamReader.EndOfStream) {
                    
                    string[] rowValues = streamReader.ReadLine().Split(',');
                    
                    Debug.Log($"row {rowNumber} : {string.Join(" ", rowValues)}");
                    DataRow row = loadedDataTable.NewRow();
                    for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++) {
                        DataColumn column = columns[columnIndex];
                        try {
                            if (column.DataType == typeof(bool)) {
                                Debug.Log($"column {column.ColumnName} value {rowValues[columnIndex]}, col index {columnIndex}");
                                if (rowValues[columnIndex] == "TRUE") row[column.ColumnName] = true;
                                if (rowValues[columnIndex] == "FALSE") row[column.ColumnName] = false;
                            }
                            if (column.DataType == typeof(float) && rowValues[columnIndex] == "") {
                                row[column.ColumnName] = 0;
                            }
                            else {
                                if (rowValues != null) row[column.ColumnName] = Convert.ChangeType(rowValues[columnIndex], column.DataType);
                            }
                        }
                        catch (FormatException e) {
                            Debug.Log($"row: {rowNumber} trying to convert column {column.ColumnName} value {row[column.ColumnName]} to type {column.DataType}");
                            throw;
                        }
                        
                        
                    }

                    loadedDataTable.Rows.Add(row);

                    rowNumber++;
                }
            }


            return loadedDataTable;
        }

    }
}