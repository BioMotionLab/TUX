using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BML_Utilities {


    public static class DataTableExtension {

        const int TruncateDefault = 8;

        public static void PrintToConsole(this DataTable dt) {
            Debug.Log(dt.AsString());
        }



        public static string AsString(this DataTable dt, bool header = true, string separator = Delimiter.Tab,
                                      int            truncate = TruncateDefault) {
            string headerString = header ? HeaderAsString(dt, separator, truncate) + "\n" : "";

            string tableString = string.Join(Environment.NewLine,
                                             dt.Rows.OfType<DataRow>()
                                                 .Select(x => string.Join(separator, x.ItemArray)));
            return headerString + tableString;
        }

        public static string HeaderAsString(this DataTable dt, string separator = Delimiter.Tab,
                                            int            truncate = TruncateDefault) {
            string headerString =
                string.Join(separator,
                            truncate > 0
                                ? dt.Columns.OfType<DataColumn>()
                                    .Select(x => string.Join(separator, x.ColumnName.Truncate(truncate)))
                                : dt.Columns.OfType<DataColumn>().Select(x => string.Join(separator, x.ColumnName))
                           );
            return headerString;
        }

        public static string AsString(this DataRow row, bool header = false, string separator = Delimiter.Tab,
                                      int          truncate = TruncateDefault) {
            string headerString =
                header ? row.Table.HeaderAsString(separator: separator, truncate: truncate) + "\n" : "";
            string rowString = truncate <= 0
                ? string.Join(separator, row.ItemArray.Select(c => c.ToString()).ToArray())
                : string.Join(separator, row.ItemArray.Select(c => c.ToString().Truncate(truncate)).ToArray());
            return headerString + rowString;
        }

        public static string AsStringWithColumnNames(this DataRow row, string separator = Delimiter.Tab,
                                      int          truncate = TruncateDefault) {
            string headerString =
                row.Table.HeaderAsString(separator: separator, truncate: truncate);
            string rowString = truncate <= 0
                ? string.Join(separator, row.ItemArray.Select(c => c.ToString()).ToArray())
                : string.Join(separator, row.ItemArray.Select(c => c.ToString().Truncate(truncate)).ToArray());
            string[] headerStrings = headerString.Split(new[] { separator}, StringSplitOptions.None);
            string[] rowStrings = rowString.Split(new [] {separator}, StringSplitOptions.None);
            string stringWithColumnNames = "";
            for (int i = 0; i < headerStrings.Length; i++) {
                string separatorString = i < headerStrings.Length - 1 ? separator : "";
                stringWithColumnNames += $"{headerStrings[i]}: {rowStrings[i]}{separatorString}";
            }

            return stringWithColumnNames;
        }

        public static List<List<DataRow>> GetPermutations(this DataTable dt) {
            List<DataRow> dataRows = new List<DataRow>();
            foreach (DataRow row in dt.Rows) {
                dataRows.Add(row);
            }

            IEnumerable<IList> permutations = Permutate(dataRows, dataRows.Count);

            List<List<DataRow>> allPermutations = new List<List<DataRow>>();
            foreach (IList perms in permutations) {
                List<DataRow> permDataRows = new List<DataRow>();
                foreach (DataRow permDataRow in perms) {
                    permDataRows.Add(permDataRow);

                }

                allPermutations.Add(permDataRows);
            }

            return allPermutations;
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

        public static DataTable AddColumnFromOtherTable(this DataTable table, DataColumn columnToAdd, int index = -1) {
            DataTable newTable = table.Copy();
            DataColumn column = new DataColumn {
                                                   DataType = columnToAdd.DataType,
                                                   ColumnName = columnToAdd.ColumnName,
                                                   ReadOnly = false,
                                                   Unique = false
                                               };
            newTable.Columns.Add(column);
            if (index >= 0) {
                column.SetOrdinal(index);
            }

            return newTable;
        }


        /// <summary>
        /// Randomly shuffles the row order of this table
        /// </summary>
        /// <param name="table"></param>
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


    }
}