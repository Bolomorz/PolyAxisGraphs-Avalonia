using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PolyAxisGraphs_Backend;

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
            int c = ColorTranslator.ToOle(color);
            uint uc = (uint)c;
            return new SolidColorBrush(uc);
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
            double col = 10;
            foreach(var series in pag.series)
            {
                DrawLine(new Avalonia.Point(10, col), new Avalonia.Point(100, col), ColorToBrush(series.color), 1);
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
                Text = text
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
