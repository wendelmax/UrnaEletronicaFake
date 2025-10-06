using System.Windows.Input;
using UrnaEletronicaFake.Administration.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.WPF.ViewModels;

public class AdminWindowViewModel : ViewModelBase
{
    private readonly IAdministrationService _administrationService;
    
    public ICommand SalvarConfiguracoesCommand { get; }
    public ICommand ResetarConfiguracoesCommand { get; }
    public ICommand AdicionarCandidatoCommand { get; }
    public ICommand EditarCandidatoCommand { get; }
    public ICommand ExcluirCandidatoCommand { get; }

    public AdminWindowViewModel(
        IAdministrationService administrationService,
        ILogger<AdminWindowViewModel> logger) : base(logger)
    {
        _administrationService = administrationService;
        
        SalvarConfiguracoesCommand = new RelayCommand(SalvarConfiguracoes);
        ResetarConfiguracoesCommand = new RelayCommand(ResetarConfiguracoes);
        AdicionarCandidatoCommand = new RelayCommand(AdicionarCandidato);
        EditarCandidatoCommand = new RelayCommand(EditarCandidato);
        ExcluirCandidatoCommand = new RelayCommand(ExcluirCandidato);
    }

    private void SalvarConfiguracoes()
    {
        LogInformation("Configurações salvas com sucesso!");
    }

    private void ResetarConfiguracoes()
    {
        LogInformation("Configurações resetadas!");
    }

    private void AdicionarCandidato()
    {
        LogInformation("Funcionalidade de adicionar candidato será implementada!");
    }

    private void EditarCandidato()
    {
        LogInformation("Funcionalidade de editar candidato será implementada!");
    }

    private void ExcluirCandidato()
    {
        LogInformation("Candidato excluído com sucesso!");
    }
}
