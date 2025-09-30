using MediatR;

namespace UrnaEletronicaFake.Modules.Core.Services;

public interface IEventBus
{
    Task PublishAsync<T>(T notification) where T : INotification;
}

public class EventBus : IEventBus
{
    private readonly IMediator _mediator;

    public EventBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task PublishAsync<T>(T notification) where T : INotification
    {
        await _mediator.Publish(notification);
    }
}