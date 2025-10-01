using MediatR;
using UrnaEletronicaFake.Core.Interfaces;

namespace UrnaEletronicaFake.Core.Services;

public class EventBus : IEventBus
{
    private readonly IMediator _mediator;

    public EventBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task PublishAsync<T>(T notification, CancellationToken cancellationToken = default) where T : INotification
    {
        await _mediator.Publish(notification, cancellationToken);
    }
}
