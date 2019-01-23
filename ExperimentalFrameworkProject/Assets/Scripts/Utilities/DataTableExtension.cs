using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

public static class DataTableExtension
{

    const string TabSeparator = "\t";
    const string CommaSeparator = ", ";

    public static void PrintToConsole(this DataTable dt) {
        Debug.Log(dt.AsString());
    }

    

    public static string AsString(this DataTable dt, string separator = TabSeparator, int truncate = 8) {
        string headerString;
        if (truncate < 0) {
            headerString =
                string.Join(separator, dt.Columns.OfType<DataColumn>().Select(x => string.Join(separator, x.ColumnName))) + "\n";
        }
        else {
            headerString =
                string.Join(separator, dt.Columns.OfType<DataColumn>().Select(x => string.Join(separator, x.ColumnName.Truncate(8)))) + "\n";
        }
        
        string tableString = string.Join(Environment.NewLine,
                                 dt.Rows.OfType<DataRow>().Select(x => string.Join(separator, x.ItemArray)));
        return headerString + tableString;
    }

    public static string AsString(this DataRow row, string separator = TabSeparator) {
        string rowString  = string.Join(separator, row.ItemArray);
        return rowString;
    }

    public static string FormattedForCSV(this DataTable dt) {
        return dt.AsString(CommaSeparator, truncate:-1);
    }

}
