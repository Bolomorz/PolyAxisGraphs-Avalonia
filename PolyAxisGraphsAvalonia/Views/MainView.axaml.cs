using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MathNet.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System.Diagnostics;
using System.IO;
using PolyAxisGraphs_Backend;

namespace PolyAxisGraphsAvalonia.Views;

public partial class MainView : UserControl
{
    CanvasGraph cg { get; set; }

    public static FilePickerFileType DataFiles = new("DataFiles") { Patterns = new[] { "*.txt", "*.csv", "*.xlsx" } };
    public static FilePickerFileType Images = new("Images") { Patterns = new[] { "*.png" } };
    
    public MainView()
    {
        InitializeComponent();
        CheckFileSystem();
        cg = new CanvasGraph(MainCanvas, this);
        LoadControls();
        DataContext = this;
    }

    private void CheckFileSystem()
    {
        if (!Directory.Exists("DataFiles")) Directory.CreateDirectory("DataFiles");
        if (!Directory.Exists("LanguageFile")) Directory.CreateDirectory("LanguageFile");
        if (!File.Exists("Settings.ini")) Settings.CreateDefault();
        if (!File.Exists(@"LanguageFile\EN.lng")) LanguagePack.CreateDefault();
        if (!File.Exists(@"DataFiles\TestFile0.txt")) FileGenerator.GenerateDefault(@"DataFiles\TestFile0.txt");
    }

    public void LoadControls()
    {
        if (cg.pag is null)
        {
            ErrorWindow.Show("error: pag is null -> settings file not found");
            return;
        }
        var ff = cg.pag.settings.FindValueFromKey("fontfamily");
        var cfs = cg.pag.settings.FindValueFromKey("controlfontsize");
        if (ff is not null && cfs is not null)
        {
            var fontfamily = new Avalonia.Media.FontFamily(ff);
            var fontsize = PolyAxisGraph.ReadStringToDouble(cfs);
            TBFile.FontFamily = fontfamily;
            TBFile.FontSize = (double)fontsize;
            TBFile.Text = cg.pag.settings.currentlang.FindElement("tbopenfile");
            BTOpenFile.FontFamily = fontfamily;
            BTOpenFile.FontSize = (double)fontsize;
            BTOpenFile.Content = cg.pag.settings.currentlang.FindElement("btopenfile");
            TBPos.FontFamily = fontfamily;
            TBPos.FontSize = (double)fontsize;
        }
        else
        {
            ErrorWindow.Show(string.Format("error: failed to load: settings variable is null.\nfontfamily={0}\ncontrolfontsize={1}",
                ff, cfs));
        }
    }

    public void CreateControls()
    {
        if (cg.pag is null)
        {
            ErrorWindow.Show("error: pag is null -> settings file not found");
            return;
        }
        ControlsGrid.Children.Clear();
        var ff = cg.pag.settings.FindValueFromKey("fontfamily");
        var fs = cg.pag.settings.FindValueFromKey("controlfontsize");
        var fontfamily = (ff is null) ? new Avalonia.Media.FontFamily("Consolas") : new Avalonia.Media.FontFamily(ff);
        var fontsize = (fs is null) ? 15 : PolyAxisGraph.ReadStringToDouble(fs);
        LanguagePack language = (cg.pag.settings.currentlang is null) ? LanguagePack.EN : cg.pag.settings.currentlang;
        try {
            Button BTSave = new Button();
            BTSave.Content = language.FindElement("btsavefilepng");
            BTSave.FontFamily = fontfamily;
            BTSave.FontSize = (double)fontsize;
            BTSave.Command = ReactiveCommand.Create(() => { SaveFileButtonClick(); });
            BTSave.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            BTSave.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(BTSave, 0);
            Grid.SetRow(BTSave, 0);
            ControlsGrid.Children.Add(BTSave);

            Button BTXaxis = new Button();
            BTXaxis.Content = "x: " + cg.pag.xaxisname;
            BTXaxis.FontFamily = fontfamily;
            BTXaxis.FontSize = (double)fontsize;
            BTXaxis.Command = ReactiveCommand.Create(XAxisButtonClick);
            BTXaxis.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            BTXaxis.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(BTXaxis, 1);
            Grid.SetRow(BTXaxis, 0);
            ControlsGrid.Children.Add(BTXaxis);

            TextBox TBTitle = new TextBox();
            TBTitle.Watermark = language.FindElement("tbentertitle");
            TBTitle.FontFamily = fontfamily;
            TBTitle.FontSize = (double)fontsize;
            TBTitle.Focusable = true;
            TBTitle.KeyUp += TitleTextChanged;
            TBTitle.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            TBTitle.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(TBTitle, 0);
            Grid.SetRow(TBTitle, 1);
            Grid.SetColumnSpan(TBTitle, 2);
            ControlsGrid.Children.Add(TBTitle);

            int col = 2;
            int row = 0;
            foreach(var series in cg.pag.series)
            {
                Button BTY = new Button();
                BTY.Content = "y: " + series.name;
                BTY.FontFamily = fontfamily;
                BTY.FontSize = (double)fontsize;
                BTY.Command = ReactiveCommand.Create(() => { YAxisButtonClick(series); });
                BTY.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                BTY.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                Grid.SetColumn(BTY, col);
                Grid.SetRow(BTY, row);
                ControlsGrid.Children.Add(BTY);
                if (col == 7)
                {
                    col = 2;
                    row++;
                }
                else
                {
                    col++;
                }
                if (row == 2) throw new System.Exception("too many series to properly display");
            }
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }

    private void TitleTextChanged(object? sender, KeyEventArgs e)
    {
        if (sender is not null) {
            TextBox tb = (TextBox)sender;
            if (tb.Text is not null) cg.SetTitle(tb.Text); else ErrorWindow.Show(string.Format("error: variable is null.\ntb.Text={0}", tb.Text));
        }
        else
        {
            ErrorWindow.Show("error: sender is null for TitleTextChanged.");
        }
    }

    private async void OpenFileButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (cg.pag is null)
        {
            ErrorWindow.Show("error: pag is null -> settings file not found");
            return;
        }
        try
        {
            var toplevel = TopLevel.GetTopLevel(this);
            if (toplevel is null) return;
            var id = cg.pag.settings.FindValueFromKey("initialdirectory");
            if (id is null || id.Length == 0)
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
                    SuggestedStartLocation = await toplevel.StorageProvider.TryGetFolderFromPathAsync(new System.Uri(new System.Uri(System.AppDomain.CurrentDomain.BaseDirectory), id))
                });

                if (files is not null && files.Count >= 1)
                {
                    string file = files[0].Path.AbsolutePath;
                    TBFile.Text = file;
                    cg.SetFile(file);
                }
            }
        }
        catch (System.Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
    }

    private void OpenSettingsButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
    }

    private async void SaveFileButtonClick()
    {
        if (cg.pag is null)
        {
            ErrorWindow.Show("error: pag is null -> settings file not found");
            return;
        }
        try
        {
            var toplevel = TopLevel.GetTopLevel(this);
            if (toplevel is null) return;
            var id = cg.pag.settings.FindValueFromKey("initialdirectory");
            if (id is null || id.Length == 0)
            {
                var file = await toplevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save File",
                    FileTypeChoices = new[] { Images },
                    ShowOverwritePrompt = true,
                    DefaultExtension = ".png"
                });

                if (file is not null)
                {
                    PixelSize psize = new((int)MainCanvas.Width, (int)MainCanvas.Height);
                    Size size = new(MainCanvas.Width, MainCanvas.Height);
                    Vector dpi = new(96, 96);
                    RenderTargetBitmap rtb = new RenderTargetBitmap(psize, dpi);
                    MainCanvas.Measure(size);
                    MainCanvas.Arrange(new Rect(size));
                    rtb.Render(MainCanvas);
                    rtb.Save(file.Path.AbsolutePath);
                }
            }
            else
            {
                var file = await toplevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save File",
                    FileTypeChoices = new[] { Images },
                    ShowOverwritePrompt = true,
                    DefaultExtension = ".png",
                    SuggestedStartLocation = await toplevel.StorageProvider.TryGetFolderFromPathAsync(new System.Uri(new System.Uri(System.AppDomain.CurrentDomain.BaseDirectory), id))
                });

                if (file is not null)
                {
                    PixelSize psize = new((int)MainCanvas.Width, (int)MainCanvas.Height);
                    Size size = new(MainCanvas.Width, MainCanvas.Height);
                    Vector dpi = new(96, 96);
                    RenderTargetBitmap rtb = new RenderTargetBitmap(psize, dpi);
                    MainCanvas.Measure(size);
                    MainCanvas.Arrange(new Rect(size));
                    rtb.Render(MainCanvas);
                    rtb.Save(file.Path.AbsolutePath);
                }
            }
        }
        catch (System.Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
    }

    private void XAxisButtonClick()
    {
        SettingsWindow settings = new SettingsWindow(new XAxisSettingsView(cg));
        settings.Show();
    }

    private void YAxisButtonClick(PolyAxisGraphs_Backend.Series series)
    {
        SettingsWindow settings = new SettingsWindow(new YAxisSettingsView(series, cg));
        settings.Show();
    }

    private void Canvas_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        if (cg.gde is null) { TBPos.Text = "no data to display"; return; }
        if (cg.pag is null) { ErrorWindow.Show("error: pag is null -> settings file not found"); return; }
        else
        {
            var pointerpoint = e.GetCurrentPoint(MainCanvas);
            var canvaspoint = pointerpoint.Position;
            var data = cg.gde.TranslateChartPointToSeriesPoint(new PolyAxisGraphs_Backend.GraphDrawingElements.PointRange() { center = new PolyAxisGraphs_Backend.GraphDrawingElements.Point() { x = canvaspoint.X, y = canvaspoint.Y }, range = 10 });
            if(data is not null)
            {
                var dt = (PolyAxisGraphs_Backend.GraphDrawingElements.SeriesData)data;
                TBPos.Text = string.Format("SeriesPoint: {0} (x={1} [{3}]| y={2} [{0}])", dt.series.name, System.Math.Round(dt.seriespoint.x, dt.series.precision), System.Math.Round(dt.seriespoint.y, dt.series.precision), cg.pag.xaxisname);
            }
            else
            {
                TBPos.Text = "no seriespoint";
            }
        }
    }
}
