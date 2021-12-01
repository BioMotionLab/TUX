using System;
using System.Data;
using Debug = System.Diagnostics.Debug;


namespace bmlTUX {
    [Serializable]
    public class ColumnNamesSettings {
        public string TotalTrialIndex = "Trial";
        public string BlockIndex      = "Block";
        public string Skipped         = "Skipped";
        public string Attempts        = "Attempts";
        public string TrialIndex      = "TrialInBlock";
        public string Completed       = "Completed";
        public string TrialTime = "TrialTime";
        
        public int DefaultMissingValue = -999;
        
        public DataColumn GetColumnWithName(string columnName) {
            Debug.Assert(TotalTrialIndex != null, nameof(TotalTrialIndex) + " != null");

            if (columnName == TotalTrialIndex) return new DataColumn(TotalTrialIndex, typeof(int));
            if (columnName == BlockIndex) return new DataColumn(BlockIndex, typeof(int));
            if (columnName == Skipped) return new DataColumn(Skipped, typeof(bool));
            if (columnName == Attempts) return new DataColumn(Attempts, typeof(int));
            if (columnName == TrialIndex) return new DataColumn(TrialIndex, typeof(int));
            if (columnName == Completed) return new DataColumn(Completed, typeof(bool));
            if (columnName == TrialTime) return new DataColumn(TrialTime, typeof(float));
            
            throw new ArgumentException($"column name: {columnName} > not defined");
        }
    }
}