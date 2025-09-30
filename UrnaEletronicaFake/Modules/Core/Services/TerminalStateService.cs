using System;
using System.Threading;
using System.Threading.Tasks;
using UrnaEletronicaFake.Modules.Core.Events;

namespace UrnaEletronicaFake.Modules.Core.Services;

public class TerminalStateService : ITerminalStateService
{
    private readonly IEventBus _eventBus;
    private string _currentState = "Stopped";
    private readonly string _terminalId = Environment.MachineName + "-" + Guid.NewGuid().ToString("N")[..8];

    public string CurrentState => _currentState;
    public string TerminalId => _terminalId;
    public DateTime LastStateChange { get; private set; } = DateTime.Now;

    public event EventHandler<string>? StateChanged;
    public event EventHandler? Started;
    public event EventHandler? Stopped;

    public TerminalStateService(IEventBus eventBus)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_currentState == "Running")
            return;

        var previousState = _currentState;
        _currentState = "Running";
        LastStateChange = DateTime.Now;

        Started?.Invoke(this, EventArgs.Empty);
        StateChanged?.Invoke(this, _currentState);

        await _eventBus.PublishAsync(new TerminalStartedEvent
        {
            TerminalId = _terminalId,
            StartTime = LastStateChange
        }, cancellationToken);

        await _eventBus.PublishAsync(new TerminalStateChangedEvent
        {
            TerminalId = _terminalId,
            PreviousState = previousState,
            CurrentState = _currentState,
            ChangedAt = LastStateChange
        }, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_currentState == "Stopped")
            return;

        var previousState = _currentState;
        _currentState = "Stopped";
        LastStateChange = DateTime.Now;

        Stopped?.Invoke(this, EventArgs.Empty);
        StateChanged?.Invoke(this, _currentState);

        await _eventBus.PublishAsync(new TerminalStoppedEvent
        {
            TerminalId = _terminalId,
            StopTime = LastStateChange
        }, cancellationToken);

        await _eventBus.PublishAsync(new TerminalStateChangedEvent
        {
            TerminalId = _terminalId,
            PreviousState = previousState,
            CurrentState = _currentState,
            ChangedAt = LastStateChange
        }, cancellationToken);
    }

    public async Task ChangeStateAsync(string newState, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newState))
            throw new ArgumentException("State cannot be null or whitespace.", nameof(newState));

        if (_currentState == newState)
            return;

        var previousState = _currentState;
        _currentState = newState;
        LastStateChange = DateTime.Now;

        StateChanged?.Invoke(this, _currentState);

        await _eventBus.PublishAsync(new TerminalStateChangedEvent
        {
            TerminalId = _terminalId,
            PreviousState = previousState,
            CurrentState = _currentState,
            ChangedAt = LastStateChange
        }, cancellationToken);
    }
}