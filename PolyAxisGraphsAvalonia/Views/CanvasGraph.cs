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

namespace PolyAxisGraphs_Avalonia.Views
{
    internal class CanvasGraph
    {
        public PolyAxisGraph pag { get; set; }
        Canvas canvas { get; set; }

        public CanvasGraph(Canvas _canvas)
        {
            canvas = _canvas;
            pag = new PolyAxisGraph(new Settings(@"..\..\..\Settings.txt"));
        }

        public static Avalonia.Media.ISolidColorBrush ColorToBrush(System.Drawing.Color color)
        {
            Avalonia.Media.Color amcolor = Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            return new SolidColorBrush(amcolor);
        }

        public void SetLanguage(string lngfile)
        {
            pag.SetLanguage(lngfile);
        }

        public void SetFile(string datafile)
        {
            pag.SetFilePath(datafile);
            pag.ReadData();
            canvas.Children.Clear();
            FontFamily ff = (pag.settings.fontfamily is null) ? new FontFamily("Consolas") : new FontFamily(pag.settings.fontfamily);
            Debug.WriteLine("create gde... {0} {1}", canvas.Width, canvas.Height);
            GraphDrawingElements gde = new GraphDrawingElements(canvas.Width, canvas.Height, pag, pag.settings);
            Debug.WriteLine("calc gde...");
            var sol = gde.CalculateChart();
            Debug.WriteLine("sol gde...");
            if (sol.err is not null) DrawText(10, ff, sol.err, 10, 10);
            else
            {
                if(sol.chartarea is not null)
                {
                    var area = (GraphDrawingElements.Rectangle)sol.chartarea;
                    DrawRectangle(area.width, area.height, Brushes.Transparent, Brushes.Black, 1, area.left, area.top);
                }
                if(sol.datearea is not null)
                {
                    var area = (GraphDrawingElements.Rectangle)sol.datearea;
                    DrawRectangle(area.width, area.height, Brushes.Transparent, Brushes.Black, 1, area.left, area.top);
                }
                if(sol.functionarea is not null)
                {
                    var area = (GraphDrawingElements.Rectangle)sol.functionarea;
                    DrawRectangle(area.width, area.height, Brushes.Transparent, Brushes.Black, 1, area.left, area.top);
                }
                if(sol.legendarea is not null)
                {
                    var area = (GraphDrawingElements.Rectangle)sol.legendarea;
                    DrawRectangle(area.width, area.height, Brushes.Transparent, Brushes.Black, 1, area.left, area.top);
                }
                if(sol.titlearea is not null)
                {
                    var area = (GraphDrawingElements.Rectangle)sol.titlearea;
                    DrawRectangle(area.width, area.height, Brushes.Transparent, Brushes.Black, 1, area.left, area.top);
                }
                if(sol.yaxisarea is not null)
                {
                    var area = (GraphDrawingElements.Rectangle)sol.yaxisarea;
                    DrawRectangle(area.width, area.height, Brushes.Transparent, Brushes.Black, 1, area.left, area.top);
                }
                if(sol.texts is not null)
                {
                    foreach(var text in sol.texts) DrawText(text.fontsize, ff, text.text, text.left, text.top);
                }
                if(sol.lines is not null)
                {
                    foreach (var line in sol.lines) DrawLine(new Avalonia.Point(line.start.x, line.start.y), new Avalonia.Point(line.end.x, line.end.y), ColorToBrush(line.color), line.thickness);
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

        private void DrawText(double fontsize, Avalonia.Media.FontFamily fontFamily, string text, double left, double top)
        {
            TextBlock tb = new()
            {
                FontSize = fontsize,
                FontFamily = fontFamily,
                Text = text,
                TextAlignment = TextAlignment.Center,
            };
            Canvas.SetLeft(tb, left);
            Canvas.SetTop(tb, top);
            canvas.Children.Add(tb);
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
