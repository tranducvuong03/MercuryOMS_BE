namespace MercuryOMS.Domain.Exceptions
{
    public class PaymentMethodNotSupportedException : AppException
    {
        public PaymentMethodNotSupportedException(string method)
            : base(
                message: $"Phương thức {method} không được hỗ trợ",
                statusCode: 400,
                errorCode: "PAYMENT_METHOD_NOT_SUPPORTED"
            )
        {
        }
    }
}