using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace UrnaEletronicaFake.UI.Components
{
    public partial class ModernBadge : UserControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<ModernBadge, string>(nameof(Text));

        public static readonly StyledProperty<double> BadgeFontSizeProperty =
            AvaloniaProperty.Register<ModernBadge, double>(nameof(BadgeFontSize), 12.0);

        public static readonly StyledProperty<FontWeight> BadgeFontWeightProperty =
            AvaloniaProperty.Register<ModernBadge, FontWeight>(nameof(BadgeFontWeight), FontWeight.SemiBold);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double BadgeFontSize
        {
            get => GetValue(BadgeFontSizeProperty);
            set => SetValue(BadgeFontSizeProperty, value);
        }

        public FontWeight BadgeFontWeight
        {
            get => GetValue(BadgeFontWeightProperty);
            set => SetValue(BadgeFontWeightProperty, value);
        }

        public ModernBadge()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
