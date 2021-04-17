using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using CsvHelper;
using GeneticAlgorithmModule.Models;
using GeneticAlgorithmModule.Models.Serializable;
using GeoAlgorithmModule.Models;
using Microsoft.Win32;
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
        private GeneticAlgorithm GeneticAlgorithm;
        private GeneticAlgorithmRun GeneticAlgorithmRun;
        private GeneticAlgorithmResult GeneticAlgorithmResult;
        private GeneticAlgorithmSummary GeneticAlgorithmSummary;        
        
        private GeoAlgorithm GeoAlgorithm;


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
            var fAvg = GeneticAlgorithmSummary.Generations.Select(_ => (double)_.FAvg).ToArray();
            var fMax = GeneticAlgorithmSummary.Generations.Select(_ => (double)_.FMax).ToArray();

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
            RunDataGrid.ItemsSource  = TableRowAllProperties.MapFromGeneration(GeneticAlgorithm.Generations[populationNumber - 1]);
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
            var last = GeoAlgorithm.IterationResults[GeoAlgorithm.T - 1];
            var list = new List<IterationResult>()
            {
                last
            };
            GeoSummaryDataGrid.ItemsSource = list;
        }
        private void GenerateGeoPlot()
        {
            var plt = GeoWpfPlot1.Plot;

            var xs = GeoAlgorithm.IterationResults.Select(_ => (double) _.Iteration).ToArray();

            var fx = GeoAlgorithm.IterationResults.Select(_ => (double) _.Fx).ToArray();
            var fxBest = GeoAlgorithm.IterationResults.Select(_ => (double) _.VBestValue).ToArray();


            plt.Clear();
            plt.PlotScatter(xs, fx, label: "fx");
            plt.PlotScatter(xs, fxBest, label: "fxBest");
            plt.Legend();

            plt.YLabel("Wartość funkcji");
            plt.XLabel("Iteracja");
            GeoWpfPlot1.Render();
        }
    }
}