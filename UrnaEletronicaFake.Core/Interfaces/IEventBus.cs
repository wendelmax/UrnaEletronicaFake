using MediatR;

namespace UrnaEletronicaFake.Core.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T notification, CancellationToken cancellationToken = default) where T : INotification;
}