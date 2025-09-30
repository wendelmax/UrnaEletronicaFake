using System;
using System.Threading;
using System.Threading.Tasks;

namespace UrnaEletronicaFake.Modules.Core.Services;

public interface ITerminalStateService
{
    string CurrentState { get; }
    string TerminalId { get; }
    DateTime LastStateChange { get; }
    
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    Task ChangeStateAsync(string newState, CancellationToken cancellationToken = default);
    
    event EventHandler<string>? StateChanged;
    event EventHandler? Started;
    event EventHandler? Stopped;
}