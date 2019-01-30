using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class ExperiementTable : MonoBehaviour {

    public DatumFactory fact;


    // Start is called before the first frame update
    void Start()
    {
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
        //    if (datum.ShuffleOrder) {
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

    public static DataTable AddBalancedVariable(DataTable table, Datum datum) {
        DataTable newTable = table.Clone();

        DataColumn column = new DataColumn {
                                               DataType = datum.Type,
                                               ColumnName = datum.Name,
                                               ReadOnly = false,
                                               Unique = false
                                           };
        newTable.Columns.Add(column);

        
        if (datum.Type == typeof(int)) {
            Debug.Log("detected datum type of int");
            DatumInt intDatum = (DatumInt) datum;
            if (table.Rows.Count == 0) {
                Debug.Log("Adding rows to empty table in variable creation");
                foreach (int value in intDatum.Values) {
                    var newRow = newTable.NewRow();
                    newRow[datum.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                Debug.Log("Adding rows to NON empty table in variable creation");
                foreach (DataRow tableRow in table.Rows) {
                    foreach (int value in intDatum.Values) {

                        newTable.ImportRow(tableRow);
                        var newRow = newTable.Rows[newTable.Rows.Count - 1];
                        newRow[datum.Name] = value;
                    }
                }
            }
            
        }
        else {
            newTable = table.Copy();
        }
        Debug.Log($"table now has {newTable.Rows.Count} rows");
        return newTable;

    }

    
    public static DataTable GetTable(List<Datum> allData, bool shuffleTrialOrder, int numberOfRepetitions) {
        DataTable table = new DataTable();

        List<Datum> balanced = new List<Datum>();
        List<Datum> looped = new List<Datum>();
        List<Datum> evenProbability = new List<Datum>();

        foreach (Datum datum in allData) {
            switch (datum.MixingTypeOfVariable) {
                case VariableMixingType.Balanced:
                    balanced.Add(datum);
                    break;
                case VariableMixingType.Looped:
                    looped.Add(datum);
                    break;
                case VariableMixingType.EvenProbability:
                    evenProbability.Add(datum);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        foreach (Datum datum in balanced) {
            table = AddBalancedVariable(table, datum);
        }

        foreach (Datum datum in looped) {
            table = AddLoopedVariable(table, datum);
        }

        foreach (Datum datum in evenProbability) {
            table = AddEvenProbabilityVariable(table, datum);
        }


        //if (numberOfRepetitions > 1) {
        if (numberOfRepetitions > 1) { 
            DataTable repeatedTable = table.Clone();
            for (int i = 0; i < numberOfRepetitions; i++) {
                foreach (DataRow row in table.Rows) {
                    repeatedTable.ImportRow(row);
                }
            }

            table = repeatedTable;
        }


        if (shuffleTrialOrder) {
            table = table.Shuffle();
        }
        //Add trial number column
        DataColumn indexColumn = new DataColumn {
                                                DataType = typeof(int),
                                                ColumnName = Config.IndexColumnName,
                                                Unique = false,
                                                ReadOnly = false,
                                            };
        table.Columns.Add(indexColumn);
        indexColumn.SetOrdinal(0);// to put the column in position 0;
        int trialIndex = 0;
        foreach (DataRow row in table.Rows) {
            row[Config.IndexColumnName] = trialIndex;
            trialIndex++;
        }

        //Add Successfully run column
        DataColumn successColumn = new DataColumn {
                                                    DataType = typeof(bool),
                                                    ColumnName = Config.SuccessColumnName,
                                                    Unique = false,
                                                    ReadOnly = false,
                                                };
        table.Columns.Add(successColumn);
        foreach (DataRow row in table.Rows) {
            row[Config.SuccessColumnName] = false;
        }

        //Add Attempts column
        DataColumn attemptsColumn = new DataColumn {
                                                      DataType = typeof(int),
                                                      ColumnName = Config.AttemptsColumnName,
                                                      Unique = false,
                                                      ReadOnly = false,
                                                  };
        table.Columns.Add(attemptsColumn);
        foreach (DataRow row in table.Rows) {
            row[Config.AttemptsColumnName] = 0;
        }

        //Add skipped column
        DataColumn skippedColumn = new DataColumn {
                                                       DataType = typeof(bool),
                                                       ColumnName = Config.SkippedColumnName,
                                                       Unique = false,
                                                       ReadOnly = false,
                                                   };
        table.Columns.Add(skippedColumn);
        foreach (DataRow row in table.Rows) {
            row[Config.SkippedColumnName] = false;
        }

        

        return table;
    }

    static DataTable AddLoopedVariable(DataTable table, Datum datum) {
        Debug.Log("Adding looped variable");
        DataTable newTable = table.Clone();

        DataColumn column = new DataColumn {
                                               DataType = datum.Type,
                                               ColumnName = datum.Name,
                                               ReadOnly = false,
                                               Unique = false
                                           };
        newTable.Columns.Add(column);

        
        if (datum.Type == typeof(int)) {
           
            Debug.Log("detected datum type of int");
            DatumInt intDatum = (DatumInt)datum;

            LoopingList < int > loopValues = new LoopingList<int>();
            loopValues.AddRange(intDatum.Values);



            if (table.Rows.Count == 0) {
                Debug.Log("Adding rows to empty table in variable creation");
                foreach (int value in intDatum.Values) {
                    var newRow = newTable.NewRow();
                    newRow[datum.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                int lowestCommonMultiple =
                    LowestCommonFunctions.LowestCommonMultiple(table.Rows.Count, intDatum.Values.Count);

                //Make the required number of copies of the table.
                int numberOfTableCopies = lowestCommonMultiple / table.Rows.Count;
                Debug.Log($"Number of table copies: {numberOfTableCopies}");
                for (int i = 0; i < numberOfTableCopies; i++) {
                    Debug.Log($"Adding {i}th copy of table");
                    foreach (DataRow tableRow in table.Rows) {
                        newTable.ImportRow(tableRow);
                    }
                }
                Debug.Log("Adding rows to NON empty table in looped variable creation");
                int value = loopValues.FirstElement;
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[datum.Name] = value;
                    value = loopValues.NextElement;
                }
            }

        }
        else {
            throw new NotImplementedException("datatype not yet supported for adding variables");
        }
        Debug.Log($"table now has {newTable.Rows.Count} rows");
        return newTable;
    }

    static DataTable AddEvenProbabilityVariable(DataTable table, Datum datum) {
        Debug.Log("Adding EvenProbability variable");
        DataTable newTable = table.Copy();

        DataColumn column = new DataColumn {
            DataType = datum.Type,
            ColumnName = datum.Name,
            ReadOnly = false,
            Unique = false
        };
        newTable.Columns.Add(column);


        if (datum.Type == typeof(int)) {

            Debug.Log("detected datum type of int");
            DatumInt intDatum = (DatumInt)datum;
            

            if (table.Rows.Count == 0) {
                Debug.Log("Adding rows to empty table in variable creation");
                foreach (int value in intDatum.Values) {
                    var newRow = newTable.NewRow();
                    newRow[datum.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                Debug.Log($"Adding values to new table (rows: {newTable.Rows.Count}) in even probability variable creation");
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[datum.Name] = intDatum.Values.RandomItem();
                }
            }

        }
        else {
            throw new NotImplementedException("datatype not yet supported for adding variables");
        }
        Debug.Log($"table now has {newTable.Rows.Count} rows");
        return newTable;
    }



}


