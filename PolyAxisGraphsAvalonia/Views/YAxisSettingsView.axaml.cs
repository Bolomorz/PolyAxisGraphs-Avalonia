using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
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
            if (cg.pag is null)
            {
                ErrorWindow.Show("error: pag is null -> settings file not found");
                return;
            }
            this.Width = width;
            var cfs = cg.pag.settings.FindValueFromKey("controlfontsize");
            this.Height = (cfs is null) ? 15 * heightfactor * controlcount : heightfactor * controlcount * PolyAxisGraphs_Backend.PolyAxisGraph.ReadStringToDouble(cfs);
            InitializeComponent();
            MainGrid.Width = width;
            MainGrid.Height = this.Height;
            LoadControls();
        }

        private void LoadControls()
        {
            if (cg.pag is null)
            {
                ErrorWindow.Show("error: pag is null -> settings file not found");
                return;
            }
            var sff = cg.pag.settings.FindValueFromKey("fontfamily");
            var scf = cg.pag.settings.FindValueFromKey("controlfontsize");
            if (sff is not null && scf is not null && cg.pag.settings.currentlang is not null)
            {
                var ff = new Avalonia.Media.FontFamily(sff);
                var fs = PolyAxisGraphs_Backend.PolyAxisGraph.ReadStringToDouble(scf);

                var controlheight = this.Height / controlcount - 10;
                var controlwidth = this.Width / 4;

                tbltitle.FontFamily = ff;
                tbltitle.FontSize = (int)fs;
                tbltitle.Text = cg.pag.settings.currentlang.FindElement("tbtitlesettingsy");
                tbltitle.Width = controlwidth * 4 - 10;
                tbltitle.Height = controlheight;

                btfunc.FontFamily = ff;
                btfunc.FontSize = (int)fs;
                btfunc.Content = (series.showfunction) ? cg.pag.settings.currentlang.FindElement("btfuncactive") : cg.pag.settings.currentlang.FindElement("btfuncnotactive");
                btfunc.Background = (series.showfunction) ? Brushes.Green : Brushes.Red;

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

                cbseries.FontFamily = ff;
                cbseries.FontSize = (int)fs;
                cbseries.Content = cg.pag.settings.currentlang.FindElement("cbseries");
                cbseries.IsChecked = (series.active) ? true : false;

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
            else
            {
                ErrorWindow.Show(string.Format("error: failed to load: settings variable is null.\nfontfamily={0}\ncontrolfontsize={1}\ncurrentlang={2}\nfilepath={3}",
                    cg.pag.settings.FindValueFromKey("fontfamily"), cg.pag.settings.FindValueFromKey("controlfontsize"), cg.pag.settings.currentlang));
            }
        }

        private void ClickFunc(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(Parent is not null)
            {
                try
                {
                    SettingsWindow sw = (SettingsWindow)Parent;
                    var view = new RegressionSettingsView(series, cg);
                    sw.Content = view;
                    sw.Width = view.Width;
                    sw.Height = view.Height;
                    sw.MaxWidth = view.Width;
                    sw.MaxHeight = view.Height;
                }
                catch (Exception ex)
                {
                    ErrorWindow.Show(ex.ToString());
                }
            }
            else
            {
                ErrorWindow.Show("error: parent window of view yaxissettingsview is null");
            }
        }

        private void ClickApply(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tboname.Text is not null) series.name = tboname.Text; else ErrorWindow.Show("error: tboname.Text of view yaxissettingsview is null");
            if (tbomin.Text is not null) series.SetMin(PolyAxisGraphs_Backend.PolyAxisGraph.ReadStringToDouble(tbomin.Text)); else ErrorWindow.Show("error: tbomin.Text of view yaxissettingsview is null");
            if (tbomax.Text is not null) series.SetMax(PolyAxisGraphs_Backend.PolyAxisGraph.ReadStringToDouble(tbomax.Text)); else ErrorWindow.Show("error: tbomax.Text of view yaxissettingsview is null");
            if (tbocolor.Text is not null) series.color = System.Drawing.Color.FromName(tbocolor.Text); else ErrorWindow.Show("error: tbocolor.Text of view yaxissettingsview is null");
            series.active = (cbseries.IsChecked is null) ? true : (bool)cbseries.IsChecked;
            if (Parent is not null)
            {
                try
                {
                    cg.ReDraw();
                    SettingsWindow sw = (SettingsWindow)Parent;
                    sw.Close();
                }
                catch (Exception ex)
                {
                    ErrorWindow.Show(ex.ToString());
                }
            }
            else
            {
                ErrorWindow.Show("error: parent window of view yaxissettingsview is null");
            }
        }

        private void ClickDiscard(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Parent is not null)
            {
                try
                {
                    SettingsWindow sw = (SettingsWindow)Parent;
                    sw.Close();
                }
                catch (Exception ex)
                {
                    ErrorWindow.Show(ex.ToString());
                }
            }
            else
            {
                ErrorWindow.Show("error: parent window of view yaxissettingsview is null");
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
