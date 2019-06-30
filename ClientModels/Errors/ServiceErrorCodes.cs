namespace ClientModels.Errors
{
    public static class ServiceErrorCodes
    {
        public const string NotFound = "not-found";
        public const string NotAcceptable = "auth:not-acceptable";
        public const string Unauthorized = "auth:unauthorized";
        public const string InvalidCredentials = "auth:invalid-credentials";
        public const string BadRequest = "bad-request";
    }
}