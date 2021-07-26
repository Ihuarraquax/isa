using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithmModule.Models.Serializable
{
    public class GeneticAlgorithmRun
    {
        private AlgorithmProperties AlgorithmProperties { get; set; }
        private List<GenerationProperties> Generations { get; set; }

        public static GeneticAlgorithmRun MapFrom(GeneticAlgorithm geneticAlgorithm)
        {
            var result = new GeneticAlgorithmRun
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
                Generations = geneticAlgorithm.Generations.Select(g => new GenerationProperties
                {
                    GenerationNumber = g.GenerationNumber,
                    Population = g.Population.Select(p => new IndividualProperties {XReal = p.X, XBin = p.XBin, Fx = p.Fx,}).ToList()
                }).ToList()
            };
            return result;
        }
    }
}