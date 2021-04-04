using CsvHelper.Configuration.Attributes;

namespace Zad2
{
    public class ResultDTO
    {
        [Name("n")]
        public int N { get; set; }
        [Name("t")]
        public int T { get; set; }
        [Name("pk")]
        public decimal Pk { get; set; }
        [Name("pm")]
        public decimal Pm { get; set; }
        [Name("fMax")]
        public decimal Fmax { get; set; }
        [Name("fAvg")]
        public decimal Favg { get; set; }
    }
}