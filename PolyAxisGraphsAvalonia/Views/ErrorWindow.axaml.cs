using Avalonia.Controls;

namespace PolyAxisGraphsAvalonia.Views
{
    public partial class ErrorWindow : Window
    {
        private ErrorWindow()
        {
            InitializeComponent();
        }

        public static void Show(string errorcode, CanvasGraph cg)
        {
            ErrorWindow window = new ErrorWindow();
            var tbmessage = window.FindControl<TextBlock>("tbmessage");
            if (tbmessage is not null && cg.pag.settings.currentlang is not null) tbmessage.Text = cg.pag.settings.currentlang.FindElement("errorcode");

        }
    }
}
