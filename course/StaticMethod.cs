using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course
{
    public static class StaticMethod
    {
        public static double Efficiency { get; set; }
        public static int OptionsCount { get; set; }
        public static int MonthCount { get; set; }
        public static int Result { get; set; }

        public static List<List<double>> Data { get; set; } = new List<List<double>>();
        public static List<double> Efficiencies { get; set; } = new List<double>();
        public static List<double> Risks { get; set; } = new List<double>();


        public static void Compute()
        {
            for (int i =0;i<OptionsCount;i++)
            {
                double eff = 0;
                double sum = 0;
                double risk = 0;

                double[] effs = new double[MonthCount];

                for(int j=0;j<MonthCount;j++)
                {
                    effs[j] = Data[i][j + MonthCount] / Data[i][j];
                    sum += effs[j];
                }

                eff = sum / MonthCount;

                sum = 0;
                for (int k=0;k<MonthCount;k++)
                {
                    sum += Math.Pow(effs[k] - eff, 2);
                }

                risk = sum / (MonthCount - 1);

                Risks.Add(risk);
                Efficiencies.Add(eff);
            }

            for (int i=0;i<Efficiencies.Count;i++)
            {
                if (Efficiencies[i]<Efficiency)
                {
                    Efficiencies[i] = double.MinValue;
                    Risks[i] = double.MinValue;
                }
            }

            double min = double.MaxValue;
            int result = 0;

            for(int i =0;i<Risks.Count;i++)
            {
                if (Risks[i] < min && Efficiencies[i]!=double.MinValue)
                {
                    min = Risks[i];
                    result = i;
                }
            }

            Result = result;
        }
    }
}
