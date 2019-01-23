using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class ExperiementTable : MonoBehaviour {

    public DatumFactory fact;


    // Start is called before the first frame update
    void Start()
    {
        ////add block variable columns
        //foreach (Datum datum in fact.AllData) {
        //    DataColumn column = new DataColumn();
        //    column.DataType = datum.Type;
        //    column.ColumnName = datum.Name;
        //    column.ReadOnly = true;
        //    column.Unique = false;
        //    table.Columns.Add(column);
        //}

        //var data = fact.AllData;
        //for (int i = 0; i < data.Count; i++) {

        //    for (int j = i+1; j < data.Count; j++) {
                
        //    }
        //}
        //foreach (Datum datum in fact.AllData) {
        //    DataRow row = table.NewRow();

        //    var values = datum.Values;
        //    if (datum.RandomizeOrder) {
        //        values = values.shuffle();
        //    }
        //    foreach (var value in values) {
                
        //        row[trialNumberColumn] = i;
        //        row[otherDatum.Name] = value;
        //        table.Rows.Add(row);
        //        i++;
        //    }

                
            
            
        //}
        

        
    }

    public static DataTable AddVariable(DataTable table, Datum datum) {
        DataTable newTable = table.Clone();

        DataColumn column = new DataColumn {
                                               DataType = datum.Type,
                                               ColumnName = datum.Name,
                                               ReadOnly = false,
                                               Unique = false
                                           };
        newTable.Columns.Add(column);

        
        if (datum.Type == typeof(int)) {
            Debug.Log("detected datum type of int");
            DatumInt intDatum = (DatumInt) datum;
            if (table.Rows.Count == 0) {
                Debug.Log("Adding rows to empty table in variable creation");
                foreach (int value in intDatum.Values) {
                    var newRow = newTable.NewRow();
                    newRow[datum.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                Debug.Log("Adding rows to NON empty table in variable creation");
                foreach (DataRow tableRow in table.Rows) {
                    foreach (int value in intDatum.Values) {

                        newTable.ImportRow(tableRow);
                        var newRow = newTable.Rows[newTable.Rows.Count - 1];
                        newRow[datum.Name] = value;
                    }
                }
            }
            
        }
        else {
            newTable = table.Copy();
        }
        Debug.Log($"table now has {newTable.Rows.Count} rows");
        return newTable;

    }

    
    public static DataTable GetTable(List<Datum> allData) {
        DataTable table = new DataTable();
        foreach (Datum datum in allData) {
            table = AddVariable(table, datum);
        }
        
        //Add trial number column
        DataColumn indexColumn = new DataColumn {
                                                    DataType = typeof(int),
                                                    ColumnName = Config.IndexColumnName,
                                                    Unique = false,
                                                    ReadOnly = false,
                                                };
        table.Columns.Add(indexColumn);
        indexColumn.SetOrdinal(0);// to put the column in position 0;
        int i = 0;
        foreach (DataRow row in table.Rows) {
            row[Config.IndexColumnName] = i;
            i++;
        }

        //Add Successfully run column
        DataColumn successColumn = new DataColumn {
                                                    DataType = typeof(bool),
                                                    ColumnName = Config.SuccessColumnName,
                                                    Unique = false,
                                                    ReadOnly = false,
                                                };
        table.Columns.Add(successColumn);
        foreach (DataRow row in table.Rows) {
            row[Config.SuccessColumnName] = false;
        }

        //Add Attempts column
        DataColumn attemptsColumn = new DataColumn {
                                                      DataType = typeof(int),
                                                      ColumnName = Config.AttemptsColumnName,
                                                      Unique = false,
                                                      ReadOnly = false,
                                                  };
        table.Columns.Add(attemptsColumn);
        foreach (DataRow row in table.Rows) {
            row[Config.AttemptsColumnName] = 0;
        }

        return table;
    }
}
