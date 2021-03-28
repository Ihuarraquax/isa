using isa.Extensions;
using NumberFormatManager.Services;
using System;
using System.Linq;

namespace isa.Models
{
    public class Generation
    {
        public Individual[] Population { get; set; }
        public Individual[] PopulationAfterSelection { get; set; }
        public int N { get; set; }

        private NumberFormatService _manager;

        public Generation(int a, int b, decimal d, int n)
        {
            N = n;
            _manager = new NumberFormatService(a, b, d);
            initPopulation();
        }

        public void GeneratePopulation()
        {
            
            for (int i = 0; i < N; i++)
            {
                Population[i].Value = _manager.RandomDecimal();
            }
        }

        private void initPopulation()
        {
            Population = new Individual[N];
            for (int i = 0; i < N; i++)
            {
                Population[i] = new Individual();
            }
        }

        private void initPopulationAfterSelection()
        {
            PopulationAfterSelection = new Individual[N];
            for (int i = 0; i < N; i++)
            {
                PopulationAfterSelection[i] = new Individual();
            }
        }

        public void CalculateFxForPopulation()
        {
            Population.ForEach(_ => _.Fx = _manager.CalculateFx(_.Value));
        }

        public void CalculateGx()
        {
            var fxMin = Population.Select(_ => _.Fx).Min();
            Population.ForEach(_ => _.Gx = _.Fx - fxMin + _manager.D);
        }

        public void CalculateP()
        {
            var gxSum = Population.Select(v => v.Gx).Sum();
            Population.ForEach(_ => _.P = _.Gx / gxSum);
        }

        public void CalculateQ()
        {
            for (int i = 0; i < Population.Length; i++)
            {
                if (i == Population.Length - 1)
                {
                    Population[i].Qx = 1;
                }
                else
                {
                    if (i > 0)
                    {
                        Population[i].Qx = Population[i].P + Population[i - 1].Qx;
                    }
                    else
                    {
                        Population[i].Qx = Population[i].P;
                    }
                }
            }
        }

        public void CalculateProbablityToSurvive()
        {
            Population.ForEach(_ =>
            {
                _.R = Convert.ToDecimal(new Random().NextDouble());
            });
        }

        public void SelectInviduals()
        {
            initPopulationAfterSelection();
            for (int i = 0; i < Population.Length; i++)
            {
                PopulationAfterSelection[i].Value = Population.First(_ => _.Qx > Population[i].R).Value;
            }
        }
    }
}
