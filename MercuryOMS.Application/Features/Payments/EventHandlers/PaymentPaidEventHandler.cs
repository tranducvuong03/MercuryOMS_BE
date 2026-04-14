using MediatR;
using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Events;
using MercuryOMS.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MercuryOMS.Application.Features
{
    public class PaymentPaidEventHandler
        : INotificationHandler<PaymentPaidEvent>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;

        public PaymentPaidEventHandler(
            IUnitOfWork uow,
            IUserService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        public async Task Handle(PaymentPaidEvent e, CancellationToken ct)
        {
            var order = await _uow.GetRepository<Order>()
                .Query
                .FirstOrDefaultAsync(x => x.Id == e.OrderId, ct);

            if (order == null)
                throw new NotFoundException(Message.OrderNotFound);

            var user = await _userService.GetByIdAsync(order.UserId);

            if (user == null)
                throw new NotFoundException(Message.UserNotFound);

            var message = new PaymentPaidMessage
            {
                PaymentId = e.PaymentId,
                OrderId = e.OrderId,
                Amount = e.Amount,
                Email = user.Email,
                FullName = user.FullName
            };

            var outbox = new OutboxMessage(
                type: nameof(PaymentPaidMessage),
                queue: QueueNames.PaymentPaid,
                payload: JsonSerializer.Serialize<PaymentPaidMessage>(message)
            );

            await _uow.GetRepository<OutboxMessage>().AddAsync(outbox);
            await _uow.SaveChangesAsync(ct);
        }
    }
}