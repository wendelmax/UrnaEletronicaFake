using System;
using System.Collections.ObjectModel;

namespace UrnaEletronicaFake.UI.Services;

public interface ITerminalLogService
{
    ObservableCollection<string> Log { get; }
    void Registrar(string mensagem);
    void Limpar();
    event Action? LogAtualizado;
} 