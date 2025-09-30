using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UrnaEletronicaFake.Modules.Core.Services;
using UrnaEletronicaFake.Modules.Core.Events;

namespace UrnaEletronicaFake.Examples;

public partial class CoreModuleUsageExample : ObservableObject
{
    private readonly IEventBus _eventBus;
    private readonly ITerminalStateService _terminalStateService;

    public CoreModuleUsageExample(IEventBus eventBus, ITerminalStateService terminalStateService)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _terminalStateService = terminalStateService ?? throw new ArgumentNullException(nameof(terminalStateService));

        _terminalStateService.StateChanged += OnTerminalStateChanged;
    }

    private void OnTerminalStateChanged(object? sender, string newState)
    {
        CurrentState = newState;
    }

    [ObservableProperty]
    private string currentState = "Stopped";

    [ObservableProperty]
    private string terminalId = string.Empty;

    partial void OnCurrentStateChanged(string value)
    {
        TerminalId = _terminalStateService.TerminalId;
    }

    [RelayCommand]
    private async Task StartTerminalAsync()
    {
        await _terminalStateService.StartAsync();
        
        await _eventBus.PublishAsync(new TerminalLogEvent
        {
            TerminalId = _terminalStateService.TerminalId,
            Message = "Terminal iniciado pelo usuário",
            Level = "Info"
        });
    }

    [RelayCommand]
    private async Task StopTerminalAsync()
    {
        await _terminalStateService.StopAsync();
        
        await _eventBus.PublishAsync(new TerminalLogEvent
        {
            TerminalId = _terminalStateService.TerminalId,
            Message = "Terminal parado pelo usuário",
            Level = "Info"
        });
    }

    [RelayCommand]
    private async Task ChangeToVotingStateAsync()
    {
        await _terminalStateService.ChangeStateAsync("Voting");
    }

    [RelayCommand]
    private async Task ChangeToIdleStateAsync()
    {
        await _terminalStateService.ChangeStateAsync("Idle");
    }

    [RelayCommand]
    private async Task SimulateErrorAsync()
    {
        await _eventBus.PublishAsync(new TerminalErrorEvent
        {
            TerminalId = _terminalStateService.TerminalId,
            ErrorMessage = "Erro simulado para demonstração",
            Exception = new InvalidOperationException("Erro de exemplo")
        });
    }
}