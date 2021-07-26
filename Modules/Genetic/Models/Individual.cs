using System.Collections.Generic;

namespace GeneticAlgorithmModule.Models
{
    public class Individual
    {
        public decimal X { get; set; }
        public string XBin { get; set; }
        public decimal Fx { get; set; }
        public double Gx { get; set; }
        public double P { get; set; }
        public double Qx { get; set; }
        public double R { get; set; }
        public decimal XAfterSelection { get; set; }
        public string XAfterSelectionBin { get; set; }
        public bool IsParent { get; set; }
        public List<Partner> Partners { get; set; }
        public string ChildXBin { get; set; }
        public List<int> MutatedGenes { get; set; }
        public string XAfterMutationBin { get; set; }
        public decimal FinalX { get; set; }
    }

    public class Partner
    {
        public Individual Individual { get; set; }
        public int Pointcut { get; set; }
    }
}
