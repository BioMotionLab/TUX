using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class ExperimentTable : MonoBehaviour {


    public static DataTable GetTable(List<Variable> allData, bool shuffleTrialOrder, int numberOfRepetitions) {
        DataTable table = new DataTable();

        List<IndependentVariable> balanced = new List<IndependentVariable>();
        List<IndependentVariable> looped = new List<IndependentVariable>();
        List<IndependentVariable> probability = new List<IndependentVariable>();
        
        //Sort datums into mixing categories so they go in order
        foreach (Variable datum in allData) {
            if (datum.TypeOfVariable == VariableType.Independent) {
                IndependentVariable IvDatum = (IndependentVariable) datum;
                switch (IvDatum.MixingTypeOfVariable) {
                    case VariableMixingType.Balanced:
                        balanced.Add(IvDatum);
                        break;
                    case VariableMixingType.Looped:
                        looped.Add(IvDatum);
                        break;
                    case VariableMixingType.EvenProbability:
                        probability.Add(IvDatum);
                        break;
                    case VariableMixingType.CustomProbability:
                        probability.Add(IvDatum);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            
        }

        Debug.Log($"1: Current table rows in getTable() {table.Rows.Count}");

        //do balanced variables first
        foreach (IndependentVariable datum in balanced) {
            table = AddVariableGeneric(datum, table);
        }

        //do looped variables second
        foreach (IndependentVariable datum in looped) {
            table = AddVariableGeneric(datum, table);
        }

        //do probability variables last
        foreach (IndependentVariable datum in probability) {
            table = AddVariableGeneric(datum, table);
        }
        //TODO custom probability

        Debug.Log($"2: Current table rows in getTable() {table.Rows.Count}");


        //Repeat all trials if specified
        if (numberOfRepetitions > 1) { 
            DataTable repeatedTable = table.Clone();
            for (int i = 0; i < numberOfRepetitions; i++) {
                foreach (DataRow row in table.Rows) {
                    repeatedTable.ImportRow(row);
                }
            }
            table = repeatedTable;
        }
        Debug.Log($"3: Current table rows in getTable() {table.Rows.Count}");

        //Shuffle trial order if needed
        if (shuffleTrialOrder) {
            table = table.Shuffle();
        }

        Debug.Log($"4: Current table rows in getTable() {table.Rows.Count}");

        //Add trial number column
        AddTrialNumberColumnTo(table);

        //Add Successfully run column
        AddSuccessColumnTo(table);

        //Add Attempts column
        AddAttemptsColumnTo(table);

        //Add skipped column
        AddSkippedColumnTo(table);

        Debug.Log($"5: Current table rows in getTable() {table.Rows.Count}");

        return table;
    }

    static void AddSkippedColumnTo(DataTable table) {
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
    }

    static void AddAttemptsColumnTo(DataTable table) {
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
    }

    static void AddSuccessColumnTo(DataTable table) {
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
    }

    static void AddTrialNumberColumnTo(DataTable table) {
        DataColumn indexColumn = new DataColumn {
                                                    DataType = typeof(int),
                                                    ColumnName = Config.IndexColumnName,
                                                    Unique = false,
                                                    ReadOnly = false,
                                                };
        table.Columns.Add(indexColumn);
        indexColumn.SetOrdinal(0); // to put the column in position 0;
        int trialIndex = 0;
        foreach (DataRow row in table.Rows) {
            row[Config.IndexColumnName] = trialIndex;
            trialIndex++;
        }
    }

    static DataTable AddVariableGeneric(IndependentVariable independentVariable, DataTable table) {
        DataTable newTable = new DataTable();
        switch (independentVariable.DataType) {
            case SupportedDataTypes.Int:
                newTable = AddVariable<int>(table, independentVariable);
                break;
            case SupportedDataTypes.Float:
                newTable = AddVariable<float>(table, independentVariable);
                break;
            case SupportedDataTypes.String:
                newTable = AddVariable<string>(table, independentVariable);
                break;
            case SupportedDataTypes.ChooseType:
                throw new ArgumentException("type not chosen");
            default:
                throw new ArgumentOutOfRangeException();
        }
        Debug.Log($"table now has {newTable.Rows.Count} rows");
        return newTable;
    }


    static DataTable AddVariable<T>(DataTable table, IndependentVariable independentVariable) {

        DataTable newTable = table.Clone();
        AddVariableColumn(independentVariable, newTable);

        switch (independentVariable.MixingTypeOfVariable) {
            case VariableMixingType.Balanced:
                newTable = AddBalancedValues<T>(table, independentVariable);
                break;
            case VariableMixingType.Looped:
                newTable = AddLoopedValues<T>(table, independentVariable);
                break;
            case VariableMixingType.EvenProbability:
                newTable = AddEvenProbabilityValues<T>(table, independentVariable);
                break;
            case VariableMixingType.CustomProbability:
                newTable = AddCustomProbabilityValues<T>(table, independentVariable);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Debug.Log($"(AddVariable<T> table now has {newTable.Rows.Count} rows");
        return newTable;
    }




    static DataTable AddBalancedValues<T>(DataTable table, IndependentVariable independentVariable) {

        DataTable newTable = table.Clone();

        AddVariableColumn(independentVariable, newTable);

        IndependentVariable<T> castIndependentVariable = (IndependentVariable<T>)independentVariable;
        if (castIndependentVariable.Values.Count == 0) {
            throw new ArgumentException($"No values defined for variable {independentVariable.Name}");
        }


        if (table.Rows.Count == 0) {
            Debug.Log("Adding rows to empty table in variable creation");
            
            foreach (T value in castIndependentVariable.Values) {
                var newRow = newTable.NewRow();
                newRow[independentVariable.Name] = value;
                newTable.Rows.Add(newRow);
            }
        }
        else {
            Debug.Log("Adding rows to NON empty table in variable creation");
            foreach (DataRow tableRow in table.Rows) {
                foreach (T value in castIndependentVariable.Values) {
                    newTable.ImportRow(tableRow);
                    var newRow = newTable.Rows[newTable.Rows.Count - 1];
                    newRow[independentVariable.Name] = value;
                }
            }
        }

        return newTable;

    }

    static DataTable AddLoopedValues<T>(DataTable table, IndependentVariable independentVariable) {
        DataTable newTable = table.Clone();

        AddVariableColumn(independentVariable, newTable);

        IndependentVariable<T> castIndependentVariable = (IndependentVariable<T>)independentVariable;
        if (castIndependentVariable.Values.Count == 0) {
            throw new ArgumentNullException($"No values defined for variable {independentVariable.Name}");
        }


        LoopingList<T> loopValues = new LoopingList<T>();
        loopValues.AddRange(castIndependentVariable.Values);


        if (table.Rows.Count == 0) {
            Debug.Log("Adding rows to empty table in variable creation");
            foreach (T value in castIndependentVariable.Values) {
                var newRow = newTable.NewRow();
                newRow[independentVariable.Name] = value;
                newTable.Rows.Add(newRow);
            }
        }
        else {
            int lowestCommonMultiple =
                LowestCommonFunctions.LowestCommonMultiple(table.Rows.Count, castIndependentVariable.Values.Count);

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
            T value = loopValues.FirstElement;
            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[independentVariable.Name] = value;
                value = loopValues.NextElement;
            }
        }

        return newTable;
    }

    static DataTable AddEvenProbabilityValues<T>(DataTable table, IndependentVariable independentVariable) {

        DataTable newTable = table.Copy();

        AddVariableColumn(independentVariable, newTable);

        IndependentVariable<T> castIndependentVariable = (IndependentVariable<T>) independentVariable;
        if (castIndependentVariable.Values.Count == 0) {
            throw new ArgumentNullException($"No values defined for variable {independentVariable.Name}");
        }


        if (table.Rows.Count == 0) {
            Debug.Log("Adding rows to empty table in variable creation");
            foreach (T value in castIndependentVariable.Values) {
                var newRow = newTable.NewRow();
                newRow[independentVariable.Name] = value;
                newTable.Rows.Add(newRow);
            }
        }
        else {
            Debug.Log($"Adding values to new table (rows: {table.Rows.Count}) in even probability variable creation");
            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[independentVariable.Name] = castIndependentVariable.Values.RandomItem();
            }
        }

        return newTable;
    }

    static DataTable AddCustomProbabilityValues<T>(DataTable table, IndependentVariable independentVariable) {
        DataTable newTable = table.Copy();


        AddVariableColumn(independentVariable, newTable);


        IndependentVariable<T> castedIndependentVariable = (IndependentVariable<T>)independentVariable;

        List<T> distribution = new List<T>();
        for (int i = 0; i < castedIndependentVariable.Probabilities.Count; i++) {
            float prob = castedIndependentVariable.Probabilities[i];
            T val = castedIndependentVariable.Values[i];
            int number = (int)(prob * 1000);
            for (int j = 0; j < number; j++) {
                distribution.Add(val);
            }
        }

        if (table.Rows.Count == 0) {
            Debug.Log("Adding rows to empty table in variable creation");
            foreach (T value in castedIndependentVariable.Values) {
                var newRow = newTable.NewRow();
                newRow[independentVariable.Name] = value;
                newTable.Rows.Add(newRow);
            }
        }
        else {
            Debug.Log($"Adding values to new table (rows: {table.Rows.Count}) in even probability variable creation");
            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[independentVariable.Name] = distribution.RandomItem();
            }
        }

        return newTable;
    }



    static void AddVariableColumn(IndependentVariable independentVariable, DataTable newTable) {
        DataColumn column = new DataColumn {
                                               DataType = independentVariable.Type,
                                               ColumnName = independentVariable.Name,
                                               ReadOnly = false,
                                               Unique = false
                                           };
        newTable.Columns.Add(column);
    }
}


