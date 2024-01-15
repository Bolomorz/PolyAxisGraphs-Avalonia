using Avalonia.Controls;

namespace PolyAxisGraphsAvalonia.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(UserControl view)
        {
            Content = view;
            InitializeComponent();
        }
    }
}
