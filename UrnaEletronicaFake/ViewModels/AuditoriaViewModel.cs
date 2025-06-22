using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.ViewModels;

public class AuditoriaViewModel : ViewModelBase
{
    private readonly IAuditoriaService _auditoriaService;
    
    private ObservableCollection<Auditoria> _auditorias;
    private bool _isLoading;
    private string _statusMessage = "";

    public AuditoriaViewModel(IAuditoriaService auditoriaService)
    {
        _auditoriaService = auditoriaService;
        
        _auditorias = new ObservableCollection<Auditoria>();
        
        // Comandos
        CarregarAuditoriasCommand = new RelayCommand(async () => await CarregarAuditorias());
        
        // Carregar dados iniciais
        _ = CarregarAuditorias();
    }

    public ObservableCollection<Auditoria> Auditorias
    {
        get => _auditorias;
        set => SetProperty(ref _auditorias, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand CarregarAuditoriasCommand { get; }

    private async Task CarregarAuditorias()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Carregando log de auditoria...";
            
            var auditorias = await _auditoriaService.ObterAuditoriasAsync();
            
            Auditorias.Clear();
            foreach (var auditoria in auditorias)
            {
                Auditorias.Add(auditoria);
            }
            
            StatusMessage = $"Carregadas {Auditorias.Count} ações de auditoria";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao carregar auditoria: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
} 