using isa.Models;
using System.Collections.Generic;

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
                var individualAfterSelection = generation.PopulationAfterSelection[i];
                tableRowList.Add(new TableRow
                {
                    Index = i + 1,
                    Value = individual.Value,
                    Fx = individual.Fx,
                    Gx = individual.Gx,
                    P = individual.P,
                    Qx = individual.Qx,
                    R = individual.R,
                    XRel = individualAfterSelection.Value,
                }); 
            }
            return tableRowList;
        }
    }
}