using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticAlgorithmModule.Extensions;
using NumberFormatManager.Services;

namespace GeneticAlgorithmModule.Models
{
    public class Generation
    {
        public List<Individual> Population { get; set; }
        public int N { get; }
        public decimal Pk { get; }
        public decimal Pm { get; }
        private readonly NumberFormatService _numberFormatService;
        private readonly Random _random;
        public int GenerationNumber { get; set; } = 1;

        public Generation(NumberFormatService numberFormatService, decimal pk, decimal pm, int n, Random random)
        {
            Population = new List<Individual>();
            _numberFormatService = numberFormatService;
            N = n;
            _random = random;
            Pk = pk;
            Pm = pm;
        }

        public static Generation From(Generation prevGen)
        {
            var nextGen = new Generation(prevGen._numberFormatService, prevGen.Pk, prevGen.Pm, prevGen.N, prevGen._random);
            nextGen.Population = prevGen.Population.Select(_ => new Individual {X = _.FinalX}).ToList();
            nextGen.CalculateFxForPopulation();
            nextGen.CalculateValueBinForPopulation();
            nextGen.GenerationNumber = prevGen.GenerationNumber + 1;
            return nextGen;
        }

        public void GenerateInitialPopulationCalculateFxAndBin()
        {
            for (var i = 0; i < N; i++)
            {
                Population.Add(new Individual
                {
                    X = _numberFormatService.RandomDecimal()
                });
            }

            CalculateFxForPopulation();
            CalculateValueBinForPopulation();
        }

        private void CalculateValueBinForPopulation()
        {
            Population.ForEach(_ => _.XBin = _numberFormatService.RealToBin(_.X));
        }

        public void CalculateNewPopulationProperties()
        {
            CalculateGx();
            CalculateP();
            CalculateQ();
            CalculateProbablityToSurvive();
            SelectInviduals();
            CalculateNewIndividualsBin();
            MarkParents();
            PairParentsAndGeneratePointcuts();
            CrossParents();
            MutateGenes();
            CalculateFinalValues();
        }

        public void CalculateFxForPopulation()
        {
            Population.ForEach(_ => _.Fx = _numberFormatService.CalculateFx(_.X));
        }

        public void CalculateGx()
        {
            var fxMin = Population.Select(_ => _.Fx).Min();
            Population.ForEach(_ => _.Gx = (double) (_.Fx - fxMin + _numberFormatService.D));
        }

        public void CalculateP()
        {
            var gxSum = Population.Select(v => v.Gx).Sum();
            Population.ForEach(_ => _.P = _.Gx / gxSum);
        }

        public void CalculateQ()
        {
            for (int i = 0; i < Population.Count; i++)
            {
                if (i == Population.Count - 1)
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

        public void CalculateFinalValues()
        {
            Population.ForEach(_ =>
            {
                _.FinalX = _numberFormatService.BinToReal(_.XAfterMutationBin);
            });
        }

        public void MutateGenes()
        {
            Population.ForEach(_ =>
            {
                _.MutatedGenes = GenerateGenesToMutate();
                StringBuilder sb = new StringBuilder(_.ChildXBin ?? _.XAfterSelectionBin);
                _.MutatedGenes.ForEach(i => { sb[i] = sb[i] == '0' ? '1' : '0'; });
                _.XAfterMutationBin = sb.ToString();
            });
        }

        private List<int> GenerateGenesToMutate()
        {
            var Pmd = (double) Pm;
            var list = new List<int>();
            var l = _numberFormatService.L;
            for (int i = 0; i < l; i++)
            {
                var r = _random.NextDouble();
                if (r < Pmd)
                {
                    list.Add(i);
                }
            }

            return list;
        }

        public void CrossParents()
        {
            Population.Where(_ => _.IsParent).ForEach(_ =>
            {
                var partnership = _.Partners[0];
                _.ChildXBin = CrossToParents(_.XAfterSelectionBin, partnership.Individual.XAfterSelectionBin, partnership.Pointcut);
            });
        }

        public static string CrossToParents(string p1, string p2, int pointcut)
        {
            return p1.Substring(0, pointcut + 1) + p2.Substring(pointcut + 1);
        }

        public void MarkParents()
        {
            Population.ForEach(_ =>
            {
                var R = (decimal) _random.NextDouble();
                if (R < Pk)
                {
                    _.IsParent = true;
                }
            });
        }

        public void PairParentsAndGeneratePointcuts()
        {
            var parents = Population.Where(_ => _.IsParent).ToArray();
            if (parents.Length < 1)
            {
                return;
            }

            initPartners(parents);
            for (int i = 0; i < parents.Length;)
            {
                var pointcut = _random.Next(0, _numberFormatService.L - 1);
                if (i + 1 < parents.Length)
                {
                    parents[i].Partners.Add(new Partner
                    {
                        Individual = parents[i + 1],
                        Pointcut = pointcut
                    });
                    parents[i + 1].Partners.Add(new Partner
                    {
                        Individual = parents[i],
                        Pointcut = pointcut
                    });
                    i += 2;
                }
                else
                {
                    var randomParentIndex = _random.Next(0, i - 1);
                    parents[i].Partners.Add(new Partner
                    {
                        Individual = parents[randomParentIndex],
                        Pointcut = pointcut
                    });
                    parents[randomParentIndex].Partners.Add(new Partner
                    {
                        Individual = parents[i],
                        Pointcut = pointcut
                    });
                    i += 1;
                }
            }
        }

        private void initPartners(Individual[] parents)
        {
            parents.ForEach(_ => _.Partners = new List<Partner>());
        }

        public void CalculateNewIndividualsBin()
        {
            Population.ForEach(_ => _.XAfterSelectionBin = _numberFormatService.RealToBin(_.XAfterSelection));
        }

        public void CalculateProbablityToSurvive()
        {
            Population.ForEach(_ => { _.R = _random.NextDouble(); });
        }

        public void SelectInviduals()
        {
            for (int i = 0; i < Population.Count; i++)
            {
                var r = Population[i].R;
                for (int j = 0; j < Population.Count; j++)
                {
                    if (Population[j].Qx > r)
                    {
                        Population[i].XAfterSelection = Population[j].X;
                        break;
                    }
                }
            }
        }
    }
}