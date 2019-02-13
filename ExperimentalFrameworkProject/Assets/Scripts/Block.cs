using System;
using System.Collections.Generic;
using System.Data;

public abstract class Block {
    private const string TabSeparator = "\t";
    private const int TruncateDefault = 10;

    public DataTable trialTable;
    public string Identity;

    public bool Complete = false;
    public int Index = -1;

    public List<Trial> Trials;

    public Block(DataTable trialTable, string identity, Type trialType) {
        this.trialTable = trialTable;
        this.Identity = identity;
        MakeTrials(trialType);
    }

    void MakeTrials(Type trialType) {

        Trials = new List<Trial>();

        int i = 1;
        //configure block index
        foreach (DataRow row in trialTable.Rows) {
            Trial newTrial = (Trial)Activator.CreateInstance(trialType, row);
            Trials.Add(newTrial);
            i++;
        }
    }

    public string AsString(string separator = TabSeparator, int truncate = TruncateDefault) {
        string tableString = trialTable.AsString();
        return "Identity: " + Identity + "\n" + tableString;
    }

}