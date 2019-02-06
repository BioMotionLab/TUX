using System.Data;

public class Block {
    private const string TabSeparator = "\t";
    private const int TruncateDefault = 10;

    public DataTable table;
    public string Identity;

    public Block(DataTable table) {
        this.table = table;
    }

    public string AsString(string separator = TabSeparator, int truncate = TruncateDefault) {
        string tableString = table.AsString();
        return "Identity: " + Identity + "\n" + tableString;
    }

}