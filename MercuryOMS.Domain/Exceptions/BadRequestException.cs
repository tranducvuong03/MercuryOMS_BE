namespace MercuryOMS.Domain.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, 400, "BAD_REQUEST") { }
    }
}
