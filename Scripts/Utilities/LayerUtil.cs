using System.Linq;

namespace BML_Utilities {
    public static class LayerUtil
    {
        const int NumberOfLayers = 32;
        
        public static int[] AllLayers => Enumerable.Range(0, NumberOfLayers).ToArray();
        
    }
}
