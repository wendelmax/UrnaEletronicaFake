using Avalonia.Controls;

namespace UrnaEletronicaFake.UI.Services;

public interface IWindowManagerService
{
    void ShowMainWindow();
    void ShowVotingWindow();
    void ShowMesaWindow();
    void ShowDashboardWindow();
    void ShowAdminWindow();
    void ShowAuditWindow();
    void ShowResultsWindow();
    void CloseAllWindows();
    void MinimizeAllWindows();
    void RestoreAllWindows();
    bool IsWindowOpen<T>() where T : Window;
    T? GetWindow<T>() where T : Window;
    void SetMainWindow(Window mainWindow);
    Window? GetMainWindow();
    void OnDashboardWindowFechada();
    void OnMesaWindowFechada();
    void OnVotacaoWindowFechada();
}
