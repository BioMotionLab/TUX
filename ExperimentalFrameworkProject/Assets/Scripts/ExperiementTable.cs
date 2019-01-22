using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class ExperiementTable : MonoBehaviour {

    public DatumFactory fact;

    //DataTable table = new DataTable("TrialData");
    const string trialIdString = "TrialNumber";


    // Start is called before the first frame update
    void Start()
    {
        ////add system columns
        //DataColumn trialNumberColumn = new DataColumn();
        //trialNumberColumn.DataType = typeof(int);
        //trialNumberColumn.ColumnName = trialIdString;
        //trialNumberColumn.ReadOnly = true;
        //trialNumberColumn.Unique = false;
        //table.Columns.Add(trialNumberColumn);
        

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
            DatumInt intDatum = (DatumInt) datum;
            if (table.Rows.Count == 0) {
                foreach (int value in intDatum.Values) {
                    var newRow = newTable.NewRow();
                    newRow[datum.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                foreach (DataRow tableRow in table.Rows) {
                    foreach (int value in intDatum.Values) {
                        newTable.ImportRow(tableRow);
                        var newRow = newTable.Rows[newTable.Rows.Count - 1];
                        newRow[datum.Name] = value;
                    }
                }
            }
            
        }

        return newTable;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static DataTable GetTable(List<Datum> allData) {
        DataTable table = new DataTable();
        foreach (Datum datum in allData) {
            table = AddVariable(table, datum);
        }

        return table;
    }
}
