namespace bmlTUX.Scripts.Utilities.Extensions {
    public class DataFormatter {

        string unFormattedString;
        public DataFormatter(string unFormattedString) {
            this.unFormattedString = unFormattedString;
        }

        public string Formatted {
            get {
                string formattedString = unFormattedString.Replace(",", "_");
                //Debug.Log($"formatted {formattedString}");
                return formattedString;
            }
        }
    }
}