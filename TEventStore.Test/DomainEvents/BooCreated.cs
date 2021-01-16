﻿using System;
using Newtonsoft.Json;

namespace TEventStore.Test.DomainEvents
{
    public sealed class BooCreated : DomainEvent
    {
        public decimal BooAmount { get; }
        public bool IsBooActive { get; }

        public BooCreated(string aggregateId, decimal booAmount, bool isBooActive) 
            : base(aggregateId) => (BooAmount, IsBooActive) = (booAmount, isBooActive);

        [JsonConstructor]
        public BooCreated(string aggregateId, Guid id, DateTime createdAt, decimal booAmount, bool isBooActive)
            : base(id, aggregateId, createdAt) => (BooAmount, IsBooActive) = (booAmount, isBooActive);
    }
}
