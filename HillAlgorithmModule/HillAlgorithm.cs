using NumberFormatManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HillAlgorithmModule
{
    public class HillAlgorithmIter
    {
        public HillAlgorithmIter(int iter)
        {
            Iter = iter;
            Vcs = new List<KeyValuePair<string, decimal>>();
        }

        public int Iter { get; set; }
        public List<KeyValuePair<string, decimal>> Vcs { get; set; }
    }

    public class HillAlgorithm
    {
        public readonly int T;
        public readonly Random random;
        public NumberFormatService Manager;
        public List<HillAlgorithmIter> Result { get; set; }
        public int IterationWithBesta { get; set; }
        public string UberBesta => "11101111111110";
        public decimal BestaFx => Result.SelectMany(_ => _.Vcs).OrderByDescending(_ => _.Value).FirstOrDefault().Value;
        public string BestaBin => Result.SelectMany(_ => _.Vcs).OrderByDescending(_ => _.Value).FirstOrDefault().Key;

        public HillAlgorithm(int a, int b, decimal d, int t)
        {
            T = t;
            random = new Random();
            Manager = new NumberFormatService(a, b, d, random);
            Result = new List<HillAlgorithmIter>();
        }

        public HillAlgorithm(int t, Random random, NumberFormatService manager)
        {
            T = t;

            this.random = random;
            Manager = manager;
            Result = new List<HillAlgorithmIter>();
        }

        public void Run()
        {
            var superBreak = false;
            for (int i = 0; i < T; i++)
            {
                if (superBreak)
                {
                    break;
                }
                var iter = new HillAlgorithmIter(i + 1);
                var vc = Manager.RandomDecimal();
                var vcBin = Manager.RealToBin(vc);
                var vcValue = Manager.CalculateFx(vc);
                iter.Vcs.Add(new KeyValuePair<string, decimal>(vcBin, vcValue));
                var localGit = true;
                while (localGit)
                {
                    var list = new List<KeyValuePair<string, decimal>>();
                    for (int j = 0; j < Manager.L; j++)
                    {
                        var sb = new StringBuilder(vcBin);
                        sb[j] = sb[j] == '1' ? '0' : '1';
                        list.Add(new KeyValuePair<string, decimal>(sb.ToString(), Manager.CalculateFx(sb.ToString())));
                    }

                    var Vn = list.OrderByDescending(_ => _.Value).FirstOrDefault();
                    
                    if (vcValue < Vn.Value)
                    {
                        vcBin = Vn.Key;
                        vcValue = Vn.Value;
                        iter.Vcs.Add(Vn);
                        if (vcBin == UberBesta)
                        {
                            superBreak = true;
                        }
                    }
                    else
                    {
                        localGit = false;
                    }
                    
                }
                Result.Add(iter);
            }
        }
    }
}