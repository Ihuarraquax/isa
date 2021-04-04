using System;
using System.Linq;
using NumberFormatManager.Services;

namespace GeneticAlgorithmModule.Models
{
    public class GeneticAlgorithm
    {
        public readonly int N;
        public readonly int T;
        public readonly decimal Pk;
        public readonly decimal Pm;
        public readonly int EliteSize;
        private readonly Random _random;

        public NumberFormatService Manager;
        public Generation[] Generations { get; set; }

        public GeneticAlgorithm(int a, int b, decimal d, decimal pk, decimal pm, int n, int t, int eliteSize)
        {
            Pk = pk;
            Pm = pm;
            N = n;
            T = t;
            EliteSize = eliteSize;
            _random = new Random();


            Manager = new NumberFormatService(a, b, d, _random);

            Generations = new Generation[T];
            Generations[0] = new Generation(Manager, Pk, Pm, N, _random);
            Generations[0].GenerateInitialPopulationCalculateFxAndBin();
        }

        public void Run()
        {
            for (int i = 1; i < Generations.Length; i++)
            {
                Generations[i - 1].CalculateNewPopulationProperties();
                Generations[i] = Generation.From(Generations[i - 1]);

                if (EliteSize > 0)
                {
                    EliteStrategy(i);
                }
            }
        }

        private void EliteStrategy(int i)
        {
            var elite = Generations[i - 1].Population.OrderByDescending(_ => _.Fx).Take(EliteSize)
                .Select(_ => new {_.X, _.Fx}).ToArray();
            var missedElites = elite.Where(e => Generations[i - 1].Population.All(p => p.FinalX != e.X)).ToList();

            var j = 0;
            while (missedElites.Any())
            {
                var missedElite = missedElites[0];
                missedElites.RemoveAt(0);
                var injected = false;

                while (injected == false && j < N)
                {
                    if (elite.All(e => e.X != Generations[i].Population[j].X))
                    {
                        Generations[i].Population[j].X = missedElite.X;
                        Generations[i].Population[j].Fx = missedElite.Fx;
                        injected = true;
                    }

                    j++;
                }
            }
        }

        public Generation GetFinalGeneration()
        {
            return Generations[T - 1];
        }

        public decimal Result()
        {
            return GetFinalGeneration().Population.Max(_ => _.Fx);
        }
    }
}