using System;
using System.Data;

public class SimpleBlock : Block {
    public SimpleBlock(DataRow row, DataTable trialTable, string identity, Type CustomTrialType) : base(row, trialTable, identity, CustomTrialType) { }
}