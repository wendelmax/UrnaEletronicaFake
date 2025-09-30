using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace UrnaEletronicaFake.Modules.Core.Services;

public class EventBusService : IEventBus
{
    private readonly IMediator _mediator;

    public EventBusService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task PublishAsync<T>(T notification, CancellationToken cancellationToken = default) 
        where T : INotification
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        await _mediator.Publish(notification, cancellationToken);
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return await _mediator.Send(request, cancellationToken);
    }

    public async Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        await _mediator.Send(request, cancellationToken);
    }
}