using System;
using System.Runtime.Serialization;

namespace Services.Common.ExceptionsModels
{
    /// <summary>
    /// Base exception type for those are thrown by FW system for FW specific exceptions.
    /// </summary>
    [Serializable]
    public class CoreException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="CoreException"/> object.
        /// </summary>
        public CoreException()
        {
        }

        /// <summary>
        /// Creates a new <see cref="CoreException"/> object.
        /// </summary>
        public CoreException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
        }

        /// <summary>
        /// Creates a new <see cref="CoreException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public CoreException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="CoreException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public CoreException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}