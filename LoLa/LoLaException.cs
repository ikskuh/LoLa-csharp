using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoLa
{
    /// <summary>
    /// An exception that is thrown when executing LoLa-Code and a runtime error happens.
    /// </summary>
    [Serializable]
    public class LoLaException : Exception
    {
        public LoLaException() { }
        public LoLaException(string message) : base(message) { }
        public LoLaException(string message, Exception inner) : base(message, inner) { }
        protected LoLaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
