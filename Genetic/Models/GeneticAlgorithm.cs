using System;
using System.Collections.Generic;
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

        public NumberFormatService Manager;
        public List<Generation> Generations { get; set; }

        public GeneticAlgorithm(int a, int b, decimal d, decimal pk, decimal pm, int n, int t, int eliteSize)
        {
            Pk = pk;
            Pm = pm;
            N = n;
            T = t;
            EliteSize = eliteSize;
            var random = new Random();


            Manager = new NumberFormatService(a, b, d, random);

            Generations = new List<Generation>();
            var initialGeneration = new Generation(Manager, Pk, Pm, N, random);
            Generations.Add(initialGeneration);
            initialGeneration.GenerateInitialPopulationCalculateFxAndBin();
        }

        public void Run()
        {
            var previousGeneration = Generations[0];
            for (int i = 1; i < T; i++)
            {
                previousGeneration.CalculateNewPopulationProperties();
                var newGeneration = Generation.From(previousGeneration);
                Generations.Add(newGeneration);

                if (EliteSize > 0)
                {
                    EliteStrategy(previousGeneration,newGeneration);
                }

                previousGeneration = newGeneration;
            }
        }

        private void EliteStrategy(Generation previousGen, Generation nextGen)
        {
            var elite = previousGen.Population.OrderByDescending(_ => _.Fx).Take(EliteSize)
                .Select(_ => new {_.X, _.Fx}).ToArray();
            var missedElites = elite.Where(e => previousGen.Population.All(p => p.FinalX != e.X)).ToList();

            var j = 0;
            while (missedElites.Any())
            {
                var missedElite = missedElites[0];
                missedElites.RemoveAt(0);
                var injected = false;

                while (injected == false && j < N)
                {
                    if (elite.All(e => e.X != nextGen.Population[j].X))
                    {
                        nextGen.Population[j].X = missedElite.X;
                        nextGen.Population[j].Fx = missedElite.Fx;
                        injected = true;
                    }

                    j++;
                }
            }
        }

        public Generation GetFinalGeneration()
        {
            return Generations[^1];
        }

        public decimal Result()
        {
            return GetFinalGeneration().Population.Max(_ => _.Fx);
        }
    }
}