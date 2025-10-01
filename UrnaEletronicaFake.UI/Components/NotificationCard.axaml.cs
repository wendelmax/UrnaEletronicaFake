using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace UrnaEletronicaFake.UI.Components;

public partial class NotificationCard : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<NotificationCard, string>(nameof(Title), "");

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<NotificationCard, string>(nameof(Message), "");

    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<NotificationCard, string>(nameof(Icon), "ℹ️");

    public static readonly StyledProperty<string> TypeProperty =
        AvaloniaProperty.Register<NotificationCard, string>(nameof(Type), "info");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public ICommand CloseCommand { get; }

    public NotificationCard()
    {
        InitializeComponent();
        CloseCommand = new RelayCommand(() => 
        {
            // Implementar lógica de fechamento
        });
    }
}
