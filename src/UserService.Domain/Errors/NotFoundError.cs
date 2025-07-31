using FluentResults;

namespace UserService.Domain.Errors
{
    public class NotFoundError : Error
    {
        public NotFoundError(string message) : base(message)
        {
            Metadata.Add("ErrorType", "NotFound");
            Metadata.Add("StatusCode", 404);
        }
    }
}