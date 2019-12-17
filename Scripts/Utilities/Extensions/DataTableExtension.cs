using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace bmlTUX.Scripts.Utilities.Extensions {


    public static class DataTableExtension {

        const int TruncateDefault = 8;

        [PublicAPI]
        public static void PrintToDebugConsole(this DataTable dt) {
            Debug.Log($"{TuxLog.Prefix} {dt.AsString()}");
        }

        public static string AsString(this DataTable dt, bool header = true, string separator = Delimiter.Tab,
                                      int            truncateLength = TruncateDefault) {
            string headerString = header ? HeaderAsString(dt, separator, truncateLength) + Environment.NewLine : "";

            string tableString = "";
            foreach (DataRow row in dt.Rows) {
                string rowString = GetRowString(row, separator, truncateLength);
                tableString +=  rowString + Environment.NewLine;
            }
            
            return headerString + tableString;
        }

        static string GetRowString(DataRow row, string separator, int truncateLength) {
            List<string> rowStrings = new List<string>();
            foreach (DataColumn dataColumn in row.Table.Columns) {
                string unFormattedString = row[dataColumn.ColumnName].ToString();
                string formattedString = new DataFormatter(unFormattedString).Formatted;
                if (truncateLength >= 0) {
                    formattedString = formattedString.Truncate(truncateLength);
                }
                rowStrings.Add(formattedString);
            }
            return string.Join(separator, rowStrings);
        }

        public static string HeaderAsString(this DataTable dt, string separator = Delimiter.Tab,
                                            int            truncate = TruncateDefault) {
            string headerString =
                string.Join(separator, truncate > 0
                                ? dt.Columns.OfType<DataColumn>()
                                    .Select(x => string.Join(separator, x.ColumnName.Truncate(truncate)))
                                : dt.Columns.OfType<DataColumn>().Select(x => string.Join(separator, x.ColumnName))
                           );
            return headerString;
        }

        public static string AsString(this DataRow row, bool header = false, string separator = Delimiter.Tab,
                                      int          truncateLength = TruncateDefault) {
            string headerString =
                header ? row.Table.HeaderAsString(separator: separator, truncate: truncateLength) + "\n" : "";
            string rowString = GetRowString(row, separator, truncateLength);
            
            return headerString + rowString;
        }

        [PublicAPI]
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

        static void RotateRight(IList sequence, int count) {
            object tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        static IEnumerable<IList> Permutate(IList sequence, int count) {
            if (count == 1) yield return sequence;
            else {
                for (int i = 0; i < count; i++) {
                    foreach (IList perm in Permutate(sequence, count - 1))
                        yield return perm;
                    RotateRight(sequence, count);
                }
            }
        }

        /// <summary>
        /// Adds a Column to the DataTable that exists in another DataTable
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnToAdd"></param>
        /// <param name="index"></param>
        /// <returns></returns>
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
        public static DataTable ShuffleRows(this DataTable table) {
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