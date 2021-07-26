using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithmModule.Models.Serializable
{
    public class GeneticAlgorithmSummary
    {
        public List<GenerationSummary> Generations { get; set; }

        public static GeneticAlgorithmSummary MapFrom(GeneticAlgorithm geneticAlgorithm)
        {
            var result = new GeneticAlgorithmSummary
            {
                Generations = geneticAlgorithm.Generations.Select(g => new GenerationSummary
                {
                    GenerationNumber = g.GenerationNumber,
                    FMin = g.Population.Min(_ => _.Fx),
                    FAvg = g.Population.Average(_ => _.Fx),
                    FMax = g.Population.Max(_ => _.Fx)
                }).ToList()
            };
            
            return result;
        }
    }
}