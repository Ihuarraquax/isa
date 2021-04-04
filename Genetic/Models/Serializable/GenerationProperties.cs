using System.Collections.Generic;

namespace GeneticAlgorithmModule.Models.Serializable
{
    public class GenerationProperties
    {
        public int GenerationNumber { get; set; }
        public List<IndividualProperties> Population { get; set; }

    }
}