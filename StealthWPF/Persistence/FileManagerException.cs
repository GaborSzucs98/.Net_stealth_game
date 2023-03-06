using System;
using System.IO;

namespace StealthWPF.Persistence
{

	[Serializable]
	public class FileManagerException : IOException
	{
		public FileManagerException() { }
		public FileManagerException(string message) : base(message) { }
		public FileManagerException(string message, Exception inner) : base(message, inner) { }
		protected FileManagerException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
