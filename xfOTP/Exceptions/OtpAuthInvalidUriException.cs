using System;
using System.Runtime.Serialization;

namespace xfOTP.Exceptions
{
    [Serializable]
    internal class OtpAuthInvalidUriException : Exception
    {
        private Exception e;

        public OtpAuthInvalidUriException()
        {
        }

        public OtpAuthInvalidUriException(Exception e)
        {
            this.e = e;
        }

        public OtpAuthInvalidUriException(string message) : base(message)
        {
        }

        public OtpAuthInvalidUriException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OtpAuthInvalidUriException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}