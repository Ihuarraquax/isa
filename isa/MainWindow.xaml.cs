﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using GeneticAlgorithmModule.Models;
using GeneticAlgorithmModule.Models.Serializable;
using Microsoft.Win32;
using ScottPlot;
using WpfApplication.ViewModels;

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

        public MainWindow()
        {
            InitializeComponent();
        }



        private void RunAlgorithm(object sender, RoutedEventArgs e)
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
    }
}