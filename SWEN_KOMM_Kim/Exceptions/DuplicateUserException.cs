﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Exceptions
{
    [Serializable]
    internal class DuplicateUserException : Exception
    {
        public DuplicateUserException()
        {
        }

        public DuplicateUserException(string? message) : base(message)
        {
        }

        public DuplicateUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DuplicateUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
