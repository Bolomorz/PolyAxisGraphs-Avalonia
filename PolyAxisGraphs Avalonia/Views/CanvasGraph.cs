using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
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

        public void SetLanguage(string lngfile)
        {
            pag.SetLanguage(lngfile);
        }

        public void SetFile(string datafile)
        {
            pag.SetFilePath(datafile);
            pag.ReadData();
            canvas.Children.Clear();
        }
    }
}
