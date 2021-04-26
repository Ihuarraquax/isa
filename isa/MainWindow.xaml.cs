using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;
using CsvHelper;
using GeneticAlgorithmModule.Models;
using GeneticAlgorithmModule.Models.Serializable;
using GeoAlgorithmModule.Models;
using HillAlgorithmModule;
using Microsoft.Win32;
using NumberFormatManager.Services;
using ScottPlot;
using WpfApplication.ViewModels;
using Zad2;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region MyRegion

        private GeneticAlgorithm GeneticAlgorithm;
        private GeneticAlgorithmRun GeneticAlgorithmRun;
        private GeneticAlgorithmResult GeneticAlgorithmResult;
        private GeneticAlgorithmSummary GeneticAlgorithmSummary;

        private GeoAlgorithm GeoAlgorithm;
        private HillAlgorithm HillAlgorithm;


        public MainWindow()
        {
            InitializeComponent();
        }


        private void RunAlgorithmGenetic(object sender, RoutedEventArgs e)
        {
            var a = int.Parse(A.Text);
            var b = int.Parse(B.Text);
            var d = decimal.Parse(D.Text, CultureInfo.InvariantCulture);
            var n = int.Parse(N.Text);
            var t = int.Parse(T.Text);
            var pk = decimal.Parse(Pk.Text, CultureInfo.InvariantCulture);
            var pm = decimal.Parse(Pm.Text, CultureInfo.InvariantCulture);
            var eliteSize = int.Parse(EliteSize.Text);


            GeneticAlgorithm = new GeneticAlgorithm(a, b, d, pk, pm, n, t, eliteSize);
            GeneticAlgorithm.Run();
            GeneticAlgorithmRun = GeneticAlgorithmRun.MapFrom(GeneticAlgorithm);
            GeneticAlgorithmResult = GeneticAlgorithmResult.MapFrom(GeneticAlgorithm);
            GeneticAlgorithmSummary = GeneticAlgorithmSummary.MapFrom(GeneticAlgorithm);

            TabControl.Visibility = Visibility.Visible;
            GenerationSlider.Minimum = 1;
            GenerationSlider.Maximum = t;
            GenerationSlider.Value = 1;

            DisplayGenerationInRunDataGrid(1);
            DisplaySummaryInSummaryDataGrid();
            GeneratePlot();
        }


        private void GeneratePlot()
        {
            var plt = WpfPlot1.Plot;

            var xs = GeneticAlgorithmSummary.Generations.Select(_ => (double) _.GenerationNumber).ToArray();

            var fMin = GeneticAlgorithmSummary.Generations.Select(_ => (double) _.FMin).ToArray();
            var fAvg = GeneticAlgorithmSummary.Generations.Select(_ => (double) _.FAvg).ToArray();
            var fMax = GeneticAlgorithmSummary.Generations.Select(_ => (double) _.FMax).ToArray();

            plt.Clear();
            plt.PlotScatter(xs, fMax, label: "fmax");
            plt.PlotScatter(xs, fAvg, label: "favg");
            plt.PlotScatter(xs, fMin, label: "fmin");
            plt.Legend();

            plt.YLabel("Wartość funkcji");
            plt.XLabel("Pokolenie");
            WpfPlot1.Render();
        }

        private void DisplayGenerationInRunDataGrid(int populationNumber)
        {
            RunDataGrid.ItemsSource = TableRowAllProperties.MapFromGeneration(GeneticAlgorithm.Generations[populationNumber - 1]);
        }

        private void DisplaySummaryInSummaryDataGrid()
        {
            SummaryDataGrid.ItemsSource = GeneticAlgorithmResult.Population;
        }

        private void SaveRun(object sender, RoutedEventArgs e)
        {
            var json = JsonSerializer.Serialize(GeneticAlgorithmRun);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, json);
        }

        private void SaveSummary(object sender, RoutedEventArgs e)
        {
            var json = JsonSerializer.Serialize(GeneticAlgorithmSummary);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, json);
        }

        private void GenerationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DisplayGenerationInRunDataGrid((int) e.NewValue);
        }

        private void ImportTestCsv(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                using var reader = new StreamReader(openFileDialog.FileName);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csvReader.GetRecords<ResultDTO>().ToList();
                TestsDataGrid.ItemsSource = records;

                var TSummary = records.GroupBy(_ => _.T).Select(_ => new
                {
                    T = _.Key,
                    SredniaSrednich = _.Average(_ => _.Favg)
                }).ToList();
                TSummaryDataGrid.ItemsSource = TSummary;

                var NSummary = records.GroupBy(_ => _.N).Select(_ => new
                {
                    N = _.Key,
                    SredniaSrednich = _.Average(_ => _.Favg)
                }).ToList();
                NSummaryDataGrid.ItemsSource = NSummary;

                var PkSummary = records.GroupBy(_ => _.Pk).Select(_ => new
                {
                    Pk = _.Key,
                    SredniaSrednich = _.Average(_ => _.Favg)
                }).ToList();
                PkSummaryDataGrid.ItemsSource = PkSummary;

                var PmSummary = records.GroupBy(_ => _.Pm).Select(_ => new
                {
                    Pm = _.Key,
                    SredniaSrednich = _.Average(_ => _.Favg)
                }).ToList();
                PmSummaryDataGrid.ItemsSource = PmSummary;
            }
        }

        private void RunAlgorithmGeo(object sender, RoutedEventArgs e)
        {
            var a = int.Parse(Geo_A.Text);
            var b = int.Parse(Geo_B.Text);
            var d = decimal.Parse(Geo_D.Text, CultureInfo.InvariantCulture);
            var tau = decimal.Parse(Geo_Tau.Text, CultureInfo.InvariantCulture);
            var t = int.Parse(Geo_T.Text);

            GeoAlgorithm = new GeoAlgorithm(a, b, d, tau, t);
            GeoAlgorithm.Run();
            DisplaySummaryInGeoSummaryDataGrid();
            GenerateGeoPlot();
        }

        private void DisplaySummaryInGeoSummaryDataGrid()
        {
            var last = GeoAlgorithm.IterationResults.Where(_ => _.Iteration == GeoAlgorithm.T).Select(_ => new {_.XBest, _.FxBest}).ToList();

            GeoSummaryDataGrid.ItemsSource = last;
        }

        public class dup
        {
            public decimal fx { get; }
            public string bin { get; }
            public decimal x { get; }

            public dup(decimal fx, string bin, decimal x)
            {
                this.fx = fx;
                this.bin = bin;
                this.x = x;
            }
        }

        private void DisplaySummaryInHillSummaryDataGrid()
        {
            var last = HillAlgorithm.BestaFx;
            var bin = HillAlgorithm.BestaBin;
            var x = HillAlgorithm.Manager.BinToReal(bin);
            var result = new dup(last, bin, x);
            HillSummaryDataGrid.ItemsSource = new List<dup> {result};
        }

        private void GenerateGeoPlot()
        {
            var plt = GeoWpfPlot1.Plot;

            var xs = GeoAlgorithm.IterationResults.Select(_ => (double) _.Iteration).ToArray();

            var fx = GeoAlgorithm.IterationResults.Select(_ => (double) _.Fx).ToArray();
            var fxBest = GeoAlgorithm.IterationResults.Select(_ => (double) _.FxBest).ToArray();


            plt.Clear();
            plt.PlotScatter(xs, fx, label: "fx");
            plt.PlotScatter(xs, fxBest, label: "fxBest");
            plt.Legend();

            plt.YLabel("Wartość funkcji");
            plt.XLabel("Iteracja");
            GeoWpfPlot1.Render();
        }

        private void TestGeoAlgorithm(object sender, RoutedEventArgs e)
        {
            var t = 5000;
            var a = -4;
            var b = 12;
            var d = 0.001m;
            var tauRangeList = new List<decimal>();

            var current = 0.5m;
            var end = 3m;
            var step = 0.1m;
            while (current <= end)
            {
                tauRangeList.Add(current);
                current += step;
            }

            var tauRange = tauRangeList.ToArray();
            var random = new Random();
            var manager = new NumberFormatService(a, b, d, random);

            var tauResults = new List<TauTestResult>();
            foreach (var tau in tauRange)
            {
                Console.WriteLine($"testing {tau}");
                var results = new List<decimal>();
                var resultsTSelf = new List<decimal>();
                for (int i = 0; i < 1000; i++)
                {
                    var algorithm = new GeoAlgorithm(tau, t, random, manager);
                    algorithm.Run();
                    results.Add(algorithm.ResultFx);
                    resultsTSelf.Add(algorithm.SelfBestT);
                }

                tauResults.Add(new TauTestResult
                {
                    Tau = tau,
                    AverageBestFx = results.Average(),
                    AverageSelfBestIteration = resultsTSelf.Average()
                });
            }

            DrawGeoTestPlot(tauResults);
            GeoTestDataGrid.ItemsSource = tauResults;
        }

        private void DrawGeoTestPlot(List<TauTestResult> tauResults)
        {
            var plt = GeoTestWpfPlot1.Plot;

            var iter = tauResults.Select(_ => Math.Round((double) _.Tau, 1)).ToArray();

            var fxAve = tauResults.Select(_ => (double) _.AverageBestFx).ToArray();

            plt.Clear();
            plt.PlotBar(iter, fxAve, barWidth: ((double) iter.Length) / 270);
            plt.Legend();

            plt.SetAxisLimits(yMin: 0);
            plt.YLabel("Średni wynik");
            plt.XLabel("Iteracja");
            GeoTestWpfPlot1.Render();
        }

        private void SaveGeoRun(object sender, RoutedEventArgs e)
        {
            var json = JsonSerializer.Serialize(GeoAlgorithm.IterationResults);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, json);
        }

        private void RunAlgorithmHill(object sender, RoutedEventArgs e)
        {
            var a = int.Parse(Hill_A.Text);
            var b = int.Parse(Hill_B.Text);
            var d = decimal.Parse(Hill_D.Text, CultureInfo.InvariantCulture);
            var t = int.Parse(Hill_T.Text);

            HillAlgorithm = new HillAlgorithm(a, b, d, t);
            HillAlgorithm.Run();
            var result = HillAlgorithm.Result;

            DrawHillPlot(HillAlgorithm.Result);
            DisplaySummaryInHillSummaryDataGrid();
        }


        public class dup2
        {
            public int Key { get; }
            public int Count { get; }

            public dup2(int key, int count)
            {
                Key = key;
                Count = count;
            }
        }

        private void TestHillAlgorithm(object sender, RoutedEventArgs e)
        {
            var t = 1000;
            var a = -4;
            var b = 12;
            var d = 0.001m;

            var random = new Random();
            var manager = new NumberFormatService(a, b, d, random);

            var result = new List<int>();
            for (int i = 0; i < t; i++)
            {
                var algorithm = new HillAlgorithm(t, random, manager);
                algorithm.Run();
                result.Add(algorithm.Result.Count);
            }

            var dup = result.GroupBy(_ => _).Select(_ => new dup2(_.Key, _.Count())).OrderBy(_ => _.Key).ToList();

            DrawHillTestPlot(dup);
            HillTestDataGrid.ItemsSource = dup;
        }

        private void DrawHillTestPlot(List<dup2> result)
        {
            var plt = HillTestWpfPlot1.Plot;

            var iter = result.Select(_ => (double) _.Key).ToArray();
            var sum = 0;
            var fxAve = result.Select(_ =>
            {
                sum += _.Count;
                return (double) sum / 10;
            }).ToArray();

            plt.Clear();
            plt.PlotScatter(iter, fxAve);
            plt.AddHorizontalLine(90, style: LineStyle.Dash);
            plt.Legend();

            plt.SetAxisLimits(yMin: 0);
            plt.YLabel("%");
            plt.XLabel("Iteracja");
            plt.Render();
        }

        public class HillPlotData
        {
            public double Key { get; set; }
            public decimal Value { get; set; }
            public double Best { get; set; }
        }

        private void DrawHillPlot(List<HillAlgorithmIter> hillAlgorithmResult)
        {
            var plt = HillWpfPlot1.Plot;

            var bestVc = hillAlgorithmResult.Select(_ => _.Vcs.FirstOrDefault().Value).FirstOrDefault();
            var result = hillAlgorithmResult.Select(_ =>
            {
                var size = _.Vcs.Count();
                var step = 1.0 / size;
                var currentStep = _.Iter;
                var res = new List<HillPlotData>();
                for (int i = 0; i < size; i++)
                {
                    if (_.Vcs[i].Value > bestVc)
                    {
                        bestVc = _.Vcs[i].Value;
                    }

                    res.Add(new HillPlotData
                    {
                        Key = currentStep + i * step,
                        Value = _.Vcs[i].Value,
                        Best = (double) bestVc
                    });
                }

                return res.ToList();
            }).ToList();

            var iter = result.SelectMany(_ => _.Select(_ => _.Key)).ToArray();
            var fxAve = result.SelectMany(_ => _.Select(_ => (double) _.Value)).ToArray();
            var fxBest = result.SelectMany(_ => _.Select(_ => _.Best)).ToArray();


            plt.Clear();
            plt.PlotScatter(iter, fxBest, label: "best");
            plt.PlotScatter(iter, fxAve, lineWidth: 0);
            hillAlgorithmResult.ForEach(_ => { plt.PlotVLine(_.Iter, lineStyle: LineStyle.Dash); });
            plt.Legend();
            HillWpfPlot1.Render();
        }

        private void SaveHillRun(object sender, RoutedEventArgs e)
        {
            var json = JsonSerializer.Serialize(HillAlgorithm.Result);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, json);
        }
    }


    public class TauTestResult
    {
        public decimal Tau { get; set; }
        public decimal AverageBestFx { get; set; }
        public decimal AverageSelfBestIteration { get; set; }
    }

    #endregion MyRegion
}