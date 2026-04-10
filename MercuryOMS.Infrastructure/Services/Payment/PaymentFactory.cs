using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Infrastructure.Services
{
    public class PaymentFactory : IPaymentFactory
    {
        private readonly IEnumerable<IPaymentStrategyService> _strategies;

        public PaymentFactory(IEnumerable<IPaymentStrategyService> strategies)
        {
            _strategies = strategies;
        }

        public IPaymentStrategyService Get(string method)
        {
            var strategy = _strategies
                .FirstOrDefault(x => x.Method == method);

            if (strategy == null)
                throw new PaymentMethodNotSupportedException(method);

            return strategy;
        }
    }
}
