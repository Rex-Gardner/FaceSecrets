using System.Runtime.Serialization;

namespace ClientModels.Secrets
{
    public class SecretCreationInfo
    {
        [DataMember(IsRequired = true)]
        public string ImageUrl { get; set; }
    }
}