using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, object key) : base($"{entity} with key '{key}' not found") { }
    }

    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
