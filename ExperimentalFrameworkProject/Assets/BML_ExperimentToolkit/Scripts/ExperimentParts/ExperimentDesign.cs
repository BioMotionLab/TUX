using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using MyNamespace;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public class ExperimentDesign {

        Experiment experiment;

        DataTable baseBlockTable;
        DataTable baseTrialTable;

        public DataTable   OrderedBlockTable;
        public List<Block> Blocks;

        public ExperimentDesign(Experiment experiment, List<Variable> allData, bool shuffleTrialOrder,
                                int        numberOfRepetitions) {
            this.experiment = experiment;
            baseBlockTable = CreateBlockTable(allData);
            baseTrialTable = CreateBaseTrialTable(allData, shuffleTrialOrder, numberOfRepetitions);
            OnEnable();
        }

        void OnEnable() {
            ExperimentEvents.OnBlockOrderChosen += BlockOrderSelected;
        }

        public void Disable() {
            ExperimentEvents.OnBlockOrderChosen -= BlockOrderSelected;

        }

        static DataTable CreateBaseTrialTable(List<Variable> allData, bool shuffleTrialOrder, int numberOfRepetitions) {
            DataTable baseTrialTable = new DataTable();

            baseTrialTable = SortAndAddIVs(allData, baseTrialTable);

            //Repeat all trials if specified
            if (numberOfRepetitions > 1) {
                DataTable repeatedTable = baseTrialTable.Clone();
                for (int i = 0; i < numberOfRepetitions; i++) {
                    foreach (DataRow row in baseTrialTable.Rows) {
                        repeatedTable.ImportRow(row);
                    }
                }

                baseTrialTable = repeatedTable;
            }

            //Shuffle trial order if needed
            if (shuffleTrialOrder) {
                baseTrialTable = baseTrialTable.Shuffle();
            }


            //Add total trial number column
            AddTotalTrialIndexColumnTo(baseTrialTable);

            //Add trial in block number column
            AddTrialIndexColumnTo(baseTrialTable);

            //Add block number column
            AddBlockNumberColumnTo(baseTrialTable);

            //Add Successfully run column
            AddSuccessColumnTo(baseTrialTable);

            //Add Attempts column
            AddAttemptsColumnTo(baseTrialTable);

            //Add skipped column
            AddSkippedColumnTo(baseTrialTable);
            return baseTrialTable;
        }

        static DataTable CreateBlockTable(List<Variable> allData) {
            DataTable blockTable = new DataTable();

            //Get block Variables
            List<Variable> blockVariables = new List<Variable>();
            foreach (Variable datum in allData) {
                if (datum.TypeOfVariable == VariableType.Independent) {
                    IndependentVariable IvDatum = (IndependentVariable) datum;
                    if (IvDatum.Block) {
                        blockVariables.Add(IvDatum);
                    }
                }
            }

            blockTable = SortAndAddIVs(blockVariables, blockTable, true);
            return blockTable;
        }

        public List<string> BlockPermutationsStrings {
            get {
                List<string> blockPermutations = new List<string>();
                int blockOrderIndex = 0;
                foreach (List<DataRow> dataRows in baseBlockTable.GetPermutations()) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Order #{blockOrderIndex}:   ");
                    foreach (DataRow dataRow in dataRows) {
                        sb.Append($"{dataRow.AsString(separator: ", ", truncate: -1)} >   ");
                    }

                    blockPermutations.Add(sb.ToString());
                    blockOrderIndex++;
                }

                return blockPermutations;
            }
        }

        public object TotalTrials      => Blocks.Count * baseTrialTable.Rows.Count;
        public int    BlockCount       => Blocks.Count;
        public string TrialTableHeader => Blocks[0].trialTable.HeaderAsString(separator: Delimiter.Comma, truncate: -1);

        public DataTable GetBlockOrderTable(int index) {
            DataTable orderedTable = baseBlockTable.Clone();
            foreach (DataRow dataRow in baseBlockTable.GetPermutations()[index]) {
                orderedTable.ImportRow(dataRow);
            }

            return orderedTable;
        }

        public void BlockOrderSelected(int selectedOrderIndex) {
            OrderedBlockTable = GetBlockOrderTable(selectedOrderIndex);
            CreateAndAddBlocks();
        }

        public void CreateAndAddBlocks() {
            Blocks = new List<Block>();

            baseTrialTable = AddBlockColumns(baseTrialTable, baseBlockTable);


            for (int i = 0; i < OrderedBlockTable.Rows.Count; i++) {
                DataRow orderedBlockRow = OrderedBlockTable.Rows[i];



                DataTable trialTable = baseTrialTable.Copy();
                trialTable = UpdateWithBlockValues(trialTable, orderedBlockRow, i);

                string blockIdentity = orderedBlockRow.AsString(separator: ", ");
                Block newBlock = (Block) Activator.CreateInstance(experiment.BlockType, orderedBlockRow, trialTable,
                                                                  blockIdentity, experiment.TrialType);
                Blocks.Add(newBlock);

                //Debug.Log($"{newBlock.AsString()}");
            }

            Debug.Log($"Blocks added {Blocks.Count}");
        }

        static DataTable UpdateWithBlockValues(DataTable blockTrialTable, DataRow blockTableRow, int blockIndex) {
            DataTable newTable = blockTrialTable.Copy();

            foreach (DataColumn blockTableColumn in blockTableRow.Table.Columns) {
                string columnName = blockTableColumn.ColumnName;
                int startingTotalTrialIndex = blockIndex * newTable.Rows.Count;
                for (int trialIndexInBlock = 0; trialIndexInBlock < newTable.Rows.Count; trialIndexInBlock++) {
                    DataRow trialRow = newTable.Rows[trialIndexInBlock];
                    trialRow[columnName] = blockTableRow[columnName];
                    trialRow[ExperimentConfig.BlockIndexColumnName] = blockIndex;
                    trialRow[ExperimentConfig.TrialIndexColumnName] = trialIndexInBlock;
                    trialRow[ExperimentConfig.TotalTrialIndexColumnName] = startingTotalTrialIndex;
                    startingTotalTrialIndex++;
                }
            }

            return newTable;
        }

        static DataTable AddBlockColumns(DataTable trialTable, DataTable blockTable) {
            DataTable newTrialTable = trialTable.Copy();

            foreach (DataColumn blockTableColumn in blockTable.Columns) {
                newTrialTable = newTrialTable.AddColumnFromOtherTable(blockTableColumn, 0);
            }

            return newTrialTable;
        }



        static DataTable SortAndAddIVs(List<Variable> allData, DataTable table, bool block = false) {
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

            //Debug.Log($"1: Current trialTable rows in getTable() {trialTable.Rows.Count}");

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


            //Debug.Log($"2: Current trialTable rows in getTable() {trialTable.Rows.Count}");
            return table;
        }

        static void AddSkippedColumnTo(DataTable table) {
            DataColumn skippedColumn = new DataColumn {
                                                          DataType = typeof(bool),
                                                          ColumnName = ExperimentConfig.SkippedColumnName,
                                                          Unique = false,
                                                          ReadOnly = false,
                                                      };
            table.Columns.Add(skippedColumn);
            foreach (DataRow row in table.Rows) {
                row[ExperimentConfig.SkippedColumnName] = false;
            }
        }

        static void AddAttemptsColumnTo(DataTable table) {
            DataColumn attemptsColumn = new DataColumn {
                                                           DataType = typeof(int),
                                                           ColumnName = ExperimentConfig.AttemptsColumnName,
                                                           Unique = false,
                                                           ReadOnly = false,
                                                       };
            table.Columns.Add(attemptsColumn);
            foreach (DataRow row in table.Rows) {
                row[ExperimentConfig.AttemptsColumnName] = 0;
            }
        }

        static void AddSuccessColumnTo(DataTable table) {
            DataColumn successColumn = new DataColumn {
                                                          DataType = typeof(bool),
                                                          ColumnName = ExperimentConfig.SuccessColumnName,
                                                          Unique = false,
                                                          ReadOnly = false,
                                                      };
            table.Columns.Add(successColumn);
            foreach (DataRow row in table.Rows) {
                row[ExperimentConfig.SuccessColumnName] = false;
            }
        }

        static void AddTrialIndexColumnTo(DataTable table) {
            DataColumn trialIndexColumn = new DataColumn {
                                                             DataType = typeof(int),
                                                             ColumnName = ExperimentConfig.TrialIndexColumnName,
                                                             Unique = false,
                                                             ReadOnly = false,
                                                         };
            table.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in table.Rows) {
                row[ExperimentConfig.TrialIndexColumnName] = -1;
            }
        }

        static void AddTotalTrialIndexColumnTo(DataTable table) {
            DataColumn trialIndexColumn = new DataColumn {
                                                             DataType = typeof(int),
                                                             ColumnName = ExperimentConfig.TotalTrialIndexColumnName,
                                                             Unique = false,
                                                             ReadOnly = false,
                                                         };
            table.Columns.Add(trialIndexColumn);
            trialIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in table.Rows) {
                row[ExperimentConfig.TotalTrialIndexColumnName] = -1;
            }
        }

        static void AddBlockNumberColumnTo(DataTable table) {
            DataColumn blockIndexColumn = new DataColumn {
                                                             DataType = typeof(int),
                                                             ColumnName = ExperimentConfig.BlockIndexColumnName,
                                                             Unique = false,
                                                             ReadOnly = false,
                                                         };
            table.Columns.Add(blockIndexColumn);
            blockIndexColumn.SetOrdinal(0); // to put the column in position 0;
            foreach (DataRow row in table.Rows) {
                row[ExperimentConfig.BlockIndexColumnName] = -1;
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
                    newTable = AddVariable<CustomSupportedDataType>(table, variable);
                    break;
                case SupportedDataTypes.ChooseType:
                    throw new ArgumentException("type not chosen");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Debug.Log($"trialTable now has {newTable.Rows.Count} rows");
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
            else if (variable.TypeOfVariable == VariableType.Dependent) {
                DependentVariable<T> dependentVariable = (DependentVariable<T>) variable;
                newTable = table.Copy();
                newTable = AddDefaultValues<T>(newTable, dependentVariable);
            }
            else {
                throw new ArgumentException("Variable is of undefined type of variable");
            }



            //Debug.Log($"(AddVariable<T> trialTable now has {newTable.Rows.Count} rows");
            return newTable;
        }




        static DataTable AddBalancedValues<T>(DataTable table, IndependentVariable independentVariable) {

            DataTable newTable = table.Clone();

            AddVariableColumn(independentVariable, newTable);

            IndependentVariable<T> castIndependentVariable = (IndependentVariable<T>) independentVariable;
            if (castIndependentVariable.Values.Count == 0) {
                throw new ArgumentException($"No values defined for variable {independentVariable.Name}");
            }


            if (table.Rows.Count == 0) {
                //Debug.Log("Adding rows to empty trialTable in variable creation");

                foreach (T value in castIndependentVariable.Values) {
                    var newRow = newTable.NewRow();
                    newRow[independentVariable.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                //Debug.Log("Adding rows to NON empty trialTable in variable creation");
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

            IndependentVariable<T> castIndependentVariable = (IndependentVariable<T>) independentVariable;
            if (castIndependentVariable.Values.Count == 0) {
                throw new ArgumentNullException($"No values defined for variable {independentVariable.Name}");
            }


            LoopingList<T> loopValues = new LoopingList<T>();
            loopValues.AddRange(castIndependentVariable.Values);


            if (table.Rows.Count == 0) {
                Debug.Log("Adding rows to empty trialTable in variable creation");
                foreach (T value in castIndependentVariable.Values) {
                    var newRow = newTable.NewRow();
                    newRow[independentVariable.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                int lowestCommonMultiple =
                    LowestCommonFunctions.LowestCommonMultiple(table.Rows.Count, castIndependentVariable.Values.Count);

                //Make the required number of copies of the trialTable.
                int numberOfTableCopies = lowestCommonMultiple / table.Rows.Count;
                Debug.Log($"Number of trialTable copies: {numberOfTableCopies}");
                for (int i = 0; i < numberOfTableCopies; i++) {
                    Debug.Log($"Adding {i}th copy of trialTable");
                    foreach (DataRow tableRow in table.Rows) {
                        newTable.ImportRow(tableRow);
                    }
                }

                Debug.Log("Adding rows to NON empty trialTable in looped variable creation");
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
                Debug.Log("Adding rows to empty trialTable in variable creation");
                foreach (T value in castIndependentVariable.Values) {
                    var newRow = newTable.NewRow();
                    newRow[independentVariable.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                Debug.Log($"Adding values to new trialTable (rows: {table.Rows.Count}) in even probability variable creation");
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[independentVariable.Name] = castIndependentVariable.Values.RandomItem();
                }
            }

            return newTable;
        }

        static DataTable AddCustomProbabilityValues<T>(DataTable table, IndependentVariable independentVariable) {
            DataTable newTable = table.Copy();


            AddVariableColumn(independentVariable, newTable);


            IndependentVariable<T> castedIndependentVariable = (IndependentVariable<T>) independentVariable;

            List<T> distribution = new List<T>();
            for (int i = 0; i < castedIndependentVariable.Probabilities.Count; i++) {
                float prob = castedIndependentVariable.Probabilities[i];
                T val = castedIndependentVariable.Values[i];
                int number = (int) (prob * 1000);
                for (int j = 0; j < number; j++) {
                    distribution.Add(val);
                }
            }

            if (table.Rows.Count == 0) {
                Debug.Log("Adding rows to empty trialTable in variable creation");
                foreach (T value in castedIndependentVariable.Values) {
                    var newRow = newTable.NewRow();
                    newRow[independentVariable.Name] = value;
                    newTable.Rows.Add(newRow);
                }
            }
            else {
                Debug.Log($"Adding values to new trialTable (rows: {table.Rows.Count}) in even probability variable creation");
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[independentVariable.Name] = distribution.RandomItem();
                }
            }

            return newTable;
        }

        static DataTable AddDefaultValues<T>(DataTable table, DependentVariable<T> dependentVariable) {

            DataTable newTable = table.Copy();

            AddVariableColumn(dependentVariable, newTable);


            if (table.Rows.Count == 0) {
                throw new ArgumentException("Can't add dependent variable values to empty trialTable");
            }

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[dependentVariable.Name] = dependentVariable.DefaultValue;
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


}