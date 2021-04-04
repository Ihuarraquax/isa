using System;
using GeneticAlgorithmModule.Models;

namespace GeneticAlgorithmBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var algorithm = new GeneticAlgorithm(-4, 12, 0.001m, 0.9m, 0.01m, 80, 150, 1);
            algorithm.Run();
            var result = algorithm.Result();
        }
    }
}