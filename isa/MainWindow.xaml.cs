using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using isa.Models;
using NumberFormatManager.Services;

namespace isa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var a = int.Parse(A.Text);
            var b = int.Parse(B.Text);
            var d = decimal.Parse(D.Text, CultureInfo.InvariantCulture);
            var n = int.Parse(N.Text);

            var pk = decimal.Parse(Pk.Text, CultureInfo.InvariantCulture);
            var pm = decimal.Parse(Pm.Text, CultureInfo.InvariantCulture);

            var generation = new Generation(a, b, d,pk,pm, n);

            generation.GeneratePopulation();
            generation.CalculateFxForPopulation();
            generation.CalculateGx();
            generation.CalculateP();
            generation.CalculateQ();
            generation.CalculateProbablityToSurvive();
            generation.SelectInviduals();
            generation.CalculateNewIndividualsBin();
            generation.MarkParents();
            generation.PairParentsAndGeneratePointcuts();
            generation.CrossParents();
            generation.MutateGenes();
            generation.CalculateFinalValues();

            List<TableRowLab4> Values = TableRowLab4.MapFromGeneration(generation);
            ValuesListView.ItemsSource = Values;
        }
    }
}