using System;

namespace bmlTUX {
	public class CallingInvalidFileException : Exception {
		public CallingInvalidFileException(string message) :base(message) { }
	}
}