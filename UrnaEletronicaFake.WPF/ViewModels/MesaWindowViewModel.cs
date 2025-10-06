using System.Windows.Input;
using UrnaEletronicaFake.Mesa.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.WPF.ViewModels;

public class MesaWindowViewModel : ViewModelBase
{
    private readonly IMesaService _mesaService;
    private string _nomeEleitor = "";

    public string NomeEleitor
    {
        get => _nomeEleitor;
        set => SetProperty(ref _nomeEleitor, value);
    }

    public ICommand LiberarVotacaoCommand { get; }
    public ICommand BloquearVotacaoCommand { get; }

    public MesaWindowViewModel(
        IMesaService mesaService,
        ILogger<MesaWindowViewModel> logger) : base(logger)
    {
        _mesaService = mesaService;
        
        LiberarVotacaoCommand = new RelayCommand(LiberarVotacao);
        BloquearVotacaoCommand = new RelayCommand(BloquearVotacao);
    }

    private void LiberarVotacao()
    {
        if (string.IsNullOrEmpty(NomeEleitor))
        {
            LogWarning("Digite o nome do eleitor antes de liberar a votação.");
            return;
        }

        LogInformation($"Votação liberada para: {NomeEleitor}");
    }

    private void BloquearVotacao()
    {
        LogInformation("Votação bloqueada.");
    }
}
