using System;
using Model = Models.Secrets;
using Client = ClientModels.Secrets;

namespace ModelConverters.Secrets
{
    public static class SecretConverter
    {
        public static Client.Secret Convert(Model.Secret modelSecret)
        {
            if (modelSecret == null)
            {
                throw new ArgumentNullException(nameof(modelSecret));
            }

            var clientSecret = new Client.Secret
            {
                Id = modelSecret.Id.ToString(), 
                ImageUrl = modelSecret.ImageUrl, 
                Description = modelSecret.Description
            };
            
            return clientSecret;
        }
    }
}