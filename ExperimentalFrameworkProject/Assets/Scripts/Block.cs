using System.Collections.Generic;
using System.Data;

public class Block {
    private const string TabSeparator = "\t";
    private const int TruncateDefault = 10;

    public DataTable Table;
    public string Identity;

    public bool Complete = false;
    public int Index = -1;

    public List<Trial> Trials;

    public Block(DataTable table, string identity) {
        this.Table = table;
        this.Identity = identity;
        MakeTrials();
    }

    void MakeTrials() {

        Trials = new List<Trial>();

        int i = 1;
        //configure block index
        foreach (DataRow row in Table.Rows) {
            Trial newTrial = new TestTrial(row);
            Trials.Add(newTrial);
            i++;
        }
    }

    

    public string AsString(string separator = TabSeparator, int truncate = TruncateDefault) {
        string tableString = Table.AsString();
        return "Identity: " + Identity + "\n" + tableString;
    }

    



}