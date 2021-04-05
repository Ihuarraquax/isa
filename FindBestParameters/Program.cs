using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Zad2;

namespace FindBestParameters
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            using var reader = new StreamReader(@"zad2.csv");
            using var csvReader = new CsvReader(reader, config);
            var records = csvReader.GetRecords<ResultDTO>();

            var bestResults = records.OrderByDescending(_ => _.Favg).ThenByDescending(_ => _.Fmax).ThenBy(_ => _.N*_.T).ToList();
            
            using var writer = new StreamWriter("zad2_posortowane.csv");
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(bestResults);
            
        }
    }
}