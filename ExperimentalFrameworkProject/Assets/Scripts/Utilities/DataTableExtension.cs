using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public static string AsString(this DataRow row, string separator = TabSeparator, int truncate = TruncateDefault) {
        string rowString = truncate <= 0
            ? string.Join(separator, row.ItemArray.Select(c => c.ToString()).ToArray())
            : string.Join(separator, row.ItemArray.Select(c => c.ToString().Truncate(truncate)).ToArray());
        return rowString;
    }

    public static string AsStringWithHeader(this DataRow row, string separator = TabSeparator, int truncate = TruncateDefault) {
        string headerString = row.Table.HeaderAsString();
        string rowString = string.Join(separator, row.ItemArray.Select(c => c.ToString().Truncate(truncate)).ToArray());
        return headerString + "\n" + rowString;
    }

    public static List<List<DataRow>> GetPermutations(this DataTable dt) {
        List<DataRow> dataRows = new List<DataRow>();
        foreach (DataRow row in dt.Rows) {
            dataRows.Add(row);
        }

        IEnumerable<IList> permutations = Permutate(dataRows, dataRows.Count);

        List<List<DataRow>> AllPermutations = new List<List<DataRow>>();
        foreach (IList perms in permutations) {
            List<DataRow> permDataRows = new List<DataRow>();
            foreach (DataRow permDataRow in perms) {
                permDataRows.Add(permDataRow);

            }

            AllPermutations.Add(permDataRows);
        }

        return AllPermutations;
    }

    public static void RotateRight(IList sequence, int count) {
        object tmp = sequence[count - 1];
        sequence.RemoveAt(count - 1);
        sequence.Insert(0, tmp);
    }

    public static IEnumerable<IList> Permutate(IList sequence, int count) {
        if (count == 1) yield return sequence;
        else {
            for (int i = 0; i < count; i++) {
                foreach (var perm in Permutate(sequence, count - 1))
                    yield return perm;
                RotateRight(sequence, count);
            }
        }
    }




    /// <summary>
    /// Randomly shuffles the item order of this list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static DataTable Shuffle(this DataTable table) {
        int n = table.Rows.Count;
        List<DataRow> shuffledRows = new List<DataRow>();
        foreach (DataRow row in table.Rows) {
            shuffledRows.Add(row);
        }
        
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            DataRow value = shuffledRows[k];
            shuffledRows[k] = shuffledRows[n];
            shuffledRows[n] = value;
        }

        DataTable shuffledTable = table.Clone();
        foreach (DataRow row in shuffledRows) {
            shuffledTable.ImportRow(row);
        }

        return shuffledTable;
    }

    public static string FormattedForCSV(this DataTable dt) {
        return dt.AsString(CommaSeparator, truncate:-1);
    }

}
