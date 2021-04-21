using System;

namespace TEventStore.Exceptions
{
    public sealed class InvalidEventRecordException : Exception
    {
        public InvalidEventRecordException(string message) : base(message)
        {
            
        }
    }
}
