using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Exceptions
{
    [Serializable]
    internal class NoContentException : Exception
    {
        public NoContentException()
        {
        }

        public NoContentException(string? message) : base(message)
        {
        }

        public NoContentException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoContentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
