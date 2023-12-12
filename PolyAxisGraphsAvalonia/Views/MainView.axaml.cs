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
        if (cg.pag.settings.fontfamily is not null && cg.pag.settings.controlfontsize is not null && cg.pag.settings.currentlang is not null)
        {
            var fontfamily = new Avalonia.Media.FontFamily(cg.pag.settings.fontfamily);
            var fontsize = cg.pag.settings.controlfontsize;
            TBFile.FontFamily = fontfamily;
            TBFile.FontSize = (double)fontsize;
            TBFile.Text = cg.pag.settings.currentlang.FindElement("tbopenfile");
            BTOpenFile.FontFamily = fontfamily;
            BTOpenFile.FontSize = (double)fontsize;
            BTOpenFile.Content = cg.pag.settings.currentlang.FindElement("btopenfile");
        }
    }

    private async void OpenFileButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        if (toplevel is null) return;
        if (cg.pag.settings.initialdirectory is null)
        {
            var files = await toplevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Data File",
                AllowMultiple = false,
                FileTypeFilter = new[] { DataFiles }
            });

            if (files is not null && files.Count >= 1)
            {
                string file = files[0].Path.AbsolutePath;
                TBFile.Text = file;
                cg.SetFile(file);
            }
        }
        else
        {
            var files = await toplevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Data File",
                AllowMultiple = false,
                FileTypeFilter = new[] { DataFiles },
                SuggestedStartLocation = (cg.pag.settings.initialdirectory[0] == '.') ?
                await toplevel.StorageProvider.TryGetFolderFromPathAsync(new System.Uri(new System.Uri(System.AppDomain.CurrentDomain.BaseDirectory), cg.pag.settings.initialdirectory)) :
                await toplevel.StorageProvider.TryGetFolderFromPathAsync(new System.Uri(cg.pag.settings.initialdirectory))
            });

            if (files is not null && files.Count >= 1)
            {
                string file = files[0].Path.AbsolutePath;
                TBFile.Text = file;
                cg.SetFile(file);
            }
        }
    }

    private void OpenSettingsButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }
}
