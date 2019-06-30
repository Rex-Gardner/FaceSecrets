using System;

namespace Models.Secrets.Exceptions
{
    public class SecretNotFoundException : Exception
    {
        public SecretNotFoundException(string secretId)
            : base($"Secret \"{secretId}\" not found.")
        {
        
        }
    }
}