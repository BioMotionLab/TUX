namespace bmlTUX {
    public class DataFormatter {

        string unFormattedString;
        public DataFormatter(string unFormattedString) {
            this.unFormattedString = unFormattedString;
        }

        public string Formatted {
            get {
                string formattedString = unFormattedString.Replace(",", "_");
                return formattedString;
            }
        }
    }
}