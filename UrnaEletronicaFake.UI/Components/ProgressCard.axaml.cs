using Avalonia;
using Avalonia.Controls;

namespace UrnaEletronicaFake.UI.Components;

public partial class ProgressCard : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ProgressCard, string>(nameof(Title), "");

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<ProgressCard, string>(nameof(Subtitle), "");

    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<ProgressCard, double>(nameof(Progress), 0.0);

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public ProgressCard()
    {
        InitializeComponent();
    }
}
