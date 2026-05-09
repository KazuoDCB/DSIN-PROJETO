namespace cabeleleira_leila.Models;

public class ErrorMessage
{
    public string Property { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;


    public static ErrorMessage CreateErrorMessage(string property, string message)
    {
        return new ErrorMessage
        {
            Message = message,
            Property = property
        };
    }
}