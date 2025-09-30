using System;
using System.Threading.Tasks;
using MediatR;
using UrnaEletronicaFake.Modules.Core.Events;

namespace UrnaEletronicaFake.Services;

public class VotacaoStateService : IVotacaoStateService
{
    private readonly IMediator _mediator;
    
    public event Action? OnTerminalStateChanged;

    private bool _isLocked = true;
    public bool IsTerminalLocked => _isLocked;

    private string? _eleitorId;
    public string? EleitorAutenticadoId => _eleitorId;

    public VotacaoStateService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async void UnlockTerminal(string eleitorId)
    {
        if (_isLocked && !string.IsNullOrWhiteSpace(eleitorId))
        {
            _isLocked = false;
            _eleitorId = eleitorId;
            OnTerminalStateChanged?.Invoke();
            
            await _mediator.Publish(new TerminalUnlockedEvent(eleitorId));
        }
    }

    public async void LockTerminal()
    {
        if (!_isLocked)
        {
            _isLocked = true;
            _eleitorId = null;
            OnTerminalStateChanged?.Invoke();
            
            await _mediator.Publish(new TerminalLockedEvent());
        }
    }
} 