using System;

namespace TEventStore.Exceptions
{
    public sealed class InvalidAggregateRecordException : Exception
    {
        public InvalidAggregateRecordException(string message) : base(message)
        {
            
        }
    }
}
