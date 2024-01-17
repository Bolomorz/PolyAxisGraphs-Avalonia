using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using PolyAxisGraphs_Backend;

namespace PolyAxisGraphsAvalonia.Views
{
    public partial class RegressionSettingsView : UserControl
    {
        PolyAxisGraphs_Backend.Series series { get; set; }
        CanvasGraph cg { get; set; }

        const int heightfactor = 2;
        const int controlcount = 19;
        const int width = 600;

        public RegressionSettingsView(PolyAxisGraphs_Backend.Series _series, CanvasGraph _cg)
        {
            series = _series;
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

                var controlheight = this.Height / controlcount;
                var controlwidth = this.Width / 4;

                tbltitle.FontFamily = ff;
                tbltitle.FontSize = (int)fs;
                tbltitle.Text = cg.pag.settings.currentlang.FindElement("tbtitlesettingsfunc") + " " + series.name;
                tbltitle.Height = controlheight - 10;
                tbltitle.Width = controlwidth * 3 - 10;

                btreturn.FontFamily = ff;
                btreturn.FontSize = (int)fs;
                btreturn.Content = cg.pag.settings.currentlang.FindElement("btreturn");

                btcreate.FontFamily = ff;
                btcreate.FontSize = (int)fs;
                btcreate.Content = cg.pag.settings.currentlang.FindElement("btcalc");

                btapplyprecision.FontFamily = ff;
                btapplyprecision.FontSize = (int)fs;
                btapplyprecision.Content = cg.pag.settings.currentlang.FindElement("btapply");

                tbltitlecurrent.FontFamily = ff;
                tbltitlecurrent.FontSize = (int)fs;
                tbltitlecurrent.Text = cg.pag.settings.currentlang.FindElement("tbtitlesettingscurrent");
                tbltitlecurrent.Height = controlheight - 10;
                tbltitlecurrent.Width = controlwidth * 4 - 10;

                tbltitlesettings.FontFamily = ff;
                tbltitlesettings.FontSize = (int)fs;
                tbltitlesettings.Text = cg.pag.settings.currentlang.FindElement("tbtitlesettingss");
                tbltitlesettings.Height = controlheight - 10;
                tbltitlesettings.Width = controlwidth * 4 - 10;

                tbltitlecreate.FontFamily = ff;
                tbltitlecreate.FontSize = (int)fs;
                tbltitlecreate.Text = cg.pag.settings.currentlang.FindElement("tbtitlesettingscreate");
                tbltitlecreate.Height = controlheight - 10;
                tbltitlecreate.Width = controlwidth * 4 - 10;

                tblfunction.FontFamily = ff;
                tblfunction.FontSize = (int)fs;
                tblfunction.Text = cg.pag.settings.currentlang.FindElement("tbfunction");
                tblfunction.Height = controlheight * 4 - 10;
                tblfunction.Width = controlwidth - 10;

                tbltype.FontFamily = ff;
                tbltype.FontSize = (int)fs;
                tbltype.Text = cg.pag.settings.currentlang.FindElement("tbftype");
                tbltype.Height = controlheight * 2 - 10;
                tbltype.Width = controlwidth - 10;

                tblprecision.FontFamily = ff;
                tblprecision.FontSize = (int)fs;
                tblprecision.Text = cg.pag.settings.currentlang.FindElement("tbprecision");
                tblprecision.Height = controlheight * 2 - 10;
                tblprecision.Width = controlwidth - 10;

                tblorder.FontFamily = ff;
                tblorder.FontSize = (int)fs;
                tblorder.Text = cg.pag.settings.currentlang.FindElement("tborder");
                tblorder.Height = controlheight * 2 - 10;
                tblorder.Width = controlwidth - 10;

                tblselecttype.FontFamily = ff;
                tblselecttype.FontSize = (int)fs;
                tblselecttype.Text = cg.pag.settings.currentlang.FindElement("tbselect");
                tblselecttype.Height = controlheight * 2 - 10;
                tblselecttype.Width = controlwidth - 10;

                tbofunction.FontFamily = ff;
                tbofunction.FontSize = (int)fs;
                tbofunction.Inlines = new Avalonia.Controls.Documents.InlineCollection();
                tbofunction.Inlines.Clear();
                var strings = series.GetFunction();
                foreach (var functionString in strings)
                {
                    if (functionString.superscript)
                    {
                        Run run = new()
                        {
                            FontSize = (int)fs * 2 / 3,
                            FontFamily = ff,
                            BaselineAlignment = BaselineAlignment.Top,
                            Text = functionString.function
                        };
                        tbofunction.Inlines.Add(run);
                    }
                    else
                    {
                        Run run = new()
                        {
                            FontSize = (int)fs,
                            FontFamily = ff,
                            BaselineAlignment = BaselineAlignment.Center,
                            Text = functionString.function
                        };
                        tbofunction.Inlines.Add(run);
                    }
                }
                tbofunction.Width = controlwidth * 3 - 10;
                tbofunction.Height = controlheight * 4 - 10;

                tboorder.FontFamily = ff;
                tboorder.FontSize = (int)fs;
                tboorder.Text = "2";
                tboorder.Height = controlheight * 2 - 10;
                tboorder.Width = controlwidth * 3 - 10;

                tboprecision.FontFamily = ff;
                tboprecision.FontSize = (int)fs;
                tboprecision.Text = series.precision.ToString();
                tboprecision.Height = controlheight * 2 - 10;
                tboprecision.Width = controlwidth * 2 - 10;

                tbotype.FontFamily = ff;
                tbotype.FontSize = (int)fs;
                tbotype.Text = series.rft.ToString();
                tbotype.Height = controlheight * 2 - 10;
                tbotype.Width = controlwidth * 2 - 10;

                cbactive.FontFamily = ff;
                cbactive.FontSize = (int)fs;
                cbactive.Content = cg.pag.settings.currentlang.FindElement("cbfunc");
                cbactive.IsChecked = series.showfunction;

                lbselecttype.FontFamily = ff;
                lbselecttype.FontSize = (int)fs;
                lbselecttype.Items.Clear();
                lbselecttype.Items.Add(PolyAxisGraphs_Backend.Regression.FunctionType.NaF);
                lbselecttype.Items.Add(PolyAxisGraphs_Backend.Regression.FunctionType.Line);
                lbselecttype.Items.Add(PolyAxisGraphs_Backend.Regression.FunctionType.Logarithm);
                lbselecttype.Items.Add(PolyAxisGraphs_Backend.Regression.FunctionType.Polynomial);
                lbselecttype.Items.Add(PolyAxisGraphs_Backend.Regression.FunctionType.Power);
                lbselecttype.Items.Add(PolyAxisGraphs_Backend.Regression.FunctionType.Exponential);
                lbselecttype.SelectedIndex = 0;
                lbselecttype.Height = controlheight * 2 - 10;
                lbselecttype.Width = controlwidth * 3 - 10;
            }
        }

        private void CheckBoxIsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(series.rft == Regression.FunctionType.NaF) series.showfunction = false;
            if (cbactive.IsChecked is null) series.showfunction = false;
            else series.showfunction = (bool)cbactive.IsChecked;
        }

        private void ClickReturn(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Parent is not null)
            {
                SettingsWindow sw = (SettingsWindow)Parent;
                var view = new YAxisSettingsView(series, cg);
                sw.Content = view;
                sw.Width = view.Width;
                sw.Height = view.Height;
            }
        }

        private void ClickApplyPrecision(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(tboprecision.Text is not null) series.precision = PolyAxisGraph.ReadStringToInt(tboprecision.Text);
            LoadControls();
        }

        private void ClickCalculate(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(tboorder.Text is not null && lbselecttype.SelectedItem is not null)
            {
                var order = PolyAxisGraph.ReadStringToInt(tboorder.Text);
                var type = (Regression.FunctionType)lbselecttype.SelectedItem;
                cg.pag.CalculateRegression(series, type, order);
                series.showfunction = true;
                LoadControls();
                cg.ReDraw();
            }
        }
    }
}
