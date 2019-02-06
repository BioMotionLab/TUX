using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Text;

public class ExperimentTable {

    public DataTable blocks;
    public DataTable trials;

    public static ExperimentTable GetTables(List<Variable> allData, bool shuffleTrialOrder, int numberOfRepetitions) {


        DataTable blockTable = new DataTable();
        List<Variable> blockVariables = new List<Variable>();

        foreach (Variable datum in allData) {
            if (datum.TypeOfVariable == VariableType.Independent) {
                IndependentVariable IvDatum = (IndependentVariable)datum;
                if (IvDatum.Block) {
                    blockVariables.Add(IvDatum);
                }
            }
        }

        blockTable = SortAndAddIVs(blockVariables, blockTable, true);

        DataTable baseTable = new DataTable();
        baseTable = SortAndAddIVs(allData, baseTable);


        //Repeat all trials if specified
        if (numberOfRepetitions > 1) {
            DataTable repeatedTable = baseTable.Clone();
            for (int i = 0; i < numberOfRepetitions; i++) {
                foreach (DataRow row in baseTable.Rows) {
                    repeatedTable.ImportRow(row);
                }
            }
            baseTable = repeatedTable;
        }
        Debug.Log($"3: Current table rows in getTable() {baseTable.Rows.Count}");

        //Shuffle trial order if needed
        if (shuffleTrialOrder) {
            baseTable = baseTable.Shuffle();
        }


        Debug.Log($"4: Current table rows in getTable() {baseTable.Rows.Count}");

        //Add trial number column
        AddTrialNumberColumnTo(baseTable);

        //Add Successfully run column
        AddSuccessColumnTo(baseTable);

        //Add Attempts column
        AddAttemptsColumnTo(baseTable);

        //Add skipped column
        AddSkippedColumnTo(baseTable);

        Debug.Log($"5: Current table rows in getTable() {baseTable.Rows.Count}");

        for (int i = 0; i < blockVariables.Count; i++) {
            Variable blockVariable = blockVariables[i];
            AddVariableColumn(blockVariable, baseTable, i);
        }

        
            
        

        ExperimentTable experimentTable = new ExperimentTable();
        experimentTable.trials = baseTable;
        Debug.Log($"***basetable\n {baseTable.AsString()}");
        experimentTable.blocks = blockTable;
        Debug.Log($"***blockTable\n {blockTable.AsString()}");
        return experimentTable;
    }

    static DataTable SortAndAddIVs(List<Variable> allData, DataTable table, bool block=false) {
        List<IndependentVariable> balanced = new List<IndependentVariable>();
        List<IndependentVariable> looped = new List<IndependentVariable>();
        List<IndependentVariable> probability = new List<IndependentVariable>();

        List<DependentVariable> dependentVariables = new List<DependentVariable>();

        //Sort Independent variables into mixing categories so they go in order
        foreach (Variable datum in allData) {
            if (datum.TypeOfVariable == VariableType.Independent) {
                IndependentVariable IvDatum = (IndependentVariable) datum;

                if (block && IvDatum.Block || !block && !IvDatum.Block) {

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
            else if (datum.TypeOfVariable == VariableType.Dependent) {
                DependentVariable dVDatum = (DependentVariable) datum;
                dependentVariables.Add(dVDatum);
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


        foreach (DependentVariable dependentVariable in dependentVariables) {
            table = AddVariableGeneric(dependentVariable, table);
        }


        Debug.Log($"2: Current table rows in getTable() {table.Rows.Count}");
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

    static DataTable AddVariableGeneric(Variable variable, DataTable table) {
        DataTable newTable = new DataTable();
        switch (variable.DataType) {
            case SupportedDataTypes.Int:
                newTable = AddVariable<int>(table, variable);
                break;
            case SupportedDataTypes.Float:
                newTable = AddVariable<float>(table, variable);
                break;
            case SupportedDataTypes.String:
                newTable = AddVariable<string>(table, variable);
                break;
            case SupportedDataTypes.GameObject:
                newTable = AddVariable<GameObject>(table, variable);
                break;
            case SupportedDataTypes.Vector3:
                newTable = AddVariable<Vector3>(table, variable);
                break;
            case SupportedDataTypes.CustomDataType:
                newTable = AddVariable<CustomMonoBehaviour>(table, variable);
                break;
            case SupportedDataTypes.ChooseType:
                throw new ArgumentException("type not chosen");
            default:
                throw new ArgumentOutOfRangeException();
        }
        //Debug.Log($"table now has {newTable.Rows.Count} rows");
        return newTable;
    }


    static DataTable AddVariable<T>(DataTable table, Variable variable) {

        Debug.Log($"Processing variable {variable.Name} type: {variable.DataType}, varType: {variable.TypeOfVariable}");

        DataTable newTable; 

        if (variable.TypeOfVariable == VariableType.Independent) {
            newTable = table.Clone();
            AddVariableColumn(variable, newTable);
            IndependentVariable<T> independentVariable = (IndependentVariable<T>) variable;
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
             
        }
        else {
            newTable = table.Copy();
            AddVariableColumn(variable, newTable);
        }

        

        //Debug.Log($"(AddVariable<T> table now has {newTable.Rows.Count} rows");
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



    static void AddVariableColumn(Variable variable, DataTable newTable, int index = -1) {
        DataColumn column = new DataColumn {
                                               DataType = variable.Type,
                                               ColumnName = variable.Name,
                                               ReadOnly = false,
                                               Unique = false
                                           };
        newTable.Columns.Add(column);
        if (index >= 0) {
            column.SetOrdinal(index);
        }
    }
}


