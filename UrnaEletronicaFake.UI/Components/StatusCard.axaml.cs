using Avalonia;
using Avalonia.Controls;

namespace UrnaEletronicaFake.UI.Components;

public partial class StatusCard : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<StatusCard, string>(nameof(Title), "");

    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<StatusCard, string>(nameof(Value), "");

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<StatusCard, string>(nameof(Subtitle), "");

    public static readonly StyledProperty<bool> IsOnlineProperty =
        AvaloniaProperty.Register<StatusCard, bool>(nameof(IsOnline), false);

    public static readonly StyledProperty<string> StatusTypeProperty =
        AvaloniaProperty.Register<StatusCard, string>(nameof(StatusType), "info");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public bool IsOnline
    {
        get => GetValue(IsOnlineProperty);
        set => SetValue(IsOnlineProperty, value);
    }

    public string StatusType
    {
        get => GetValue(StatusTypeProperty);
        set => SetValue(StatusTypeProperty, value);
    }

    public StatusCard()
    {
        InitializeComponent();
    }
}
