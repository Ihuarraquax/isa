using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isa.Models
{
    public class Individual
    {
        public decimal Value { get; set; }
        public decimal Fx { get; set; }
        public decimal Gx { get; set; }
        public decimal P { get; set; }
        public decimal Qx { get; set; }
        public decimal R { get; set; }
        public decimal ValueAfterSelection { get; set; }
        public string ValueAfterSelectionBin { get; set; }
        public bool IsParent { get; set; }
        public List<Partner> Partners { get; set; }
        public string ChildValueBin { get; set; }
        public List<int> MutatedGenes { get; set; }
        public string ValueAfterMutationBin { get; set; }
        public decimal FinalValue { get; set; }
        public decimal FxFinalValue { get; set; }
    }

    public class GenericProperties
    {

    }

    public class Partner
    {
        public Individual Individual { get; set; }
        public int Pointcut { get; set; }
    }
}
