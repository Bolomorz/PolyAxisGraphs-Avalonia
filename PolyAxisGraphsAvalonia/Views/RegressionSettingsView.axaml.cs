using Avalonia.Controls;
using PolyAxisGraphs_Backend;

namespace PolyAxisGraphsAvalonia.Views
{
    public partial class RegressionSettingsView : UserControl
    {
        PolyAxisGraphs_Backend.Series series { get; set; }
        CanvasGraph cg { get; set; }

        const int heightfactor = 4;
        const int controlcount = 6;
        const int width = 600;

        public RegressionSettingsView(PolyAxisGraphs_Backend.Series _series, CanvasGraph _cg)
        {
            series = _series;
            cg = _cg;
            InitializeComponent();
        }
    }
}
