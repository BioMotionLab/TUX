using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor
{
    public class ExperimentTableTests
    {
        public class AddVariableMethod {

            DataTable testTable;
            DatumInt testIntDatum;

            [SetUp]
            public void SetUp() {
                string intVariable = "intValue";
                testTable = new DataTable();
                DataColumn column = new DataColumn {
                                                       DataType = typeof(int),
                                                       ColumnName = intVariable,
                                                       ReadOnly = false,
                                                       Unique = false
                                                   };
                testTable.Columns.Add(column);

                var fact = new DatumFactory();
                var intList = new List<int> {
                                                1,2,3,
                                            };
                testIntDatum = fact.NewInt(intList);
            }

            // A Test behaves as an ordinary method
            [Test]
            public void AddsAColumn() {
                DataTable addedTable = ExperiementTable.AddVariable(testTable, testIntDatum);
                Assert.True(addedTable.Columns.Count == 2);
            }

            [Test]
            public void AddsCorrectNumberOfRowsWhenEmpty() {
                DataTable addedTable = ExperiementTable.AddVariable(testTable, testIntDatum);
                int expected = 3;
                Assert.AreEqual(expected, addedTable.Rows.Count, $"datum values: {testIntDatum.Values.Count}");

            }

            [Test]
            public void AddsCorrectNumberOfRowsWhenAlready1Row() {
                var newRow = testTable.NewRow();
                testTable.Rows.Add(newRow);

                DataTable addedTable = ExperiementTable.AddVariable(testTable, testIntDatum);
                
                int expected = 3;
                Assert.AreEqual(expected, addedTable.Rows.Count, $"datum values: {testIntDatum.Values.Count}");

            }
            [Test]
            public void AddsCorrectNumberOfRowsWhenAlreadySeveralRows() {
                var newRow = testTable.NewRow();
                testTable.Rows.Add(newRow);
                var newRow2 = testTable.NewRow();
                testTable.Rows.Add(newRow2);
                var newRow3 = testTable.NewRow();
                testTable.Rows.Add(newRow3);

                DataTable addedTable = ExperiementTable.AddVariable(testTable, testIntDatum);

                const int expected = 9;
                UnityEngine.Debug.Log(addedTable);
                Assert.AreEqual(expected, addedTable.Rows.Count, $"datum values: {testIntDatum.Values.Count}");

            }
            [Test]
            public void ThirdVariableAddsCorrectNumberOfRowsWhenAlreadySeveralRows() {
                DataTable addedTable = ExperiementTable.AddVariable(testTable, testIntDatum);
                testIntDatum.Name = "second";
                addedTable = ExperiementTable.AddVariable(addedTable, testIntDatum);
                testIntDatum.Name = "third";
                addedTable = ExperiementTable.AddVariable(addedTable, testIntDatum);
                const int expected = 27;
                Assert.AreEqual(expected, addedTable.Rows.Count, $"datum values: {testIntDatum.Values.Count}");
            }

        }
        

        
    }
}
