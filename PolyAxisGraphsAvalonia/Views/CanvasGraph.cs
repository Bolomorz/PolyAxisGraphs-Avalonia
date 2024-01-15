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

namespace PolyAxisGraphsAvalonia.Views
{
    public class CanvasGraph
    {
        public PolyAxisGraph pag { get; set; }
        Canvas canvas { get; set; }
        public GraphDrawingElements? gde { get; set; }

        public CanvasGraph(Canvas _canvas)
        {
            canvas = _canvas;
            pag = new PolyAxisGraph(new Settings(@"..\..\..\Settings.txt"));
            gde = null;
        }

        public static Avalonia.Media.ISolidColorBrush ColorToBrush(System.Drawing.Color color)
        {
            Avalonia.Media.Color amcolor = Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            return new SolidColorBrush(amcolor);
        }

        public void SetTitle(string title)
        {
            pag.charttitle = title;
            gde = new GraphDrawingElements(canvas.Width, canvas.Height, pag);
            DrawGDE();
        }

        public void SetLanguage(string lngfile)
        {
            pag.SetLanguage(lngfile);
        }

        public void SetFile(string datafile)
        {
            pag.SetFilePath(datafile);
            pag.ReadData();
            gde = new GraphDrawingElements(canvas.Width, canvas.Height, pag);
            DrawGDE();
        }

        public void DrawGDE()
        {
            if (gde is null) return;
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
                    double left, top, height, width;
                    if(sol.functionarea is null)
                    {
                        left = 0.91 * canvas.Width;
                        top = 0.11 * canvas.Height;
                        height = 0.95 * canvas.Height - 0.11 * canvas.Height;
                        width = 0.99 * canvas.Width - 0.91 * canvas.Width;
                    }
                    else
                    {
                        var area = (GraphDrawingElements.Rectangle)sol.functionarea;
                        left = area.left;
                        top = area.top;
                        height = area.height;
                        width = area.width;
                    }
                    double intervall = height / sol.functions.Count;
                    int count = 0;
                    foreach (var function in sol.functions)
                    {
                        if (function is not null) DrawFunctionText(function, fontsize, ff, left, top, top + count * intervall, left + width);
                        count++;
                    }
                }
            }
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

        private void DrawFunctionText(List<Series.FunctionString> strings,double fontsize, FontFamily fontFamily, double left, double top, double bottom, double right)
        {
            var color = ColorToBrush(strings[0].fcolor);
            TextBlock outer = new()
            {
                FontSize = fontsize,
                Foreground = color,
                FontFamily = fontFamily,
                Width = right - left,
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
