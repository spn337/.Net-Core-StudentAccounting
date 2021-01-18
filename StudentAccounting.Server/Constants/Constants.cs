namespace StudentAccounting.Server.Constants
{
    public static class Roles
    {
        public const string ADMIN = "Admin";

        public const string STUDENT = "Student";
    }
    public static class Authorized
    {
        public static string UserId { get; set; }
    }
    public static class Names
    {
        public const string ID = "id";
        public const string USER = "User";
        public const string ROLE = "Role";
        public const string AUTHORIZATION = "Authorization";
    }
    public static class Validated
    {
        public const int MIN_LENGTH = 8;
        public const int MAX_LENGTH = 30;
        public const string USERNAME_REGEX = "^[a-zA-Z]*$";
        public const int MIN_AGE = 18;
        public const int MAX_AGE = 100;
        public const string PASSWORD_WITH_SPECIAL_CHARACTER_REGEX = "(?=.*[[\\]{};:=<>_+^#@$!%*?&])";
        public const string PASSWORD_WITH_DIGIT_REGEX = "(?=.*\\d)";
        public const string PASSWORD_WITH_LOWERCASE_REGEX = "(?=.*[a-z])";
        public const string PASSWORD_WITH_UPPERCASE_REGEX = "(?=.*[A-Z])";

    }
    public static class ClientUrls
    {
        public const string CONFIRM_EMAIL = "http://localhost:3000/auth/confirmemail";
    }
    public static class Items
    {
        public const int MAX_COUNT = 100;
    }
    public static class Format
    {
        public const string DATETIME_TOSTRING = "MM/dd/yyyy";
    }
}
