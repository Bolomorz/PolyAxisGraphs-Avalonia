using Avalonia.Controls;

namespace PolyAxisGraphsAvalonia.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(UserControl view)
        {
            Content = view;
            this.Width = view.Width;
            this.Height = view.Height;
            this.MaxWidth = view.Width;
            this.MaxHeight = view.Height;
            InitializeComponent();
        }

    }
}
