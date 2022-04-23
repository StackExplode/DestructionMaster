using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using static DesdructionMaster.DestructionWarlock;

namespace DesdructionMaster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double M = double.Parse(txt_m.Text);
            double V = double.Parse(txt_v.Text);
            double K = double.Parse(txt_k.Text);
            double n = double.Parse(txt_n.Text) / 100.0;
            double mas = M / 100.0, ver = V / 100.0;

            Func<double, double> fun = (x) =>
             {
                 if (x > M - 16)
                     x = M - 16;
                 if (x < -V * 40 / 35 * 2)
                     x = -V * 40 / 35 * 2;
                 
                 double k1 = 1 - x / 100.0 * K / (2 + mas * K);
                 double delta_green_m = Mastery2Green(M) - Mastery2Green(M - x);
                 double green_v_origin = Versatility2Green(V);
                 double v_after = Green2Versatility(green_v_origin + delta_green_m);
                 double k2 = (1 + v_after / 100.0) / (1 + ver);
                 double dmg = k2 * (k1 * (1 - n) + n) - 1;
                 return dmg * 100;
             };


            var dd = new NewFuncDrawer(this.plotView1);
            dd.SetTitle("情况1图像");
            dd.SetAxesTitle("匀出X%精通(面板最终数值)给全能","伤害提升%");
            dd.DrawFunction(fun, -100, M - 16, 2000, Color.Red);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double M = double.Parse(txt_m.Text);
            double V = double.Parse(txt_v.Text);
            double K = double.Parse(txt_k.Text);
            double dM = double.Parse(txt_dm.Text);
            double dV = double.Parse(txt_dv.Text);
            double mas = M / 100.0, ver = V / 100.0;
            double k1 = 1 + dM / 100.0 * K / (2 + mas * K);
            double k2 = (1 + (V + dV) / 100.0) / (1 + ver);

            Func<double, double> fun = (n) =>
             {
                 if (n < 0)
                     n = 0;
                 if (n > 100)
                     n = 100;

                 n /= 100.0;
                 double dmg = k2 * (k1 * (1 - n) + n) - 1;
                 return dmg * 100;
             };

            var dd = new NewFuncDrawer(this.plotView1);
            dd.SetTitle("情况2图像");
            dd.SetAxesTitle("不吃精通的技能总占比%", "伤害提升%");
            dd.DrawFunction(fun, 0, 100, 2000, Color.Red);

            double zero = (1 / k2 - k1) / (1 - k1) * 100;
            lbl_n0.Text = zero.ToString("#.##") + "%";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double M = double.Parse(txt_m.Text);
            double V = double.Parse(txt_v.Text);
            double dM = double.Parse(txt_dm.Text);

            if(M + dM < 16)
            {
                MessageBox.Show("你哪有那么多精通可以转给全能啊？！\r\n(提示：毁灭自带16%初始精通)");
                return;
            }

            double delta_green_m = Mastery2Green(M) - Mastery2Green(M + dM);
            double green_v_origin = Versatility2Green(V);
            double v_after = Green2Versatility(green_v_origin + delta_green_m);

            if(v_after < 0)
            {
                MessageBox.Show("你哪有那么多全能可以转给精通啊？！\r\n(提示：毁灭自带16%初始精通)");
                return;
            }

            txt_dv.Text = (v_after - V).ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double M = double.Parse(txt_m.Text);
            double V = double.Parse(txt_v.Text);
            double dV = double.Parse(txt_dv.Text);

            if(V + dV < 0)
            {
                MessageBox.Show("你哪有那么多全能可以转给精通啊？！\r\n(提示：毁灭自带16%初始精通)");
                return;
            }

            double delta_green_v = Versatility2Green(V) - Versatility2Green(V + dV);
            double green_m_origin = Mastery2Green(M);
            double m_after = Green2Mastery(green_m_origin + delta_green_v);

            if (m_after < 16)
            {
                MessageBox.Show("你哪有那么多精通可以转给全能啊？！\r\n(提示：毁灭自带16%初始精通)");
                return;
            }

            txt_dm.Text = (m_after - M).ToString();
        }
    }
}
