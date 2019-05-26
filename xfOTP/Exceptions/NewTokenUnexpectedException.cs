using System;
using System.Runtime.Serialization;

namespace xfOTP.Exceptions
{
    [Serializable]
    internal class NewTokenUnexpectedException : Exception
    {
        private Exception e;

        public NewTokenUnexpectedException()
        {
        }

        public NewTokenUnexpectedException(Exception e)
        {
            this.e = e;
        }

        public NewTokenUnexpectedException(string message) : base(message)
        {
        }

        public NewTokenUnexpectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NewTokenUnexpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}