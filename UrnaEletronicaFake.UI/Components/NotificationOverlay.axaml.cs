using System.Collections.ObjectModel;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using UrnaEletronicaFake.UI.Models;
using UrnaEletronicaFake.UI.Services;

namespace UrnaEletronicaFake.UI.Components;

public partial class NotificationOverlay : UserControl
{
    public NotificationOverlay()
    {
        InitializeComponent();
        DataContext = new NotificationOverlayViewModel();
    }
    
    public void Initialize(INotificationService notificationService)
    {
        if (DataContext is NotificationOverlayViewModel viewModel)
        {
            viewModel.Initialize(notificationService);
        }
    }
}

public partial class NotificationOverlayViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<NotificationModel> _notifications = new();
    
    public void Initialize(INotificationService notificationService)
    {
        notificationService.OnNotificationRequested += OnNotificationReceived;
    }
    
    private void OnNotificationReceived(NotificationModel notification)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Notifications.Insert(0, notification);
            
            var timer = new System.Timers.Timer(notification.DurationMs);
            timer.Elapsed += (s, e) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Notifications.Remove(notification);
                });
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        });
    }
}


