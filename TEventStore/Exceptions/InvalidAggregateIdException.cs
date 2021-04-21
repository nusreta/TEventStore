using System;

namespace TEventStore.Exceptions
{
    public sealed class InvalidAggregateIdException : Exception
    {
        public InvalidAggregateIdException(string message) : base(message)
        {
            
        }
    }
}
