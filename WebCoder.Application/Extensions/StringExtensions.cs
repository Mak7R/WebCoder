namespace WebCoder.Application.Extensions;

public static class StringExtensions
{
    public static bool ContainsOnlyCharacters(this string str, string characters)
    {
        return str.All(characters.Contains);
    }
}