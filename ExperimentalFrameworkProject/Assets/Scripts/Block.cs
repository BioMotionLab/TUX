using System.Collections.Generic;
using System.Data;

public class Block {
    private const string TabSeparator = "\t";
    private const int TruncateDefault = 10;

    public DataTable table;
    public string Identity;

    public bool Complete = false;
    public int Index = -1;

    public List<Trial> Trials;

    public Block(DataTable table) {
        this.table = table;
        MakeTrials();
    }

    void MakeTrials() {

        Trials = new List<Trial>();

        int i = 1;
        //configure block index
        foreach (DataRow row in table.Rows) {
            row[Config.TrialIndexColumnName] = i;
            row[Config.BlockIndexColumnName] = Index;
            Trial newTrial = new TestTrial(row);
            Trials.Add(newTrial);
            i++;
        }
    }

    

    public string AsString(string separator = TabSeparator, int truncate = TruncateDefault) {
        string tableString = table.AsString();
        return "Identity: " + Identity + "\n" + tableString;
    }

    



}