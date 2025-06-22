using System;
using System.Collections.ObjectModel;

namespace UrnaEletronicaFake.Services;

public class TerminalLogService : ITerminalLogService
{
    public ObservableCollection<string> Log { get; } = new();
    public event Action? LogAtualizado;

    public void Registrar(string mensagem)
    {
        var registro = $"[{DateTime.Now:HH:mm:ss}] {mensagem}";
        Log.Add(registro);
        
        // Escrever no console do sistema
        Console.WriteLine(registro);
        
        LogAtualizado?.Invoke();
    }

    public void Limpar()
    {
        Log.Clear();
        LogAtualizado?.Invoke();
    }
} 