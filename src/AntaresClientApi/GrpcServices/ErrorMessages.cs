namespace AntaresClientApi.GrpcServices
{
    public static class ErrorMessages
    {
        public const string AssetNotFound = "Asset not found";
        public const string AssetPairNotFound = "Asset pair not found";
        public const string AssetPairDisabled = "Asset pair is disabled";
        public static string LessThanZero(string name) => $"{name} cannot be less than zero.";
        public static string MustBeGreaterThan(string name, string minValue) => $"{name} must be greater than {minValue}";
        public static string TooBig(string name, string value, string maxValue) =>
            $"{name} '{value}' is too big, maximum is '{maxValue}'.";

        public const string NotEnoughFunds = "Not enough funds";
        public const string MeNotAvailable = "ME not available";
        public static string CantBeEmpty(string name) => $"{name} cannot be empty.";
        public static string InvalidFieldValue(string name) => $"{name} value is invalid.";

        public const string InvalidLoginOrPassword = "Invalid login or password";

        public const string RuntimeProblemTryAgain = "Sorry, the request could not be completed on the server. Please try again.";

        public const string InvalidVerificationCode = "Invalid verification code";

        public const string CannotDeliverySms = "SMS could not be delivered. Please try again later or use a different number.";

        public const string CannotDeliveryEmail = "EMail could not be delivered. Please try again later or use a different number.";

        public const string Unauthorized = "Unauthorized.";

        public const string ClientAlreadyExist = "Email is already registered. Try to sign-in or use a different email address.";

        

    }
}
