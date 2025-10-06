using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UrnaEletronicaFake.UI.Components
{
    public partial class ModernCard : UserControl
    {

        public ModernCard()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
