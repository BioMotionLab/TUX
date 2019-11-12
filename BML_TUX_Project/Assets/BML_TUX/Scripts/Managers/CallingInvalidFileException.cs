using System;

namespace BML_TUX.Scripts.Managers {
	public class CallingInvalidFileException : Exception {
		public CallingInvalidFileException(string message) :base(message) { }
	}
}