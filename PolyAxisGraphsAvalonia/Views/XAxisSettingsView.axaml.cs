using Avalonia.Controls;

namespace PolyAxisGraphsAvalonia.Views
{
    public partial class XAxisSettingsView : UserControl
    {
        CanvasGraph cg { get; set; }

        const int heightfactor = 4;
        const int controlcount = 5;
        const int width = 600;

        public XAxisSettingsView(CanvasGraph _cg)
        {
            cg = _cg;
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
                tbltitle.Text = cg.pag.settings.currentlang.FindElement("tbtitlesettingsx");
                tbltitle.Width = controlwidth * 4 - 10;
                tbltitle.Height = controlheight;

                tblname.FontFamily = ff;
                tblname.FontSize = (int)fs;
                tblname.Text = cg.pag.settings.currentlang.FindElement("tbnamesettingsx");
                tblname.Width = controlwidth - 10;
                tblname.Height = controlheight;
                tboname.FontFamily = ff;
                tboname.FontSize = (int)fs;
                tboname.Text = cg.pag.xaxisname;
                tboname.Width = controlwidth * 3 - 10;
                tboname.Height = controlheight;

                tblmin.FontFamily = ff;
                tblmin.FontSize = (int)fs;
                tblmin.Text = cg.pag.settings.currentlang.FindElement("tbminvalue");
                tblmin.Width = controlwidth - 10;
                tblmin.Height = controlheight;
                tbomin.FontFamily = ff;
                tbomin.FontSize = (int)fs;
                tbomin.Text = cg.pag.x1.ToString();
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
                tbomax.Text = cg.pag.x2.ToString();
                tbomax.Width= controlwidth * 2 - 10;
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

        private void ClickApply(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tboname.Text is not null) cg.pag.xaxisname = tboname.Text;
            if (tbomin.Text is not null) cg.pag.x1 = PolyAxisGraphs_Backend.PolyAxisGraph.ReadStringToInt(tbomin.Text);
            if (tbomax.Text is not null) cg.pag.x2 = PolyAxisGraphs_Backend.PolyAxisGraph.ReadStringToInt(tbomax.Text);
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
            tbomin.Text = cg.pag.defx1.ToString();
        }

        private void ClickResetMax(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            tbomax.Text = cg.pag.defx2.ToString();
        }
    }
}
