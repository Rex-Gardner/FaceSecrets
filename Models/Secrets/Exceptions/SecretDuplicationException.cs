using System;

namespace Models.Secrets.Exceptions
{
    public class SecretDuplicationException : Exception
    {
        public SecretDuplicationException(string secretId)
            : base($"Secret \"{secretId}\" already exists.")
        {
        
        }
    }
}