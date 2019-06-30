using System;

namespace Models.Secrets
{
    public class SecretCreationInfo
    {
        public string ImageUrl { get; }
        public string Description { get; }

        public SecretCreationInfo(string imageUrl, string description)
        {
            ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}