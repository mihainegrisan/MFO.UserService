using FluentResults;

namespace MFO.UserService.Domain.Errors;

public class UnauthorizedAccessError : Error
{
    public UnauthorizedAccessError(string message) : base(message)
    {
        Metadata.Add("ErrorType", "Unauthorized");
        Metadata.Add("StatusCode", 401);
    }
}