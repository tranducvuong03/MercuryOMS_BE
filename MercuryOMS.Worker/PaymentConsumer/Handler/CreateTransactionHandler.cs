using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public class CreateTransactionHandler : IPaymentPaidHandler
    {
        private readonly IUnitOfWork _uow;

        public CreateTransactionHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task HandleAsync(PaymentPaidMessage message)
        {
            var repo = _uow.GetRepository<Transaction>();

            var existed = repo.Query.Any(x => x.PaymentId == message.PaymentId);
            if (existed)
                return;

            var transaction = new Transaction(
                message.PaymentId,
                message.OrderId,
                message.Amount,
                TransactionType.Payment
            );

            await repo.AddAsync(transaction);

            await _uow.SaveChangesAsync();
        }
    }
}