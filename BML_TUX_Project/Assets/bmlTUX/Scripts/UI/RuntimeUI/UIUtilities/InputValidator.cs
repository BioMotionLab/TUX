using System.Collections.Generic;

namespace bmlTUX.Scripts.UI.Runtime {
    public interface InputValidator {
        List<string> Errors { get; }
        bool         Valid  { get; }
    }
}