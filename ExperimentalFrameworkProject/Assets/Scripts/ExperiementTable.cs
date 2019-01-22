using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.;

public class ExperiementTable : MonoBehaviour {

    public DatumFactory fact;

    DataTable table = new DataTable("TrialData");
    const string trialIdString = "TrialNumber";


    // Start is called before the first frame update
    void Start()
    {
        //add system columns
        DataColumn trialNumberColumn = new DataColumn();
        trialNumberColumn.DataType = typeof(int);
        trialNumberColumn.ColumnName = trialIdString;
        trialNumberColumn.ReadOnly = true;
        trialNumberColumn.Unique = false;
        table.Columns.Add(trialNumberColumn);
        

        //add block variable columns
        foreach (Datum datum in fact.AllData) {
            DataColumn column = new DataColumn();
            column.DataType = datum.Type;
            column.ColumnName = datum.Name;
            column.ReadOnly = true;
            column.Unique = false;
            table.Columns.Add(column);
        }

        var data = fact.AllData;
        for (int i = 0; i < data.Count; i++) {

            for (int j = i+1; j < data.Count; j++) {
                
            }
        }
        foreach (Datum datum in fact.AllData) {
            DataRow row = table.NewRow();

            var values = datum.Values;
            if (datum.RandomizeOrder) {
                values = values.shuffle();
            }
            foreach (var value in values) {
                
                row[trialNumberColumn] = i;
                row[otherDatum.Name] = value;
                table.Rows.Add(row);
                i++;
            }

                
            
            
        }
        

        
    }

    DataTable AddVariable(DataTable table, Datum datum) {
        DataTable newTable = table.Copy();
        DataColumn column = new DataColumn();
        column.DataType = datum.Type;
        column.ColumnName = datum.Name;
        column.ReadOnly = true;
        column.Unique = false;
        newTable.Columns.Add(column);
        foreach (DataRow tableRow in table.Rows) {
            foreach (var value in datum.Values) {

                DataRow newRow = newTable.NewRow();
                newRow 
            }
        }
        foreach (var value in datum.Values) {
            
            newData.RemoveAt(0);
            IterateThroughDatums(newData);
        }

        return data;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
