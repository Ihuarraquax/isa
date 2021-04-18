using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberFormatManager.Services;

namespace GeoAlgorithmModule.Models
{

    public class GeoAlgorithm
    {    
        public readonly decimal Tau;
        public readonly int T;
        public readonly Random random;
        public NumberFormatService Manager;
        public List<IterationResult> IterationResults { get; set; }
        public int SelfBestT { get; set; }
        public decimal ResultFx => IterationResults[T - 1].FxBest;

        public decimal VBestX { get; set; }
        public decimal VBestFx { get; set; }
        public GeoAlgorithm(int a, int b, decimal d, decimal tau, int t)
        {
            Tau = tau;
            T = t;
            SelfBestT = t;
            
            random = new Random();
            Manager = new NumberFormatService(a, b, d, random);
        }
        
        public GeoAlgorithm(decimal tau, int t, Random random, NumberFormatService manager)
        {
            Tau = tau;
            T = t;
            SelfBestT = t;

            this.random = random;
            Manager = manager;
        }

        public void Run()
        {
            IterationResults = new List<IterationResult>();
            // generate first guy
            var firstGuy = Manager.RandomDecimal();
            VBestX = firstGuy;
            VBestFx = Manager.CalculateFx(VBestX);
            var firstGuyBin = Manager.RealToBin(firstGuy);
            
            var iterationGuyBin = firstGuyBin;
            for (int iteration = 0; iteration < T; iteration++)
            {
                // create guys with mutate one gene
                var newGuysArray = new string[Manager.L];
                for (int i = 0; i < Manager.L; i++)
                {
                    StringBuilder sb = new StringBuilder(iterationGuyBin);
                    sb[i] = sb[i] == '1' ? '0' : '1';
                    var newGuy = sb.ToString();
                    newGuysArray[i] = newGuy;
                }

                // rank
                var guysResult = new List<KeyValuePair<int,decimal>>();
                for (int i = 0; i < newGuysArray.Length; i++)
                {
                    guysResult.Add(new KeyValuePair<int, decimal>(i, Manager.CalculateFx(Manager.BinToReal(newGuysArray[i]))));
                }
            
                var sortedGuys = guysResult.OrderByDescending(_ => _.Value).ToList();
            
                // calculate propablity 1/r^tau

                var indexWithPropabilityArray = new double[Manager.L];
                var r = 1;
                foreach (var guy in sortedGuys)
                {
                    indexWithPropabilityArray[guy.Key] = 1.0 / Math.Pow(r++, (double)Tau);
                }

            
                // mutate genes with propablity
                StringBuilder mutatedFirstGuy = new StringBuilder(iterationGuyBin);
                for (int i = 0; i < Manager.L; i++)
                {
                    var R = random.NextDouble();
                    if (R < indexWithPropabilityArray[i])
                    {
                        mutatedFirstGuy[i] = mutatedFirstGuy[i] == '1' ? '0' : '1';
                    }
                }

                // new iteration with result
                var resultBin = mutatedFirstGuy.ToString();
                var resultFx = Manager.CalculateFx(resultBin);

                if (resultFx > VBestFx)
                {
                    VBestX = Manager.BinToReal(resultBin);
                    VBestFx = resultFx;
                    SelfBestT = iteration + 1;
                }


                var iterationResult = new IterationResult
                {
                    Iteration = iteration + 1,
                    X = Manager.BinToReal(resultBin),
                    Fx = Manager.CalculateFx(resultBin),
                    XBest = VBestX,
                    FxBest = VBestFx
                };
                
                IterationResults.Add(iterationResult);
                iterationGuyBin = resultBin;
            }
        }
    }

    public class IterationResult
    {
        public int Iteration { get; set; }
        public decimal X { get; set; }
        public decimal Fx { get; set; }
        
        public decimal XBest { get; set; }
        public decimal FxBest { get; set; }
    }
}