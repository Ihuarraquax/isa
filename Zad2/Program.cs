using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using GeneticAlgorithmModule.Models;

namespace Zad2
{
// N dla zbioru np.: {30, 35,.., 75, 80},
// T dla zbioru np.: {50, 60,.., 140, 150},
// pk dla zbioru np.: {0,5, 0,55,.., 0,85, 0,9},
// pm dla zbioru np.: {0,0001, 0,0005,.., 0,005, 0,01},

    class Program
    {
        static void Main(string[] args)
        {
            var a = -4;
            var b = 12;
            var d = 0.001m;
            var eliteSize = 1;


            var RangeT = new[] {50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150};
            var RangeN = new[] {30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80};
            var RangePk = new[] {0.5m, 0.55m, 0.6m, 0.65m, 0.70m, 0.75m, 0.80m, 0.85m, 0.9m};
            var RangePm = new[] {0.0001m, 0.0005m, 0.001m, 0.002m, 0.003m, 0.004m, 0.005m, 0.006m, 0.007m, 0.008m, 0.009m, 0.01m};


            foreach (var t in RangeT)
            {
                foreach (var n in RangeN)
                {
                    foreach (var pk in RangePk)
                    {
                        foreach (var pm in RangePm)
                        {
                            var algorithResults = new List<decimal>();
                            for (int i = 0; i < 100; i++)
                            {
                                var algorithm = new GeneticAlgorithm(a, b, d, pk, pm, n, t, eliteSize);
                                algorithm.Run();
                                var result = algorithm.Result();
                                algorithResults.Add(result);
                            }

                            var resultDTO = new ResultDTO()
                            {
                                N = n,
                                T = t,
                                Pk = pk,
                                Pm = pm,
                                Favg = algorithResults.Average(),
                                Fmax = algorithResults.Max()
                            };
                            using var writer = new StreamWriter("zad2.csv", true);
                            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                            csv.WriteRecord(resultDTO);
                            csv.NextRecord();
                        }
                    }
                }
            }
        }
    }
}