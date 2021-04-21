using System;

namespace TEventStore.Exceptions
{
    public sealed class ConcurrencyCheckException : Exception
    {
        public ConcurrencyCheckException(string message) : base(message)
        {
            
        }
    }
}
