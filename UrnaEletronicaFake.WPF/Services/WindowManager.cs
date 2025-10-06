using UrnaEletronicaFake.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace UrnaEletronicaFake.WPF.Services
{
    public class WindowManager
    {
        private readonly IServiceProvider _serviceProvider;
        private VotacaoWindow? _votacaoWindow;
        private MesaWindow? _mesaWindow;
        private DashboardWindow? _dashboardWindow;
        private AdminWindow? _adminWindow;

        public WindowManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OpenVotacaoWindow()
        {
            try
            {
                if (_votacaoWindow == null || !_votacaoWindow.IsLoaded)
                {
                    _votacaoWindow = _serviceProvider.GetRequiredService<VotacaoWindow>();
                    _votacaoWindow.Closed += (sender, e) => _votacaoWindow = null;
                }
                _votacaoWindow.Show();
                _votacaoWindow.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir Urna: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenMesaWindow()
        {
            try
            {
                if (_mesaWindow == null || !_mesaWindow.IsLoaded)
                {
                    _mesaWindow = _serviceProvider.GetRequiredService<MesaWindow>();
                    _mesaWindow.Closed += (sender, e) => _mesaWindow = null;
                }
                _mesaWindow.Show();
                _mesaWindow.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir Mesa: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenDashboardWindow()
        {
            try
            {
                if (_dashboardWindow == null || !_dashboardWindow.IsLoaded)
                {
                    _dashboardWindow = _serviceProvider.GetRequiredService<DashboardWindow>();
                    _dashboardWindow.Closed += (sender, e) => _dashboardWindow = null;
                }
                _dashboardWindow.Show();
                _dashboardWindow.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir Dashboard: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenAdminWindow()
        {
            try
            {
                if (_adminWindow == null || !_adminWindow.IsLoaded)
                {
                    _adminWindow = _serviceProvider.GetRequiredService<AdminWindow>();
                    _adminWindow.Closed += (sender, e) => _adminWindow = null;
                }
                _adminWindow.Show();
                _adminWindow.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir Admin: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LiberarVotacao(string nomeEleitor)
        {
            _votacaoWindow?.LiberarVotacao(nomeEleitor);
        }

        public void BloquearVotacao()
        {
            _votacaoWindow?.BloquearVotacao();
        }
    }
}
