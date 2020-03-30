namespace Arcadia.Assistant.Permissions.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NotEnoughPermissionsException : ApplicationException
    {
        public NotEnoughPermissionsException()
        {
        }

        public NotEnoughPermissionsException(string message) : base(message)
        {
        }

        public NotEnoughPermissionsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NotEnoughPermissionsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}