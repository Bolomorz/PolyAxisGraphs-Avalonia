using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PolyAxisGraphsAvalonia.Views
{
    public partial class YAxisSettingsView : UserControl
    {
        CanvasGraph cg { get; set; }
        PolyAxisGraphs_Backend.Series series { get; set; }

        const int heightfactor = 4;
        const int controlcount = 6;
        const int width = 600;

        public YAxisSettingsView(PolyAxisGraphs_Backend.Series _series, CanvasGraph _cg)
        {
            cg = _cg;
            series = _series;
            this.Width = width;
            this.Height = (cg.pag.settings.controlfontsize is null) ? 15 * heightfactor * controlcount : heightfactor * controlcount * (double)cg.pag.settings.controlfontsize;
            InitializeComponent();
            MainGrid.Width = width;
            MainGrid.Height = this.Height;
            LoadControls();
        }

        private void LoadControls()
        {
            if (cg.pag.settings.fontfamily is not null && cg.pag.settings.controlfontsize is not null && cg.pag.settings.currentlang is not null)
            {
                var ff = new Avalonia.Media.FontFamily(cg.pag.settings.fontfamily);
                var fs = cg.pag.settings.controlfontsize;

                var controlheight = this.Height / controlcount - 10;
                var controlwidth = this.Width / 4;

                tbltitle.FontFamily = ff;
                tbltitle.FontSize = (int)fs;
                tbltitle.Text = cg.pag.settings.currentlang.FindElement("tbtitlesettingsy");
                tbltitle.Width = controlwidth * 4 - 10;
                tbltitle.Height = controlheight;

                btfunc.FontFamily = ff;
                btfunc.FontSize = (int)fs;
                btfunc.Content = (series.rft == PolyAxisGraphs_Backend.Regression.FunctionType.NaF) ? cg.pag.settings.currentlang.FindElement("btfuncnotset") : cg.pag.settings.currentlang.FindElement("btfuncset");
                btfunc.Background = (series.rft == PolyAxisGraphs_Backend.Regression.FunctionType.NaF) ? Brushes.Red : Brushes.Green;

                tblname.FontFamily = ff;
                tblname.FontSize = (int)fs;
                tblname.Text = cg.pag.settings.currentlang.FindElement("tbnamesettingsy");
                tblname.Width = controlwidth - 10;
                tblname.Height = controlheight;
                tboname.FontFamily = ff;
                tboname.FontSize = (int)fs;
                tboname.Text = series.name;
                tboname.Width = controlwidth * 3 - 10;
                tboname.Height = controlheight;

                tblcolor.FontFamily = ff;
                tblcolor.FontSize = (int)fs;
                tblcolor.Text = cg.pag.settings.currentlang.FindElement("tbcolorsettingsy");
                tblcolor.Width = controlwidth - 10;
                tblcolor.Height = controlheight;
                tbocolor.FontFamily = ff;
                tbocolor.FontSize = (int)fs;
                tbocolor.Text = series.color.Name;
                tbocolor.Width = controlwidth * 3 - 10;
                tbocolor.Height = controlheight;

                tblmin.FontFamily = ff;
                tblmin.FontSize = (int)fs;
                tblmin.Text = cg.pag.settings.currentlang.FindElement("tbminvalue");
                tblmin.Width = controlwidth - 10;
                tblmin.Height = controlheight;
                tbomin.FontFamily = ff;
                tbomin.FontSize = (int)fs;
                tbomin.Text = series.setmin.ToString();
                tbomin.Width = controlwidth * 2 - 10;
                tbomin.Height = controlheight;
                btmin.FontFamily = ff;
                btmin.FontSize = (int)fs;
                btmin.Content = cg.pag.settings.currentlang.FindElement("btreset");

                tblmax.FontFamily = ff;
                tblmax.FontSize = (int)fs;
                tblmax.Text = cg.pag.settings.currentlang.FindElement("tbmaxvalue");
                tblmax.Width = controlwidth - 10;
                tblmax.Height = controlheight;
                tbomax.FontFamily = ff;
                tbomax.FontSize = (int)fs;
                tbomax.Text = series.setmax.ToString();
                tbomax.Width = controlwidth * 2 - 10;
                tbomax.Height = controlheight;
                btmax.FontFamily = ff;
                btmax.FontSize = (int)fs;
                btmax.Content = cg.pag.settings.currentlang.FindElement("btreset");

                btapply.FontFamily = ff;
                btapply.FontSize = (int)fs;
                btapply.Content = cg.pag.settings.currentlang.FindElement("btapply");
                btdiscard.FontFamily = ff;
                btdiscard.FontSize = (int)fs;
                btdiscard.Content = cg.pag.settings.currentlang.FindElement("btdiscard");
            }
        }

        private void ClickFunc(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(Parent is not null)
            {
                SettingsWindow sw = (SettingsWindow)Parent;
                sw.Content = new RegressionSettingsView(series, cg);
            }
        }

        private void ClickApply(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tboname.Text is not null) series.name = tboname.Text;
            if (tbomin.Text is not null) series.SetMin(PolyAxisGraphs_Backend.PolyAxisGraph.ReadStringToDouble(tbomin.Text));
            if (tbomax.Text is not null) series.SetMax(PolyAxisGraphs_Backend.PolyAxisGraph.ReadStringToDouble(tbomax.Text));
            if (tbocolor.Text is not null) series.color = System.Drawing.Color.FromName(tbocolor.Text);
            if (Parent is not null)
            {
                cg.ReDraw();
                SettingsWindow sw = (SettingsWindow)Parent;
                sw.Close();
            }
        }

        private void ClickDiscard(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Parent is not null)
            {
                SettingsWindow sw = (SettingsWindow)Parent;
                sw.Close();
            }
        }

        private void ClickResetMin(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            tbomin.Text = series.min.ToString();
            series.ResetMin();
        }

        private void ClickResetMax(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            tbomax.Text = series.max.ToString();
            series.ResetMax();
        }
    }
}
