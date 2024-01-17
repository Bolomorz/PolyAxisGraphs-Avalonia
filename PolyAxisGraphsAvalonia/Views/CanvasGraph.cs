using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PolyAxisGraphs_Backend;
using System.Diagnostics;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;

namespace PolyAxisGraphsAvalonia.Views
{
    public class CanvasGraph
    {
        public PolyAxisGraph? pag { get; set; }
        Canvas canvas { get; set; }
        public GraphDrawingElements? gde { get; set; }
        MainView mw { get; set; }

        public CanvasGraph(Canvas _canvas, MainView _mw)
        {
            canvas = _canvas;
            try
            {
                pag = new PolyAxisGraph(new Settings(@"..\..\..\Settings.ini"));
            }
            catch (Exception ex)
            {
                ErrorWindow.Show(ex.ToString());
            }
            gde = null;
            mw = _mw;
        }

        public static Avalonia.Media.ISolidColorBrush ColorToBrush(System.Drawing.Color color)
        {
            Avalonia.Media.Color amcolor = Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            return new SolidColorBrush(amcolor);
        }

        public void SetTitle(string title)
        {
            try
            {
                if (pag is null)
                {
                    ErrorWindow.Show("error: pag is null -> probably settings file not found");
                    return;
                }
                pag.charttitle = title;
                var c = (TextBlock)canvas.Children[0];
                c.Text = title;
            }
            catch (Exception ex)
            {
                ErrorWindow.Show(ex.ToString());
            }
        }

        public void SetLanguage(string lngfile)
        {
            try
            {
                if (pag is null)
                {
                    ErrorWindow.Show("error: pag is null -> probably settings file not found");
                    return;
                }
                pag.SetLanguage(lngfile);
            }
            catch (Exception ex)
            {
                ErrorWindow.Show(ex.ToString());
            }
        }

        public void SetFile(string datafile)
        {
            try
            {
                if (pag is null)
                {
                    ErrorWindow.Show("error: pag is null -> probably settings file not found");
                    return;
                }
                pag.SetFilePath(datafile);
                pag.ReadData();
                gde = new GraphDrawingElements(canvas.Width, canvas.Height, pag);
                DrawGDE();
            }
            catch (Exception ex)
            {
                ErrorWindow.Show(ex.ToString());
            }
        }

        public void ReDraw()
        {
            try
            {
                gde = new GraphDrawingElements(canvas.Width, canvas.Height, pag);
                DrawGDE();
            }
            catch (Exception ex)
            {
                ErrorWindow.Show(ex.ToString());
            }
        }

        private void DrawGDE()
        {
            if (gde is null || pag is null) return;
            canvas.Children.Clear();
            FontFamily ff = (pag.settings.fontfamily is null) ? new FontFamily("Consolas") : new FontFamily(pag.settings.fontfamily);
            double fontsize = (pag.settings.chartfontsize is null) ? 10 : (double)pag.settings.chartfontsize;
            var sol = gde.CalculateChart();
            if (sol.err is not null) DrawText(10, ff, sol.err, 0, 0, canvas.Width, canvas.Height);
            else
            {
                if (sol.texts is not null)
                {
                    foreach (var text in sol.texts) DrawText(text.fontsize, ff, text.text, text.left, text.top, text.right, text.bottom);
                }
                if (sol.lines is not null)
                {
                    foreach (var line in sol.lines) DrawLine(new Avalonia.Point(line.start.x, line.start.y), new Avalonia.Point(line.end.x, line.end.y), ColorToBrush(line.color), line.thickness);
                }
                if (sol.functions is not null && sol.functions.Count > 0)
                {
                    double left, top, height, width, right, bottom;
                    if(sol.functionarea is null)
                    {
                        left = 0.91 * canvas.Width;
                        top = 0.11 * canvas.Height;
                        right = 0.99 * canvas.Width;
                        bottom = 0.95 * canvas.Height;
                        height = bottom - top;
                        width = right - left;
                    }
                    else
                    {
                        var area = (GraphDrawingElements.Rectangle)sol.functionarea;
                        left = area.left;
                        top = area.top;
                        right = area.right;
                        bottom = area.bottom;
                        height = area.height;
                        width = area.width;
                    }
                    double intervall = height / sol.functions.Count;
                    foreach (var function in sol.functions)
                    {
                        if (function is not null) DrawFunctionText(function, fontsize, ff, left, top, top + intervall, right);
                        top += intervall;
                    }
                }
            }
            mw.CreateControls();
        }

        private void DrawLine(Avalonia.Point start, Avalonia.Point end, Avalonia.Media.ISolidColorBrush brush, double thickness)
        {
            Line line = new Line()
            {
                StartPoint = start,
                EndPoint = end,
                Stroke = brush,
                StrokeThickness = thickness
            };
            canvas.Children.Add(line);
        }

        private void DrawText(double fontsize, Avalonia.Media.FontFamily fontFamily, string text, double left, double top, double right, double bottom)
        {
            TextBlock tb = new()
            {
                FontSize = fontsize,
                FontFamily = fontFamily,
                Text = text,
                Width = right - left,
                Height = bottom - top,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
            };
            Canvas.SetLeft(tb, left);
            Canvas.SetTop(tb, top);
            Canvas.SetRight(tb, right);
            Canvas.SetBottom(tb, bottom);
            canvas.Children.Add(tb);
        }

        private void DrawFunctionText(List<Series.FunctionString> strings, double fontsize, FontFamily fontFamily, double left, double top, double bottom, double right)
        {
            var color = ColorToBrush(strings[0].fcolor);
            TextBlock outer = new()
            {
                FontSize = fontsize,
                Foreground = color,
                FontFamily = fontFamily,
                Width = right - left,
                Height = bottom - top,
                TextWrapping = TextWrapping.WrapWithOverflow
            };
            outer.Inlines = new InlineCollection();
            foreach(var fs in strings)
            {
                if (fs.superscript)
                {
                    Run run = new()
                    {
                        FontSize = fontsize * 2/3,
                        FontFamily = fontFamily,
                        BaselineAlignment = BaselineAlignment.Top,
                        Text = fs.function
                    };
                    outer.Inlines.Add(run);
                }
                else
                {
                    Run run = new()
                    {
                        FontSize = fontsize,
                        FontFamily = fontFamily,
                        BaselineAlignment = BaselineAlignment.Center,
                        Text = fs.function
                    };
                    outer.Inlines.Add(run);
                }
            }
            Canvas.SetLeft(outer, left);
            Canvas.SetTop(outer, top);
            Canvas.SetBottom(outer, bottom);
            Canvas.SetRight(outer, right);
            canvas.Children.Add(outer);
        }

        private void DrawEllipse(double width, double height, Avalonia.Media.ISolidColorBrush fill, Avalonia.Media.ISolidColorBrush stroke, double thickness, double left, double top)
        {
            Ellipse ellipse = new()
            {
                Width = width,
                Height = height,
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = thickness
            };
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);
            canvas.Children.Add(ellipse);
        }

        private void DrawRectangle(double width, double height, Avalonia.Media.ISolidColorBrush fill, Avalonia.Media.ISolidColorBrush stroke, double thickness, double left, double top)
        {
            Avalonia.Controls.Shapes.Rectangle rectangle = new()
            {
                Width= width,
                Height= height,
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = thickness
            };
            Canvas.SetLeft(rectangle, left);
            Canvas.SetTop(rectangle, top);
            canvas.Children.Add(rectangle);
        }
    }
}
