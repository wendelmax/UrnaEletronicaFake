namespace UrnaEletronicaFake.Core.Interfaces;

public interface INotificationHandler<in T>
{
    Task Handle(T notification, CancellationToken cancellationToken = default);
}

