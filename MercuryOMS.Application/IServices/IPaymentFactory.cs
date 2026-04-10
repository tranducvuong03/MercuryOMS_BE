namespace MercuryOMS.Application.IServices
{
    public interface IPaymentFactory
    {
        IPaymentStrategyService Get(string method);
    }
}
