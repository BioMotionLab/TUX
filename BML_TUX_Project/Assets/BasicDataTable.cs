using System.Collections;
using System.Collections.Generic;
using System.Data;
using BML_Utilities.Extensions;
using UnityEngine;

[ExecuteInEditMode]
public class BasicDataTable : MonoBehaviour {

    public DataTable table;
    
    [ContextMenu("Generate")]
    public void Generate()
    {
        table = new DataTable();
        DataColumn column1 = new DataColumn(columnName:"Head1", typeof(int));
        table.Columns.Add(column1);
        
        DataColumn column2 = new DataColumn(columnName:"Head2", typeof(int));
        table.Columns.Add(column2);
        
        DataColumn column3 = new DataColumn(columnName:"Head3", typeof(int));
        table.Columns.Add(column3);


        DataColumn column4 = new DataColumn(columnName:"Head4", typeof(string));
        table.Columns.Add(column4);
        DataColumn column5 = new DataColumn(columnName:"Head5", typeof(int));
        table.Columns.Add(column5);
        DataColumn column6 = new DataColumn(columnName:"Head6", typeof(int));
        table.Columns.Add(column6);
        
        for (int i = 0; i < 30; i++) {
            DataRow newRow = table.NewRow();
            newRow["Head1"] = i;
            newRow["Head2"] = i*i;
            newRow["Head3"] = i*i*i;
            newRow["head4"] = "SomeLONGGGGGGtext ThatIsReallyLong";
            newRow["Head5"] = i;
            newRow["Head6"] = i;
            table.Rows.Add(newRow);
        }
        
        DataRow longRow = table.NewRow();
        longRow["Head1"] = 2147483647;
        longRow["Head2"] = 2147483647;
        longRow["Head3"] = 2147483647;
        table.Rows.Add(longRow);
        
        Debug.Log(table.AsString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
