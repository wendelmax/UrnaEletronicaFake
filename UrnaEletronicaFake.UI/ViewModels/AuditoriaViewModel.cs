using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.UI.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.UI.ViewModels;

public partial class AuditoriaViewModel : ViewModelBase
{
    private readonly IAuditoriaService _auditoriaService;
    
    private ObservableCollection<Auditoria> _auditorias;
    private bool _isLoading;
    private string _statusMessage = "";

    public AuditoriaViewModel(IAuditoriaService auditoriaService, ILogger<AuditoriaViewModel> logger) : base(logger)
    {
        _auditoriaService = auditoriaService;
        
        _auditorias = new ObservableCollection<Auditoria>();
        
        // Comandos
        
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