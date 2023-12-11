using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace PolyAxisGraphs_Avalonia.Views;

public partial class MainView : UserControl
{
    CanvasGraph cg { get; set; }

    public static FilePickerFileType DataFiles = new("DataFiles") { Patterns = new[] { "*.txt", "*.csv", "*.xlsx" } };
    public MainView()
    {
        InitializeComponent();
        cg = new CanvasGraph(MainCanvas);
        LoadControls();
    }

    public void LoadControls()
    {
        TBFile.FontFamily = new Avalonia.Media.FontFamily("Consolas");
        TBFile.FontSize = 9;
        TBFile.Text = cg.pag.settings.currentlang.FindElement("tbopenfile");
        BTOpenFile.FontFamily = new Avalonia.Media.FontFamily("Consolas");
        BTOpenFile.FontSize = 9;
        BTOpenFile.Content = cg.pag.settings.currentlang.FindElement("btopenfile");
    }

    private async void OpenFileButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        var files = await toplevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = "Open Data File",
            AllowMultiple = false,
            FileTypeFilter = new[] { DataFiles },
            SuggestedStartLocation = await toplevel.StorageProvider.TryGetFolderFromPathAsync(
                new System.Uri(new System.Uri(System.AppDomain.CurrentDomain.BaseDirectory), cg.pag.settings.initialdirectory))
        });

        if(files != null)
        {
            string file = files[0].Path.AbsolutePath;
            TBFile.Text = file;
            cg.SetFile(file);
        }
    }
}
