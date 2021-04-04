using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithmModule.Models;

namespace WpfApplication.ViewModels
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
                    Value = individual.X,
                    Fx = individual.Fx,
                    Gx = individual.Gx,
                    P = individual.P,
                    Qx = individual.Qx,
                    R = individual.R,
                    XRel = individual.XAfterSelection,
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
                    Value = individual.XAfterSelection,
                    ValueBin = individual.XAfterSelectionBin,
                    IsParent = individual.IsParent? "Tak": "",
                    PointCut = individual.IsParent ? string.Join(", ", individual.Partners.Select(_ => $"{_.Pointcut}")) : "",
                    ChildValueBin = individual.ChildXBin,
                    ValueAfterCrossing = individual.ChildXBin ?? individual.XAfterSelectionBin,
                    MutatedGenes = string.Join(", ", individual.MutatedGenes),
                    ValueAfterMutationBin = individual.MutatedGenes.Count > 0 ? individual.XAfterMutationBin: "",
                    FinalValue = individual.FinalX,
                    FxFinalValue = individual.FinalFx
                });
            }
            return tableRowList;
        }
    }

    public class TableRowLab5
    {
        public int Index { get; set; }
        public decimal Value { get; set; }
        public decimal Fx { get; set; }

        public static List<TableRowLab5> MapFromGeneration(Generation generation)
        {
            var tableRowList = new List<TableRowLab5>();

            for (int i = 0; i < generation.N; i++)
            {
                var individual = generation.Population[i];
                tableRowList.Add(new TableRowLab5
                {
                    Index = i + 1,
                    Value = individual.X,
                    Fx = individual.Fx
                });
            }
            return tableRowList;
        }
    }
    
    public class TableRowAllProperties
    {
        public int lp { get; set; }
        public decimal X { get; set; }
        public string XBin { get; set; }
        public decimal Fx { get; set; }
        public decimal Gx { get; set; }
        public decimal P { get; set; }
        public decimal Qx { get; set; }
        public decimal R { get; set; }
        public decimal XAfterSelection { get; set; }
        public string XBinAfterSelection { get; set; }
        public string IsParent { get; set; }
        public string PointCut { get; set; }
        public string ChildXBin { get; set; }
        public string XAfterCrossing { get; set; }
        public string MutatedGenes { get; set; }
        public string XAfterMutationBin { get; set; }
        public decimal FinalX { get; set; }
        public decimal FinalFx { get; set; }

        public static List<TableRowAllProperties> MapFromGeneration(Generation generation)
        {
            var tableRowList = new List<TableRowAllProperties>();

            for (int i = 0; i < generation.N; i++)
            {
                var individual = generation.Population[i];
                tableRowList.Add(new TableRowAllProperties
                {
                    lp = i + 1,
                    X = individual.X,
                    XBin = individual.XBin,
                    Fx = individual.Fx,
                    Gx = individual.Gx,
                    P = individual.P,
                    Qx = individual.Qx,
                    R = individual.R,
                    XAfterSelection = individual.XAfterSelection,
                    XBinAfterSelection = individual.XAfterSelectionBin,
                    IsParent = individual.IsParent? "Tak": "",
                    PointCut = individual.IsParent ? string.Join(", ", individual.Partners.Select(_ => $"{_.Pointcut}")) : "",
                    ChildXBin = individual.ChildXBin,
                    XAfterCrossing = individual.ChildXBin ?? individual.XAfterSelectionBin,
                    MutatedGenes = individual.MutatedGenes != null ? string.Join(", ", individual.MutatedGenes): null,
                    XAfterMutationBin = individual.MutatedGenes != null && individual.MutatedGenes.Count > 0 ? individual.XAfterMutationBin: "",
                    FinalX = individual.FinalX,
                    FinalFx = individual.FinalFx
                });
            }
            return tableRowList;
        }
    }
}