using isa.Models;
using System.Collections.Generic;
using System.Linq;

namespace isa
{
    public class TableRow
    {


        public int Index { get; set; }
        public decimal Value { get; set; }
        public decimal Fx { get; set; }
        public decimal Gx { get; set; }
        public decimal P { get; set; }
        public decimal Qx { get; set; }
        public decimal R { get; set; }
        public decimal XRel { get; set; }



        public static List<TableRow> MapFromGeneration(Generation generation)
        {
            var tableRowList = new List<TableRow>();

            for (int i = 0; i < generation.N; i++)
            {
                var individual = generation.Population[i];
                tableRowList.Add(new TableRow
                {
                    Index = i + 1,
                    Value = individual.Value,
                    Fx = individual.Fx,
                    Gx = individual.Gx,
                    P = individual.P,
                    Qx = individual.Qx,
                    R = individual.R,
                    XRel = individual.ValueAfterSelection,
                }); 
            }
            return tableRowList;
        }
    }

    public class TableRowLab4
    {
        public int Index { get; set; }
        public decimal Value { get; set; }
        public string ValueBin { get; set; }
        public string IsParent { get; set; }
        public string PointCut { get; set; }
        public string ChildValueBin { get; set; }
        public string ValueAfterCrossing { get; set; }
        public string MutatedGenes { get; set; }
        public string ValueAfterMutationBin { get; set; }
        public decimal FinalValue { get; set; }
        public decimal FxFinalValue { get; set; }

        public static List<TableRowLab4> MapFromGeneration(Generation generation)
        {
            var tableRowList = new List<TableRowLab4>();

            for (int i = 0; i < generation.N; i++)
            {
                var individual = generation.Population[i];
                tableRowList.Add(new TableRowLab4
                {
                    Index = i + 1,
                    Value = individual.ValueAfterSelection,
                    ValueBin = individual.ValueAfterSelectionBin,
                    IsParent = individual.IsParent? "Tak": "",
                    PointCut = individual.IsParent ? string.Join(", ", individual.Partners.Select(_ => $"{_.Pointcut}")) : "",
                    ChildValueBin = individual.ChildValueBin,
                    ValueAfterCrossing = individual.ChildValueBin ?? individual.ValueAfterSelectionBin,
                    MutatedGenes = string.Join(", ", individual.MutatedGenes),
                    ValueAfterMutationBin = individual.MutatedGenes.Count > 0 ? individual.ValueAfterMutationBin: "",
                    FinalValue = individual.FinalValue,
                    FxFinalValue = individual.FxFinalValue
                });
            }
            return tableRowList;
        }
    }
}