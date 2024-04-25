namespace WebCoder.Domain.Rules;

public static class DataSizes
{
    public static class ProjectRepository
    {
        public const int MaxTitleLength = 64;
        public const int MinTitleLength = 4;
        public const int MaxAboutLength = 1024;
    }
    
    public static class ApplicationUser
    {
        public const int MaxUserNameLength = 64;
        public const int MaxNameLength = 64;
        public const int MaxSurnameLength = 64;
        public const int MaxProfessionLength = 64;
        public const int MaxEmailLength = 320;
    }
}