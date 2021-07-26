using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using GeneticAlgorithmModule;
using GeneticAlgorithmModule.Models;
using GeneticAlgorithmModule.Models.Serializable;
using Microsoft.Win32;
using WpfApplication.ViewModels;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GeneticAlgorithmView : Window
    {
        private GeneticAlgorithm _geneticAlgorithm;
        private GeneticAlgorithmRun _geneticAlgorithmRun;
        private GeneticAlgorithmResult _geneticAlgorithmResult;
        private GeneticAlgorithmSummary _geneticAlgorithmSummary;

        public GeneticAlgorithmView()
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

            _geneticAlgorithm = new GeneticAlgorithm(a, b, d, pk, pm, n, t, eliteSize);
            _geneticAlgorithm.Run();
            _geneticAlgorithmRun = GeneticAlgorithmRun.MapFrom(_geneticAlgorithm);
            _geneticAlgorithmResult = GeneticAlgorithmResult.MapFrom(_geneticAlgorithm);
            _geneticAlgorithmSummary = GeneticAlgorithmSummary.MapFrom(_geneticAlgorithm);

            TabControl.Visibility = Visibility.Visible;

            InitGenerationSlider(t);
            DisplayGenerationInRunDataGrid(1);
            DisplaySummaryInSummaryDataGrid();
            GeneratePlot();
        }

        private void InitGenerationSlider(int t)
        {
            GenerationSlider.Minimum = 1;
            GenerationSlider.Maximum = t;
            GenerationSlider.Value = 1;
        }

        private void GeneratePlot()
        {
            var plot = WpfPlot1.Plot;

            var xs = _geneticAlgorithmSummary.Generations.Select(_ => (double) _.GenerationNumber).ToArray();

            var fMin = _geneticAlgorithmSummary.Generations.Select(_ => (double) _.FMin).ToArray();
            var fAvg = _geneticAlgorithmSummary.Generations.Select(_ => (double) _.FAvg).ToArray();
            var fMax = _geneticAlgorithmSummary.Generations.Select(_ => (double) _.FMax).ToArray();

            plot.Clear();
            plot.PlotScatter(xs, fMax, label: "fmax");
            plot.PlotScatter(xs, fAvg, label: "favg");
            plot.PlotScatter(xs, fMin, label: "fmin");
            plot.Legend();

            plot.YLabel("Wartość funkcji");
            plot.XLabel("Pokolenie");
            WpfPlot1.Render();
        }

        private void DisplayGenerationInRunDataGrid(int populationNumber)
        {
            RunDataGrid.ItemsSource = TableRowAllProperties.MapFromGeneration(_geneticAlgorithm.Generations[populationNumber - 1]);
        }

        private void DisplaySummaryInSummaryDataGrid()
        {
            SummaryDataGrid.ItemsSource = _geneticAlgorithmResult.Population;
        }

        private void SaveRun(object sender, RoutedEventArgs e)
        {
            var json = JsonSerializer.Serialize(_geneticAlgorithmRun);

            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, json);
        }

        private void SaveSummary(object sender, RoutedEventArgs e)
        {
            var json = JsonSerializer.Serialize(_geneticAlgorithmSummary);

            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, json);
        }

        private void GenerationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DisplayGenerationInRunDataGrid((int) e.NewValue);
        }
    }
}