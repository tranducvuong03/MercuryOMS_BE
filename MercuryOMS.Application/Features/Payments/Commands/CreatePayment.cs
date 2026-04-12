using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Application.Features
{
    public record CreatePaymentCommand(
        Guid OrderId,
        decimal Amount,
        PaymentMethod Method
    ) : IRequest<Result<Guid>>;

    public class CreatePaymentHandler
        : IRequestHandler<CreatePaymentCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentFactory _factory;

        public CreatePaymentHandler(
            IUnitOfWork unitOfWork,
            IPaymentFactory factory)
        {
            _unitOfWork = unitOfWork;
            _factory = factory;
        }

        public async Task<Result<Guid>> Handle(
            CreatePaymentCommand request,
            CancellationToken ct)
        {
            var strategy = _factory.Get(request.Method.ToString());
            var payment = await strategy.CreatePaymentAsync(
                request.OrderId,
                request.Amount,
                ct
            );
            payment.MarkPaid();
            await _unitOfWork.GetRepository<Payment>()
                .AddAsync(payment, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Success(payment.Id, "Tạo payment thành công");
        }
    }
}