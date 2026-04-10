namespace MercuryOMS.Domain.Exceptions
{
    public class DomainException : AppException
    {
        public DomainException(string message)
            : base(message, 400, "DOMAIN_ERROR") { }
    }
}
