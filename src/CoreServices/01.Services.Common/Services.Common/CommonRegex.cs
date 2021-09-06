namespace Services.Common
{
    public static class CommonRegex
    {
        public const string NumberRegex = @"^[0-9]*$";
        public const string PhoneRegex = @"^[0-9+-]*$";
        public const string EmailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        public const string StringNonSpaceRegex = @"\S*$";
    }
}