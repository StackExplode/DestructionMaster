using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesdructionMaster
{
    static class DestructionWarlock
    {
        private static readonly double[] PunishPoint = new double[] { 0, 30, 39, 47, 54, 66 };
        private static readonly double[] PunishV, PunishM;

        static DestructionWarlock()
        {
            PunishV = new double[PunishPoint.Length];
            PunishM = new double[PunishPoint.Length];
            PunishM[0] = PunishV[0] = 0;
            for (int i = 1; i < PunishPoint.Length; i++)
            {
                PunishV[i] = Versatility2Green(PunishPoint[i]);
                //PunishM[i] = PM2VM(PunishPoint[i] *2 +16);
                PunishM[i] = PunishV[i] / 40 * 35;
            }
        }

        public static double Mastery2Green(double m)
        {
            double vm = 0;
            m -= 16;    //毁灭自带16精通
            m /= 2;     //精通系数2

            if (m > 126)
                return 30 * 35 + 9 * 1.1 * 35 + 8 * 1.2 * 35 + 7 * 1.3 * 35 + 12 * 1.4 * 35 + 60 * 1.5 * 35;

            for (int i = 1; i < PunishPoint.Length; i++)
            {
                if (m <= PunishPoint[i])
                {
                    vm += (m - PunishPoint[i - 1]) * (1 + (i - 1) / 10.0) * 35;
                    break;
                }
                else
                {
                    vm += (PunishPoint[i] - PunishPoint[i - 1]) * (1 + (i - 1) / 10.0) * 35;
                }
            }

            return vm;
        }
        public static double Versatility2Green(double v)
        {
            double vm = 0, m = v;
            if (m >= 126)
                return 30 * 40 + 9 * 1.1 * 40 + 8 * 1.2 * 40 + 7 * 1.3 * 40 + 12 * 1.4 * 40 + 60 * 1.5 * 40;

            for (int i = 1; i < PunishPoint.Length; i++)
            {
                if (m <= PunishPoint[i])
                {
                    vm += (m - PunishPoint[i - 1]) * (1 + (i - 1) / 10.0) * 40;
                    break;
                }
                else
                {
                    vm += (PunishPoint[i] - PunishPoint[i - 1]) * (1 + (i - 1) / 10.0) * 40;
                }
            }

            return vm;
        }

        public static double Green2Versatility(double v)
        {
            if(v >= (30 * 40 + 9 * 1.1 * 40 + 8 * 1.2 * 40 + 7 * 1.3 * 40 + 12 * 1.4 * 40 + 60 * 1.5 * 40))
                return 126;
            double pv = 0, m = v;
            for (int i = 1; i < PunishV.Length; i++)
            {
                if (m <= PunishV[i])
                {
                    pv += (m - PunishV[i - 1]) / (1 + (i - 1) / 10.0) / 40;
                    break;
                }
                else
                {
                    pv += (PunishV[i] - PunishV[i - 1]) / (1 + (i - 1) / 10.0) / 40;
                }
            }

            return pv;
        }

        public static double Green2Mastery(double m)
        {
            if (m >= (30 * 35 + 9 * 1.1 * 35 + 8 * 1.2 * 35 + 7 * 1.3 * 35 + 12 * 1.4 * 35 + 60 * 1.5 * 35))
                return 126;
            double mv = 0;
            for (int i = 1; i < PunishM.Length; i++)
            {
                if (m <= PunishM[i])
                {
                    mv += (m - PunishM[i - 1]) / (1 + (i - 1) / 10.0) / 35;
                    break;
                }
                else
                {
                    mv += (PunishM[i] - PunishM[i - 1]) / (1 + (i - 1) / 10.0) / 35;
                }
            }
            mv *= 2;
            mv += 16;
            return mv;
        }

    }
}
