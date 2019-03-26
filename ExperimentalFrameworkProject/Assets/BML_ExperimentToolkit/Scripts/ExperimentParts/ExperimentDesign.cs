using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    public partial class ExperimentDesign {

        readonly Experiment experiment;

        public BlockTable OrderedBlockTable;

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

        #region BlockOrderSelection
            void OnEnable() {
                ExperimentEvents.OnBlockOrderChosen += BlockOrderSelected;
            }
            public void Disable() {
                ExperimentEvents.OnBlockOrderChosen -= BlockOrderSelected;
            }
            public void BlockOrderSelected(int selectedOrderIndex) {
                OrderedBlockTable = baseBlockTable.GetBlockOrderTable(selectedOrderIndex);
                CreateAndAddBlocks();
            }
        #endregion



        public DataTable GetBlockOrderTable(int sessionOrderChosenIndex) {
            BlockTable orderedBlockTable = baseBlockTable.GetBlockOrderTable(sessionOrderChosenIndex);
            return orderedBlockTable;
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

            SortedVariableContainer sorted = new SortedVariableContainer(allData, block);

            //Order matters.
            foreach (IndependentVariable independentVariable in sorted.BalancedIndependentVariables) {
                AddVariableColumn(independentVariable, table);
                table = independentVariable.AddValuesTo(table);
            }

            foreach (IndependentVariable independentVariable in sorted.LoopedIndependentVariables) {
                AddVariableColumn(independentVariable, table);
                table = independentVariable.AddValuesTo(table);
            }

            foreach (IndependentVariable independentVariable in sorted.ProbabilityIndependentVariables) {
                AddVariableColumn(independentVariable, table);
                table = independentVariable.AddValuesTo(table);
            }

            foreach (DependentVariable dependentVariable in sorted.DependentVariables) {
                AddVariableColumn(dependentVariable, table);
                table = dependentVariable.AddValuesTo(table);
            }

            return table;
        }

/*
        static DataTable AddVariableGeneric(Variable variable, DataTable table) {
            DataTable newTable;
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
*/


/*
        static DataTable AddVariable<T>(DataTable table, Variable variable) {

            Debug.Log($"Processing variable {variable.Name} type: {variable.DataType}, varType: {variable.TypeOfVariable}");

            DataTable newTable = table.Copy();
            AddVariableColumn(variable, newTable);

            if (variable.TypeOfVariable == VariableType.Independent) {
                
                IndependentVariable<T> independentVariable = (IndependentVariable<T>) variable;
                Debug.Log($"Variable values: {string.Join(", ", independentVariable.Values.ToArray())}");
                switch (independentVariable.MixingTypeOfVariable) {
                    case VariableMixingType.Balanced:
                        newTable = AddBalancedValues<T>(newTable, independentVariable);
                        break;
                    case VariableMixingType.Looped:
                        newTable = AddLoopedValues<T>(newTable, independentVariable);
                        break;
                    case VariableMixingType.EvenProbability:
                        newTable = AddEvenProbabilityValues<T>(newTable, independentVariable);
                        break;
                    case VariableMixingType.CustomProbability:
                        newTable = AddCustomProbabilityValues<T>(newTable, independentVariable);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            else if (variable.TypeOfVariable == VariableType.Dependent) {
                DependentVariable<T> dependentVariable = (DependentVariable<T>) variable;
                newTable = AddDefaultValues<T>(newTable, dependentVariable);
            }
            else {
                throw new ArgumentException("Variable is of undefined type");
            }

            //Debug.Log($"(AddVariable<T> trialTable now has {newTable.Rows.Count} rows");
            return newTable;
        }
*/

/*
        static DataTable AddBalancedValues<T>(DataTable table, IndependentVariable<T> castIndependentVariable) {

            DataTable newTable = table.Clone();
            
            castIndependentVariable.EnsureHasValues();
             
            if (table.Rows.Count == 0) {
                AddRowsToEmptyTable(castIndependentVariable, newTable);
            }
            else {
                //Debug.Log("Adding rows to NON empty trialTable in variable creation");
                foreach (DataRow tableRow in table.Rows) {
                    foreach (T value in castIndependentVariable.Values) {
                        newTable.ImportRow(tableRow);
                        var newRow = newTable.Rows[newTable.Rows.Count - 1];
                        newRow[castIndependentVariable.Name] = value;
                    }
                }
            }

            return newTable;

        }
*/

/*
        static void AddRowsToEmptyTable<T>(IndependentVariable<T> castIndependentVariable,
                                           DataTable  newTable) {
            //Debug.Log("Adding rows to empty trialTable in variable creation");
            foreach (T value in castIndependentVariable.Values) {
                var newRow = newTable.NewRow();
                newRow[castIndependentVariable.Name] = value;
                newTable.Rows.Add(newRow);
            }

        }
*/

/*
        static DataTable AddLoopedValues<T>(DataTable table, IndependentVariable<T> castIndependentVariable) {
            DataTable newTable = table.Clone();

            if (castIndependentVariable.Values.Count == 0) {
                throw new ArgumentNullException($"No values defined for variable {castIndependentVariable.Name}");
            }


            LoopingList<T> loopValues = new LoopingList<T>();
            loopValues.AddRange(castIndependentVariable.Values);


            if (table.Rows.Count == 0) {
                AddRowsToEmptyTable(castIndependentVariable, newTable);
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
                    newTableRow[castIndependentVariable.Name] = value;
                    value = loopValues.NextElement;
                }
            }

            return newTable;
        }
*/

/*
        static DataTable AddEvenProbabilityValues<T>(DataTable table, IndependentVariable<T> castIndependentVariable) {

            DataTable newTable = table.Copy();

            if (castIndependentVariable.Values.Count == 0) {
                throw new ArgumentNullException($"No values defined for variable {castIndependentVariable.Name}");
            }
            
            if (table.Rows.Count == 0) {
                AddRowsToEmptyTable(castIndependentVariable, newTable);
            }
            else {
                Debug.Log($"Adding values to new trialTable (rows: {table.Rows.Count}) in even ProbabilityIndependentVariables variable creation");
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[castIndependentVariable.Name] = castIndependentVariable.Values.RandomItem();
                }
            }

            return newTable;
        }
*/

/*
        static DataTable AddCustomProbabilityValues<T>(DataTable table, IndependentVariable<T> castIndependentVariable) {
            DataTable newTable = table.Copy();
                       
            List<T> distribution = new List<T>();
            for (int i = 0; i < castIndependentVariable.Probabilities.Count; i++) {
                float prob = castIndependentVariable.Probabilities[i];
                T val = castIndependentVariable.Values[i];
                int number = (int) (prob * 1000);
                for (int j = 0; j < number; j++) {
                    distribution.Add(val);
                }
            }

            if (table.Rows.Count == 0) {
                AddRowsToEmptyTable(castIndependentVariable, newTable);
            }
            else {
                Debug.Log($"Adding values to new trialTable (rows: {table.Rows.Count}) in even ProbabilityIndependentVariables variable creation");
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[castIndependentVariable.Name] = distribution.RandomItem();
                }
            }

            return newTable;
        }
*/

/*
        static DataTable AddDefaultValues<T>(DataTable table, DependentVariable<T> dependentVariable) {

            DataTable newTable = table.Copy();
            
            if (table.Rows.Count == 0) {
                throw new ArgumentException("Can't add dependent variable values to empty trialTable");
            }

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[dependentVariable.Name] = dependentVariable.DefaultValue;
            }
            
            return newTable;
        }
*/


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

    
    public abstract class AddIndependentValuesStrategy<T> {

        public abstract DataTable AddValuesToCopyOf(DataTable table, IndependentVariable<T> independentVariable);

        protected DataTable AddRowsToCloneOfEmptyTable<T>(IndependentVariable<T> castIndependentVariable,
                                                          DataTable table) {

            DataTable newTable = table.Clone();
            //Debug.Log("Adding rows to empty trialTable in variable creation");
            foreach (T value in castIndependentVariable.Values) {
                var newRow = newTable.NewRow();
                newRow[castIndependentVariable.Name] = value;
                newTable.Rows.Add(newRow);
            }

            return newTable;
        }
    }

    public class AddDependentValuesStrategy<T> {

        public DataTable AddValuesToCopyOf(DataTable table, DependentVariable<T> dependentVariable) {

            DataTable newTable = table.Copy();

            if (table.Rows.Count == 0) {
                throw new ArgumentException("Can't add dependent variable values to empty trialTable");
            }

            foreach (DataRow newTableRow in newTable.Rows) {
                newTableRow[dependentVariable.Name] = dependentVariable.DefaultValue;
            }

            return newTable;
        }

        
    }


    public class AddBalancedValuesStrategy<T> : AddIndependentValuesStrategy<T> {

        
        public override DataTable AddValuesToCopyOf(DataTable table, IndependentVariable<T> castIndependentVariable) {

            DataTable newTable = table.Clone();
            castIndependentVariable.EnsureHasValues();
            
            if (table.Rows.Count == 0) {
                return AddRowsToCloneOfEmptyTable(castIndependentVariable, newTable);
            }
            else {
                //Debug.Log("Adding rows to NON empty trialTable in variable creation");
                foreach (DataRow tableRow in table.Rows) {
                    foreach (T value in castIndependentVariable.Values) {
                        newTable.ImportRow(tableRow);
                        var newRow = newTable.Rows[newTable.Rows.Count - 1];
                        newRow[castIndependentVariable.Name] = value;
                    }
                }
            }

            return newTable;
        }
    
    }

    public class AddLoopedValuesStrategy<T> : AddIndependentValuesStrategy<T> {

        public override DataTable AddValuesToCopyOf(DataTable table, IndependentVariable<T> castIndependentVariable) {

            DataTable newTable = table.Clone();

            if (castIndependentVariable.Values.Count == 0) {
                throw new ArgumentNullException($"No values defined for variable {castIndependentVariable.Name}");
            }


            LoopingList<T> loopValues = new LoopingList<T>();
            loopValues.AddRange(castIndependentVariable.Values);


            if (table.Rows.Count == 0) {
                AddRowsToCloneOfEmptyTable(castIndependentVariable, newTable);
            }
            else {
                int lowestCommonMultiple =
                    LowestCommonFunctions.LowestCommonMultiple(table.Rows.Count, castIndependentVariable.Values.Count);

                //Make the required number of copies of the trialTable.
                int numberOfTableCopies = lowestCommonMultiple / table.Rows.Count;
                for (int i = 0; i < numberOfTableCopies; i++) {
                    foreach (DataRow tableRow in table.Rows) {
                        newTable.ImportRow(tableRow);
                    }
                }

                T value = loopValues.FirstElement;
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[castIndependentVariable.Name] = value;
                    value = loopValues.NextElement;
                }
            }

            return newTable;
        }
        
    }

    public class AddEvenProbabilityValuesStrategy<T> : AddIndependentValuesStrategy<T> {
        public override DataTable AddValuesToCopyOf(DataTable table, IndependentVariable<T> castIndependentVariable) {

            DataTable newTable = table.Copy();

            if (castIndependentVariable.Values.Count == 0) {
                throw new ArgumentNullException($"No values defined for variable {castIndependentVariable.Name}");
            }

            if (table.Rows.Count == 0) {
                AddRowsToCloneOfEmptyTable(castIndependentVariable, newTable);
            }
            else {
                Debug.Log($"Adding values to new trialTable (rows: {table.Rows.Count}) in even ProbabilityIndependentVariables variable creation");
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[castIndependentVariable.Name] = castIndependentVariable.Values.RandomItem();
                }
            }

            return newTable;
        }
    }

    public class AddCustomProbabilityValuesStrategy<T> : AddIndependentValuesStrategy<T> {
        public override DataTable AddValuesToCopyOf(DataTable table, IndependentVariable<T> castIndependentVariable) {
            DataTable newTable = table.Copy();

            List<T> distribution = new List<T>();
            for (int i = 0; i < castIndependentVariable.Probabilities.Count; i++) {
                float prob = castIndependentVariable.Probabilities[i];
                T val = castIndependentVariable.Values[i];
                int number = (int)(prob * 1000);
                for (int j = 0; j < number; j++) {
                    distribution.Add(val);
                }
            }

            if (table.Rows.Count == 0) {
                AddRowsToCloneOfEmptyTable(castIndependentVariable, newTable);
            }
            else {
                Debug.Log($"Adding values to new trialTable (rows: {table.Rows.Count}) in even ProbabilityIndependentVariables variable creation");
                foreach (DataRow newTableRow in newTable.Rows) {
                    newTableRow[castIndependentVariable.Name] = distribution.RandomItem();
                }
            }

            return newTable;
        }
    }
}