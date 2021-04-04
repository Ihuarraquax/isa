using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;

namespace GeneticAlgorithmModule.Models.Serializable
{
    public class GeneticAlgorithmResult
    {
        public List<IndividualResult> Population { get; set; }
        
        public static GeneticAlgorithmResult MapFrom(GeneticAlgorithm geneticAlgorithm)
        {
            var result = new GeneticAlgorithmResult();
            var finalGeneration = geneticAlgorithm.GetFinalGeneration();
            result.Population = finalGeneration.Population.DistinctBy(_ => _.X).Select(p => new IndividualResult
            {
                XReal = p.X,
                XBin = p.XBin,
                Fx = p.Fx,
                Percent = ((decimal)finalGeneration.Population.Count(_ => _.X == p.X) / finalGeneration.Population.Length) * 100
            }).OrderByDescending(_ => _.Fx).ToList();

            return result;
        }
    }
    
    public class IndividualResult
    {
        public decimal XReal { get; set; }
        public string XBin { get; set; }
        public decimal Fx { get; set; }
        public decimal Percent { get; set; }
    }
}