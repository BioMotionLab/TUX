using System;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using MyNamespace;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public partial class ExperimentDesign {

        readonly Experiment experiment;

        public BlockTable   OrderedBlockTable;
        public List<Block> Blocks;
        readonly BlockTable baseBlockTable;
        readonly TrialTable baseTrialTable;

        public List<string> BlockPermutationsStrings => baseBlockTable.BlockPermutationsStrings;
    
        public int TotalTrials      => Blocks.Count * baseTrialTable.Trials;
        public int    BlockCount       => Blocks.Count;
        public string TrialTableHeader => Blocks[0].trialTable.HeaderAsString(separator: Delimiter.Comma, truncate: -1);

        public bool HasBlocks => baseBlockTable.HasBlocks;

        readonly bool shuffleTrialsBetweenBlocks;

        public ExperimentDesign(Experiment experiment, List<Variable> allData, bool shuffleTrialOrder,
                                int        numberOfRepetitions, bool shuffleTrialsBetweenBlocks) {
            this.experiment = experiment;
            this.shuffleTrialsBetweenBlocks = shuffleTrialsBetweenBlocks;
            baseBlockTable = new BlockTable(allData);
            baseTrialTable = new TrialTable(allData, baseBlockTable, shuffleTrialOrder, numberOfRepetitions, experiment.ColumnNames);
            OnEnable();
        }

        void OnEnable() {
            ExperimentEvents.OnBlockOrderChosen += BlockOrderSelected;
        }

        public void Disable() {
            ExperimentEvents.OnBlockOrderChosen -= BlockOrderSelected;
        }

        public DataTable GetBlockOrderTable(int sessionOrderChosenIndex) {
            BlockTable orderedBlockTable = baseBlockTable.GetBlockOrderTable(sessionOrderChosenIndex);
            return orderedBlockTable;
        }

        public void BlockOrderSelected(int selectedOrderIndex) {
            OrderedBlockTable = baseBlockTable.GetBlockOrderTable(selectedOrderIndex);
            CreateAndAddBlocks();
        }


        public void CreateAndAddBlocks() {
            Blocks = new List<Block>();
            
            if (OrderedBlockTable == null) {
                Debug.Log("No Block Variables");
                DataTable trialTable = baseTrialTable.Copy();
                for (int i = 0; i < trialTable.Rows.Count; i++) {
                    DataRow trialRow = trialTable.Rows[i];
                    trialRow[experiment.ColumnNames.BlockIndex] = 0;
                    trialRow[experiment.ColumnNames.TrialIndex] = i;
                    trialRow[experiment.ColumnNames.TotalTrialIndex] = i;
                }

                string blockIdentity = "Main Block";
                Block newBlock = (Block) Activator.CreateInstance(experiment.BlockType, 
                                                                  experiment, 
                                                                  trialTable,
                                                                  blockIdentity, 
                                                                  experiment.TrialType 
                                                                  );
                Blocks.Add(newBlock);

            }
            else {
                for (int i = 0; i < OrderedBlockTable.Rows.Count; i++) {
                    DataRow orderedBlockRow = OrderedBlockTable.Rows[i];
                    
                    DataTable trialTable = baseTrialTable.Copy();
                    if (shuffleTrialsBetweenBlocks) {
                        trialTable = trialTable.Shuffle();
                    }
                    trialTable = UpdateWithBlockValues(trialTable, orderedBlockRow, i);

                    string blockIdentity = orderedBlockRow.AsStringWithColumnNames(separator: ", ");
                    Block newBlock = (Block) Activator.CreateInstance(experiment.BlockType, experiment, trialTable,
                                                                      blockIdentity, experiment.TrialType);
                    Blocks.Add(newBlock);

                    //Debug.Log($"{newBlock.AsString()}");
                }
            }

            //Debug.Log($"Blocks added {Blocks.Count}");
        }

        DataTable UpdateWithBlockValues(DataTable blockTrialTable, DataRow blockTableRow, int blockIndex) {
            DataTable newTable = blockTrialTable.Copy();

            foreach (DataColumn blockTableColumn in blockTableRow.Table.Columns) {
                string columnName = blockTableColumn.ColumnName;
                int startingTotalTrialIndex = blockIndex * newTable.Rows.Count;
                for (int trialIndexInBlock = 0; trialIndexInBlock < newTable.Rows.Count; trialIndexInBlock++) {
                    DataRow trialRow = newTable.Rows[trialIndexInBlock];
                    trialRow[columnName] = blockTableRow[columnName];
                    trialRow[experiment.ColumnNames.BlockIndex] = blockIndex;
                    trialRow[experiment.ColumnNames.TrialIndex] = trialIndexInBlock;
                    trialRow[experiment.ColumnNames.TotalTrialIndex] = startingTotalTrialIndex;
                    startingTotalTrialIndex++;
                }
            }

            return newTable;
        }

        
        public static DataTable SortAndAddIVs(List<Variable> allData, bool block = false) {
            DataTable table = new DataTable();

            List<IndependentVariable> balanced = new List<IndependentVariable>();
            List<IndependentVariable> looped = new List<IndependentVariable>();
            List<IndependentVariable> probability = new List<IndependentVariable>();

            List<DependentVariable> dependentVariables = new List<DependentVariable>();

            //Sort Independent variables into mixing categories so they go in order
            int numberOfBlockIvs = 0;
            int numberOfNonBlockIvs = 0;
            foreach (Variable variable in allData) {
                if (variable.TypeOfVariable == VariableType.Independent) {
                    IndependentVariable ivVariable = (IndependentVariable) variable;

                    if (ivVariable.Block) {
                        numberOfBlockIvs++;
                    }
                    else {
                        numberOfNonBlockIvs++;
                    }
                    

                    if (block && ivVariable.Block || !block && !ivVariable.Block) {

                        switch (ivVariable.MixingTypeOfVariable) {
                            case VariableMixingType.Balanced:
                                balanced.Add(ivVariable);
                                break;
                            case VariableMixingType.Looped:
                                looped.Add(ivVariable);
                                break;

                            case VariableMixingType.EvenProbability:
                            case VariableMixingType.CustomProbability:
                                probability.Add(ivVariable);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    
                }
                else if (variable.TypeOfVariable == VariableType.Dependent) {
                    DependentVariable dVDatum = (DependentVariable) variable;
                    dependentVariables.Add(dVDatum);
                }
            }

            bool thereAreBlockIvsButNoNormalIvs = numberOfBlockIvs > 0 && numberOfNonBlockIvs == 0;
            if (!block && thereAreBlockIvsButNoNormalIvs) {

                throw new InvalidExperimentDesignException($"You defined {numberOfBlockIvs} block variable(s), " +
                                                           $"when there are {numberOfNonBlockIvs} unblocked independent variables." +
                                                           $"You can safely unblock the variable " +
                                                           $"to make it a normal variable");
            }


            //Order they're added matters.
            foreach (IndependentVariable datum in balanced) table = AddVariableGeneric(datum, table);
            foreach (IndependentVariable datum in looped) table = AddVariableGeneric(datum, table);
            foreach (IndependentVariable datum in probability) table = AddVariableGeneric(datum, table);
            foreach (DependentVariable dependentVariable in dependentVariables) table = AddVariableGeneric(dependentVariable, table);
            
            //Debug.Log($"2: Current trialTable rows in getTable() {trialTable.Rows.Count}");
            return table;
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
                case SupportedDataTypes.Bool:
                    newTable = AddVariable<bool>(table, variable);
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
                Debug.Log($"Variable values: {string.Join(", ", independentVariable.Values.ToArray())}");
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
                //Debug.Log($"Number of trialTable copies: {numberOfTableCopies}");
                for (int i = 0; i < numberOfTableCopies; i++) {
                    //Debug.Log($"Adding {i}th copy of trialTable");
                    foreach (DataRow tableRow in table.Rows) {
                        newTable.ImportRow(tableRow);
                    }
                }

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