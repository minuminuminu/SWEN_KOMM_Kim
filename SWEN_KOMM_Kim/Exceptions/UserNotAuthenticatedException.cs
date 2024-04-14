using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Exceptions
{
    [Serializable]
    internal class UserNotAuthenticatedException : Exception
    {
        public UserNotAuthenticatedException()
        {
        }

        public UserNotAuthenticatedException(string? message) : base(message)
        {
        }

        public UserNotAuthenticatedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserNotAuthenticatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
