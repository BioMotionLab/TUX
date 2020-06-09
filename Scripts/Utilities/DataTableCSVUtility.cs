using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using bmlTUX.Scripts.VariableSystem;
using UnityEngine;

namespace bmlTUX.Scripts.Utilities {
    public static class DataTableCsvUtility {
        
        public static DataTable DataTableFromCsv(ExperimentDesignFile configurationFile, string fullPath) {
            
            DataTable loadedDataTable = new DataTable();
            
            using (StreamReader streamReader = new StreamReader(fullPath)) {
                
                int rowNumber = 0;
                List<DataColumn> columns = GetColumns(configurationFile, streamReader, loadedDataTable);

                rowNumber++;
                while (!streamReader.EndOfStream) {
                    string[] rowValues = streamReader.ReadLine()?.Split(',');
                    DataRow row = loadedDataTable.NewRow();
                    for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++) {
                        DataColumn column = columns[columnIndex];
                        try {
                            LoadAndSetValue(column, rowValues, columnIndex, row);
                        }
                        catch (FormatException) {
                            Debug.Log($"{TuxLog.Prefix} row: {rowNumber} trying to convert column {column.ColumnName} value {row[column.ColumnName]} to type {column.DataType}");
                            throw;
                        }
                    }

                    loadedDataTable.Rows.Add(row);
                    rowNumber++;
                }
            }
            return loadedDataTable;
        }

        static void LoadAndSetValue(DataColumn column, string[] rowValues, int columnIndex, DataRow row) {
            if (column.DataType == typeof(bool)) {
                ProcessBool(column, rowValues, columnIndex, row);
            }
            if (column.DataType == typeof(float) && rowValues[columnIndex] == "") {
                row[column.ColumnName] = 0;
            }
            else {
                if (rowValues != null) row[column.ColumnName] = Convert.ChangeType(rowValues[columnIndex], column.DataType);
            }
        }

        static void ProcessBool(DataColumn column, string[] rowValues, int columnIndex, DataRow row) {
            if (rowValues[columnIndex] == "TRUE") row[column.ColumnName] = true;
            if (rowValues[columnIndex] == "FALSE") row[column.ColumnName] = false;
        }

        static List<DataColumn> GetColumns(ExperimentDesignFile configurationFile, StreamReader streamReader, DataTable loadedDataTable) {
            string[] headerNames = streamReader.ReadLine()?.Split(',');
            
            List<DataColumn> columns = new List<DataColumn>();

            if (headerNames == null) throw new NullReferenceException("Headers not found");
            
            foreach (string header in headerNames) {
                Variable variableWithName = configurationFile.Variables.GetVariableWithName(header);
                DataColumn newColumn = variableWithName == null
                    ? configurationFile.ColumnNamesSettings.GetColumnWithName(header)
                    : new DataColumn(variableWithName.Name, variableWithName.Type);
                loadedDataTable.Columns.Add(newColumn);
                columns.Add(newColumn);
            }

            return columns;
        }
    }
}