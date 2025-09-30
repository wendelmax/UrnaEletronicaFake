using MediatR;

namespace UrnaEletronicaFake.Modules.Core.Events;

public record TerminalUnlockedEvent(string EleitorId) : INotification;

public record TerminalLockedEvent : INotification;