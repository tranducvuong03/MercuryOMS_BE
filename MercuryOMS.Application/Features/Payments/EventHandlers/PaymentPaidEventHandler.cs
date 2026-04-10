using MediatR;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Events;
using MercuryOMS.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public class PaymentPaidEventHandler
        : INotificationHandler<PaymentPaidEvent>
    {
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;

        public PaymentPaidEventHandler(
            IMessageBus bus,
            IUnitOfWork uow,
            IUserService userService)
        {
            _bus = bus;
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

            await _bus.PublishAsync("payment.paid", new PaymentPaidMessage
            {
                PaymentId = e.PaymentId,
                OrderId = e.OrderId,
                Amount = e.Amount,
                Email = user.Email,
                FullName = user.FullName
            });
        }
    }
}