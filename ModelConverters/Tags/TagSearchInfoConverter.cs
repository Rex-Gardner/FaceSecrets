using System;
using Model = Models.Tags;
using Client = ClientModels.Tags;

namespace ModelConverters.Tags
{
    public static class TagSearchInfoConverter
    {
        public static Model.TagSearchInfo Convert(Client.TagSearchInfo clientSearchInfo)
        {
            if (clientSearchInfo == null)
            {
                throw new ArgumentNullException(nameof(clientSearchInfo));
            }

            var modelSearchInfo = new Model.TagSearchInfo
            {
                Offset = clientSearchInfo.Offset,
                Limit = clientSearchInfo.Limit,
                Tags = clientSearchInfo.Tag
            };

            return modelSearchInfo;
        }
    }
}