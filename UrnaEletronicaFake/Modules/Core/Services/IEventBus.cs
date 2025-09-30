using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace UrnaEletronicaFake.Modules.Core.Services;

public interface IEventBus
{
    Task PublishAsync<T>(T notification, CancellationToken cancellationToken = default) 
        where T : INotification;
        
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    
    Task SendAsync(IRequest request, CancellationToken cancellationToken = default);
}