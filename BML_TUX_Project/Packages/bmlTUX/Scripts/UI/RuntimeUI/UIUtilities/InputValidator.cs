using System.Collections.Generic;

namespace bmlTUX.Scripts.UI.RuntimeUI.UIUtilities {
    public interface InputValidator {
        List<string> Errors { get; }
        bool         Valid  { get; }
    }
}