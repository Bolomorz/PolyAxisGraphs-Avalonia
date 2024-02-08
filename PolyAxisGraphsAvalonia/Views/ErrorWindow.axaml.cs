using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PolyAxisGraphsAvalonia.Views
{
    public partial class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
        }

        public static void Show(string errormessage)
        {
            ErrorWindow window = new ErrorWindow();
            window.Topmost = true;
            var tbmessage = window.FindControl<TextBlock>("tbmessage");
            if (tbmessage is not null) tbmessage.Text = errormessage;
            window.Show();
        }

        private void ClickOk(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
