using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Datatableshuffler : MonoBehaviour
{
    

    

    void Start() {
        DataTable table = new DataTable();
        DataColumn col = new DataColumn();
        col.DataType = typeof(string);
        col.ColumnName = "string";
        table.Columns.Add(col);
        for (int i = 0; i <10; i++) {
            var row = table.NewRow();
            row["string"] = i.ToString();
            table.Rows.Add(row);
        }
        Debug.Log(table.AsString());
        Debug.Log(table.Shuffle().AsString());


    }
}
