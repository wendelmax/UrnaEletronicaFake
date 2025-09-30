using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UrnaEletronicaFake.Modules.Core.Services;

public interface INotification
{
}

public interface INotificationHandler<in TNotification>
    where TNotification : class, INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}

public interface IEventBus
{
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : class, INotification;
    
    void Subscribe<TNotification>(INotificationHandler<TNotification> handler)
        where TNotification : class, INotification;
    
    void Unsubscribe<TNotification>(INotificationHandler<TNotification> handler)
        where TNotification : class, INotification;
}

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<object>> _handlers = new();
    private readonly object _lock = new object();

    public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : class, INotification
    {
        List<object>? handlers;
        
        lock (_lock)
        {
            if (!_handlers.TryGetValue(typeof(TNotification), out handlers))
            {
                return Task.CompletedTask;
            }
            
            handlers = handlers.ToList();
        }

        var tasks = handlers
            .Cast<INotificationHandler<TNotification>>()
            .Select(h => h.Handle(notification, cancellationToken))
            .ToArray();

        return Task.WhenAll(tasks);
    }

    public void Subscribe<TNotification>(INotificationHandler<TNotification> handler)
        where TNotification : class, INotification
    {
        lock (_lock)
        {
            if (!_handlers.TryGetValue(typeof(TNotification), out var handlers))
            {
                handlers = new List<object>();
                _handlers[typeof(TNotification)] = handlers;
            }
            
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }
    }

    public void Unsubscribe<TNotification>(INotificationHandler<TNotification> handler)
        where TNotification : class, INotification
    {
        lock (_lock)
        {
            if (_handlers.TryGetValue(typeof(TNotification), out var handlers))
            {
                handlers.Remove(handler);
                
                if (handlers.Count == 0)
                {
                    _handlers.Remove(typeof(TNotification));
                }
            }
        }
    }
}