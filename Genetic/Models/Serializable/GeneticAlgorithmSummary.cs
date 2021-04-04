using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithmModule.Models.Serializable
{
    public class GeneticAlgorithmSummary
    {
        public AlgorithmProperties AlgorithmProperties { get; set; }
        public List<GenerationSummary> Generations { get; set; }

        public static GeneticAlgorithmSummary MapFrom(GeneticAlgorithm geneticAlgorithm)
        {
            var result = new GeneticAlgorithmSummary
            {
                AlgorithmProperties = new AlgorithmProperties
                {
                    A = geneticAlgorithm.Manager.A,
                    B = geneticAlgorithm.Manager.B,
                    D = geneticAlgorithm.Manager.D,
                    N = geneticAlgorithm.N,
                    Pk = geneticAlgorithm.Pk,
                    Pm = geneticAlgorithm.Pm,
                    T = geneticAlgorithm.T,
                    EliteSize = geneticAlgorithm.EliteSize
                },
                Generations = geneticAlgorithm.Generations.Select(g => new GenerationSummary
                {
                    GenerationNumber = g.GenerationNumber,
                    FMin = g.Population.Min(_ => _.Fx),
                    FAvg = g.Population.Average(_ => _.Fx),
                    FMax = g.Population.Max(_ => _.Fx),
                }).ToList()
            };



            return result;
        }
    }
}