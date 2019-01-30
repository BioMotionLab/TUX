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
    const int TruncateDefault = 10;

    public static void PrintToConsole(this DataTable dt) {
        Debug.Log(dt.AsString());
    }

    

    public static string AsString(this DataTable dt, string separator = TabSeparator, int truncate = TruncateDefault) {
        string headerString = HeaderAsString(dt, separator, truncate);

        string tableString = string.Join(Environment.NewLine,
                                 dt.Rows.OfType<DataRow>().Select(x => string.Join(separator, x.ItemArray)));
        return headerString + "\n" + tableString;
    }

    public static string HeaderAsString(this DataTable dt, string separator = TabSeparator, int truncate = TruncateDefault) {
        string headerString = string.Join(separator, truncate < 0 ? 
                                       dt.Columns.OfType<DataColumn>().Select(x => string.Join(separator, x.ColumnName)) : 
                                       dt.Columns.OfType<DataColumn>().Select(x => string.Join(separator, x.ColumnName.Truncate(truncate))));
        return headerString;
    }

    public static string AsString(this DataRow row, string separator = TabSeparator) {
        string rowString  = string.Join(separator, row.ItemArray);
        return rowString;
    }

    public static string AsStringWithHeader(this DataRow row, DataTable dt, string separator = TabSeparator, int truncate = TruncateDefault) {
        string headerString = dt.HeaderAsString();
        string rowString = string.Join(separator, row.ItemArray);
        return headerString + "\n" + rowString;
    }


    public static string FormattedForCSV(this DataTable dt) {
        return dt.AsString(CommaSeparator, truncate:-1);
    }

}
