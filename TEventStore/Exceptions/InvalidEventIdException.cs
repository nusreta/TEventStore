using System;

namespace TEventStore.Exceptions
{
    public sealed class InvalidEventIdException : Exception
    {
        public InvalidEventIdException(string message) : base(message)
        {
            
        }
    }
}
