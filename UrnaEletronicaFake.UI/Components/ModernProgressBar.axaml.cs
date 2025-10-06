using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace UrnaEletronicaFake.UI.Components
{
    public partial class ModernProgressBar : UserControl
    {
        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<ModernProgressBar, double>(nameof(Value), 0.0);

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<ModernProgressBar, double>(nameof(Maximum), 100.0);

        public static readonly StyledProperty<double> BarHeightProperty =
            AvaloniaProperty.Register<ModernProgressBar, double>(nameof(BarHeight), 20.0);

        public static readonly StyledProperty<double> BarWidthProperty =
            AvaloniaProperty.Register<ModernProgressBar, double>(nameof(BarWidth), 200.0);

        public static readonly StyledProperty<IBrush> FillBrushProperty =
            AvaloniaProperty.Register<ModernProgressBar, IBrush>(nameof(FillBrush));

        public static readonly StyledProperty<bool> ShowTextProperty =
            AvaloniaProperty.Register<ModernProgressBar, bool>(nameof(ShowText), true);

        public static readonly StyledProperty<double> ProgressFontSizeProperty =
            AvaloniaProperty.Register<ModernProgressBar, double>(nameof(ProgressFontSize), 12.0);

        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double BarHeight
        {
            get => GetValue(BarHeightProperty);
            set => SetValue(BarHeightProperty, value);
        }

        public double BarWidth
        {
            get => GetValue(BarWidthProperty);
            set => SetValue(BarWidthProperty, value);
        }

        public IBrush FillBrush
        {
            get => GetValue(FillBrushProperty);
            set => SetValue(FillBrushProperty, value);
        }

        public bool ShowText
        {
            get => GetValue(ShowTextProperty);
            set => SetValue(ShowTextProperty, value);
        }

        public double ProgressFontSize
        {
            get => GetValue(ProgressFontSizeProperty);
            set => SetValue(ProgressFontSizeProperty, value);
        }

        public string ProgressText 
        { 
            get 
            { 
                var percentage = Maximum > 0 ? (Value / Maximum) * 100 : 0;
                return $"{percentage:F0}%"; 
            } 
        }

        public double Percentage 
        { 
            get => Maximum > 0 ? (Value / Maximum) * 100 : 0; 
        }

        public double ProgressWidth 
        { 
            get 
            { 
                var percentage = Maximum > 0 ? (Value / Maximum) * 100 : 0;
                return (percentage / 100) * BarWidth; 
            } 
        }

        public ModernProgressBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
