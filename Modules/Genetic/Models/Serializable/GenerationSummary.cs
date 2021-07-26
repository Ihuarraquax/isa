namespace GeneticAlgorithmModule.Models.Serializable
{
    public class GenerationSummary
    {
        public int GenerationNumber { get; set; }
        public decimal FMin { get; set; }
        public decimal FAvg { get; set; }
        public decimal FMax { get; set; }
    }
}