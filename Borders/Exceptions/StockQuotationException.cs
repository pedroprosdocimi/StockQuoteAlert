using System;

namespace Borders.Exceptions
{
    [Serializable]
    public class EmailRepositoryException : Exception
    {
        public EmailRepositoryException(string errorMessage) : base(errorMessage) { }
    }
}
