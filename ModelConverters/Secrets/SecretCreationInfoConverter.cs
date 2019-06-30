using System;
using Model = Models.Secrets;
using Client = ClientModels.Secrets;

namespace ModelConverters.Secrets
{
    public static class SecretCreationInfoConverter
    {
        public static Model.SecretCreationInfo Convert(Client.SecretCreationInfo clientCreationInfo)
        {
            if (clientCreationInfo == null)
            {
                throw new ArgumentNullException(nameof(clientCreationInfo));
            }

            //todo stub
            var modelCreationInfo = new Model.SecretCreationInfo(clientCreationInfo.ImageUrl, string.Empty);
            return modelCreationInfo;
        }
    }
}