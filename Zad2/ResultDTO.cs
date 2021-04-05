using CsvHelper.Configuration.Attributes;

namespace Zad2
{
    public class ResultDTO
    {
        [Index(0)]
        public int N { get; set; }
        [Index(1)]
        public int T { get; set; }
        [Index(2)]
        public decimal Pk { get; set; }
        [Index(3)]
        public decimal Pm { get; set; }
        [Index(4)]
        public decimal Fmax { get; set; }
        [Index(5)]
        public decimal Favg { get; set; }
    }
}