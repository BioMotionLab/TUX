using System;

namespace bmlTUX.Scripts.Managers {
	public class CallingInvalidFileException : Exception {
		public CallingInvalidFileException(string message) :base(message) { }
	}
}