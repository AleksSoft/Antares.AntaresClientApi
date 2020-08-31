using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace AntaresClientApi.GrpcServices.Authentication
{
    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException()
        {
        }

        protected UnAuthorizedException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UnAuthorizedException(string message) : base(message)
        {
        }

        public UnAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
