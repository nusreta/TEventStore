using System;

namespace TEventStore.Exceptions
{
    public sealed class InvalidSequenceException : Exception
    {
        public InvalidSequenceException(string message) : base(message)
        {
            
        }
    }
}
