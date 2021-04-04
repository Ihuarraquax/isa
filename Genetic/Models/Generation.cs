using isa.Extensions;
using NumberFormatManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isa.Models
{
    public class Generation
    {
        public Individual[] Population { get; set; }
        public int N { get; }
        public decimal Pk { get; }
        public decimal Pm { get; }
        private NumberFormatService _manager;
        public int GenerationNumber { get; set; } = 0;

        public Generation(int a, int b, decimal d, decimal pk, decimal pm, int n)
        {
            _manager = new NumberFormatService(a, b, d);
            N = n;
            Pk = pk;
            Pm = pm;
        }

        public Generation(NumberFormatService manager, decimal pk, decimal pm, int n)
        {
            _manager = manager;
            N = n;
            Pk = pk;
            Pm = pm;
        }

        public static Generation From(Generation prevGen)
        {
            var nextGen = new Generation(prevGen._manager, prevGen.Pk, prevGen.Pm, prevGen.N);
            nextGen.Population = prevGen.Population.Select(_ => new Individual { Value = _.FinalValue }).ToArray();
            nextGen.CalculateFxForPopulation();
            nextGen.GenerationNumber = prevGen.GenerationNumber + 1;
            return nextGen;
        } 

        public void GenerateInitialPopulationAndCalculateFx()
        {
            initEmptyPopulation();
            for (int i = 0; i < N; i++)
            {
                Population[i].Value = _manager.RandomDecimal();
            }
            CalculateFxForPopulation();
        }
        
        private void initEmptyPopulation()
        {
            Population = new Individual[N];
            for (int i = 0; i < N; i++)
            {
                Population[i] = new Individual();
            }
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

        public void CalculateFinalValues()
        {
            Population.ForEach(_ =>
            {
                _.FinalValue = _manager.BinToReal(_.ValueAfterMutationBin);
                _.FxFinalValue = _manager.CalculateFx(_.FinalValue);
            });
        }

        public void MutateGenes()
        {
            Population.ForEach(_ =>
            {
                _.MutatedGenes = GenerateGenesToMutate();
                StringBuilder sb = new StringBuilder(_.ChildValueBin ?? _.ValueAfterSelectionBin);
                _.MutatedGenes.ForEach(i =>
                {
                    sb[i] = sb[i] == '0' ? '1' : '0';
                });
                _.ValueAfterMutationBin = sb.ToString();
            });
        }

        private List<int> GenerateGenesToMutate()
        {
            var list = new List<int>();
            var l = _manager.L;
            for (int i = 0; i < l; i++)
            {
                var r = (decimal)new Random().NextDouble();
                if (r < Pm)
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
                _.ChildValueBin = CrossToParents(_.ValueAfterSelectionBin, partnership.Individual.ValueAfterSelectionBin, partnership.Pointcut);
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
                var R = (decimal)new Random().NextDouble();
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
                var pointcut = new Random().Next(0, _manager.L - 1);
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
                    var randomParentIndex = new Random().Next(0, i - 1);
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
            Population.ForEach(_ => _.ValueAfterSelectionBin = _manager.RealToBin(_.ValueAfterSelection));
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
            for (int i = 0; i < Population.Length; i++)
            {
                Population[i].ValueAfterSelection = Population.First(_ => _.Qx > Population[i].R).Value;
            }
        }
    }
}
