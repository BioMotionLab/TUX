using System;
using System.Data;

internal class SimpleBlock : Block {
    public SimpleBlock(DataTable trialTable, string identity, Type CustomTrialType) : base(trialTable, identity, CustomTrialType) { }
}