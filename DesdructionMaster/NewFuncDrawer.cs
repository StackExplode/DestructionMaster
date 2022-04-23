using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot.Axes;
using OxyPlot.WindowsForms;
using System.Drawing;

namespace DesdructionMaster
{


    public delegate void OnScaleChangedEvent(object sender, double delta_min, double delte_max);

    internal class DrawFuncData
    {
        public bool AutoRedraw = true;
        public Color color = Color.Red;
        public Func<double, double> Fun;
        public string Name = null;
        public double Step = 0.1;
    }
    class NewFuncDrawer
    {
        private List<DrawFuncData> _funcs = new List<DrawFuncData>();
        private OxyPlot.WindowsForms.PlotView _plot;

        public OxyPlot.WindowsForms.PlotView Plot { get { return _plot; } }
        public NewFuncDrawer(PlotView pl)
        {
            System.Drawing.StringFormat.GenericTypographic.FormatFlags &= ~StringFormatFlags.LineLimit;
            this._plot = pl;

            _plot.Model = new OxyPlot.PlotModel();
            _plot.Model.PlotType = OxyPlot.PlotType.XY;

            OxyPlot.Axes.LinearAxis ax = new LinearAxis();
            ax.Position = AxisPosition.Bottom;
            ax.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            ax.LabelFormatter = _formatter;
            ax.AxisChanged += Form1_AxisChanged;
            //ax.Title = "在";
            

            OxyPlot.Axes.LinearAxis ay = new LinearAxis();
            ay.Position = AxisPosition.Left;
            ay.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            ay.LabelFormatter = _formatter;
            //ay.Title = "因变量 y";

            _plot.Model.Axes.Add(ax);
            _plot.Model.Axes.Add(ay);
        }

        public event OnScaleChangedEvent OnScaleChanged;

        public void SetTitle(string tt)
        {
            _plot.Model.Title = tt;
        }

        public void SetAxesTitle(string x,string y)
        {
            _plot.Model.Axes[0].Title = x;
            _plot.Model.Axes[1].Title = y;
        }

        public void AppendFunction(Func<double, double> fun, double start, double end, double step, Color cl, string title = null)
        {
            var ff = new DrawFuncData();
            ff.Name = title;
            ff.Fun = fun;
            ff.Step = step;
            if (cl != null)
                ff.color = cl;
            ff.Step = step;
            var ser = new OxyPlot.Series.FunctionSeries(fun, start, end, (end - start) / step, title);
            if (cl != null)
                ser.Color = OxyPlot.OxyColor.FromRgb(cl.R, cl.G, cl.B);
            _plot.Model.Series.Add(ser);
            _funcs.Add(ff);
        }

        public void DrawFunction(Func<double, double> fun, double start, double end, double step, Color cl, string title = null)
        {
            RemoveFunctions();
            AppendFunction(fun, start, end, step, cl, title);
        }

        public void DrawXYLine(double[] data, Color cl)
        {
            RemoveFunctions();
            _funcs.Add(new DrawFuncData() { AutoRedraw = false });
            OxyPlot.Series.LineSeries ser = new OxyPlot.Series.LineSeries();
            ser.Color = cl.ToOxyColor();
            for (int i = 0; i < data.Length; i++)
                ser.Points.Add(new OxyPlot.DataPoint(i, data[i]));
            _plot.Model.Series.Add(ser);
            _plot.InvalidatePlot(true);
        }

        public void DrawXYLine(PointF[] pts, Color cl)
        {
            RemoveFunctions();
            _funcs.Add(new DrawFuncData() { AutoRedraw = false });
            OxyPlot.Series.LineSeries ser = new OxyPlot.Series.LineSeries();
            ser.Color = cl.ToOxyColor();
            for (int i = 0; i < pts.Length; i++)
                ser.Points.Add(new OxyPlot.DataPoint(pts[i].X, pts[i].Y));
            _plot.Model.Series.Add(ser);
            _plot.InvalidatePlot(true);
        }

        public void RemoveFunctions()
        {
            _plot.Model.Series.Clear();
            _funcs.Clear();
        }

        protected void ReDraw(int n)
        {
            double max = _plot.Model.Axes[0].ActualMaximum;
            double min = _plot.Model.Axes[0].ActualMinimum;
            //_plot.Model.Series.RemoveAt(n);
            double step = 0.1;
            if (_funcs[n].AutoRedraw)
                step = (max - min) / _funcs[n].Step;
            else
                step = _funcs[n].Step;
            var ser = new OxyPlot.Series.FunctionSeries(_funcs[n].Fun, min, max, step, _funcs[n].Name);
            Color cl = _funcs[n].color;
            ser.Color = OxyPlot.OxyColor.FromRgb(cl.R, cl.G, cl.B);
            _plot.Model.Series[n] = ser;
            _plot.InvalidatePlot(true);
        }

        private static string _formatter(double d1)
        {
            double d = Math.Abs(d1);
            if (d < 1 && d >=1e-3)
            {
                return String.Format("{0}m", d1 * 1E3);
            }
            else if (d < 1E-3 && d >= 1e-6)
            {
                return String.Format("{0}u", d1 * 1E6);
            }
            else if (d < 1E-6 && d >= 1e-9)
            {
                return String.Format("{0}n", d1 * 1E9);
            }
            else if (d >= 1E3 && d < 1E6)
            {
                return String.Format("{0}K", d1 / 1E3);
            }
            else if (d >= 1E6 && d < 1E9)
            {
                return String.Format("{0}M", d1 / 1E6);
            }
            else if (d >= 1E9)
            {
                return String.Format("{0}G", d1 / 1E9);
            }
            else
            {
                return String.Format("{0}", d1);
            }
        }
        private void Form1_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            for (int i = 0; i < _plot.Model.Series.Count;i++ )
            {
                if (_funcs[i].AutoRedraw)
                    ReDraw(i);
            }
            if(OnScaleChanged != null)
                this.OnScaleChanged(sender, e.DeltaMinimum, e.DeltaMaximum);
        }
    }
}
