using System;
using isa.Models;
using NumberFormatManager.Services;
using System.Linq;

namespace isa
{
    public class GeneticAlgorithm
    {
        private readonly int _n;
        private readonly int _t;
        private readonly decimal _pk;
        private readonly decimal _pm;
        private readonly int _eliteSize;

        private NumberFormatService _manager;
        public Generation[] Generations { get; set; }

        public GeneticAlgorithm(int a, int b, decimal d, decimal pk, decimal pm, int n, int t, int eliteSize)
        {
            _pk = pk;
            _pm = pm;
            _n = n;
            _t = t;
            _eliteSize = eliteSize;


            _manager = new NumberFormatService(a, b, d);

            Generations = new Generation[_t];
            Generations[0] = new Generation(_manager, _pk, _pm, _n);
            Generations[0].GenerateInitialPopulationAndCalculateFx();
        }

        public void Run()
        {
            for (int i = 1; i < Generations.Length; i++)
            {
                Generations[i - 1].CalculateNewPopulationProperties();
                Generations[i] = Generation.From(Generations[i - 1]);

                if (_eliteSize > 0)
                {
                    EliteStrategy(i);
                }
            }
        }

        private void EliteStrategy(int i)
        {
            var elite = Generations[i - 1].Population.OrderByDescending(_ => _.Fx).Take(_eliteSize)
                .Select(_ => new {_.Value, _.Fx}).ToArray();
            var missedElites = elite.Where(e => Generations[i - 1].Population.All(p => p.FinalValue != e.Value)).ToList();

            var j = 0;
            while (missedElites.Any())
            {
                var missedElite = missedElites[0];
                missedElites.RemoveAt(0);
                var injected = false;

                while (injected == false && j < _n)
                {
                    if (elite.All(e => e.Value != Generations[i].Population[j].Value))
                    {
                        Generations[i].Population[j].Value = missedElite.Value;
                        Generations[i].Population[j].Fx = missedElite.Fx;
                        injected = true;
                    }

                    j++;
                }
            }
        }

        public Generation GetFinalGeneration()
        {
            return Generations[_t - 1];
        }
    }
}