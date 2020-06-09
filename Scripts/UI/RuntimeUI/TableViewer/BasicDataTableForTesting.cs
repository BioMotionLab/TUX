using System.Data;
using UnityEngine;

namespace bmlTUX.Scripts.UI.RuntimeUI.TableViewer {
    [ExecuteInEditMode]
    public class BasicDataTableForTesting : MonoBehaviour {

        public DataTable Table;


        public TableViewer TableDisplay;
        [ContextMenu("Generate")]
        public void Generate()
        {
            Table = new DataTable();
            DataColumn column1 = new DataColumn(columnName:"Head1", typeof(int));
            Table.Columns.Add(column1);
        
            DataColumn column2 = new DataColumn(columnName:"Head2", typeof(int));
            Table.Columns.Add(column2);
        
            DataColumn column3 = new DataColumn(columnName:"Head3", typeof(int));
            Table.Columns.Add(column3);


            DataColumn column4 = new DataColumn(columnName:"Head4", typeof(string));
            Table.Columns.Add(column4);
            DataColumn column5 = new DataColumn(columnName:"Head5", typeof(int));
            Table.Columns.Add(column5);
            DataColumn column6 = new DataColumn(columnName:"Head6", typeof(int));
            Table.Columns.Add(column6);
        
            for (int i = 0; i < 300; i++) {
                DataRow newRow = Table.NewRow();
                newRow["Head1"] = i;
                newRow["Head2"] = i*i;
                newRow["Head3"] = i*i*i;
                newRow["head4"] = "SomeLONGGGGGGtext ThatIsReallyLong";
                newRow["Head5"] = i;
                newRow["Head6"] = i;
                Table.Rows.Add(newRow);
            }
        
            DataRow longRow = Table.NewRow();
            longRow["Head1"] = 2147483647;
            longRow["Head2"] = 2147483647;
            longRow["Head3"] = 2147483647;
            Table.Rows.Add(longRow);
        
            TableDisplay.Display(Table);
        }

        [ContextMenu("Clear")]
        public void Clear() {
            TableDisplay.Clear();
        }

    }
}
