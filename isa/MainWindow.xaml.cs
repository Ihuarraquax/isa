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

            var generation = new Generation(a, b, d, n);

            generation.GeneratePopulation();
            generation.CalculateFxForPopulation();
            generation.CalculateGx();
            generation.CalculateP();
            generation.CalculateQ();
            generation.CalculateProbablityToSurvive();
            generation.SelectInviduals();


            List<TableRow> Values = TableRow.MapFromGeneration(generation);
            ValuesListView.ItemsSource = Values;
            //List<TableRow> Values = new();
            //for (int i = 0; i < n; i++)
            //{
            //    var value = new TableRow();
            //    value.Index = i + 1;
            //    value.GeneratedXReal = manager.RandomDecimal();
            //    value.Fx = manager.CalculateFx(value.GeneratedXReal);

            //    Values.Add(value);
            //}



            //var fxMin = Values.Select(_ => _.Fx).Min();

            //// gx
            //Values.ForEach(v =>
            //{
            //    v.Gx = v.Fx - fxMin + manager.D;
            //});

            // p
            //var gxSum = Values.Select(v => v.Gx).Sum();

            //Values.ForEach(v =>
            //{
            //    v.P = v.Gx / gxSum;
            //});


            // q
            //for (int i = 0; i < Values.Count; i++)
            //{
            //    if (i == Values.Count - 1)
            //    {
            //        Values[i].Qx = 1;
            //    }
            //    else
            //    {
            //        if (i > 0)
            //        {
            //            Values[i].Qx = Values[i].P + Values[i - 1].Qx;
            //        }
            //        else
            //        {
            //            Values[i].Qx = Values[i].P;
            //        }
            //    }
            //}


            // r 
            //Values.ForEach(_ =>
            //{
            //    _.R = Convert.ToDecimal(new Random().NextDouble());
            //});

            //for (int i = 0; i < Values.Count; i++)
            //{
            //    Values[i].XRel = Values.First(_ => _.Qx > Values[i].R).GeneratedXReal;
            //}

            //    for (int i = 0; i < Values.Count; i++)
            //    {
            //        Values[i].XRelBin = manager.RealToBin(Values[i].XRel);
            //    }

            //    for (int i = 0; i < Values.Count; i++)
            //    {
            //        var R = (decimal)new Random().NextDouble();
            //        if (R < pk)
            //        {
            //            Values[i].XRelParent = Values[i].XRelBin;
            //        }
            //    }

            //    for (int i = 0; i < Values.Count; i++)
            //    {
            //        if (!string.IsNullOrEmpty(Values[i].XRelParent) && Values[i].Pc == null)
            //        {
            //            var nextParentIndex = GetNextParent(i, Values); 

            //            if(nextParentIndex != -1)
            //            {
            //                var r = new Random().Next(0, manager.L - 2);
            //                Values[i].Pc = r;
            //                Values[nextParentIndex].Pc = r;
            //            }
            //        }
            //    }

            //    ValuesListView.ItemsSource = Values;
            //}

            //private int GetNextParent(int i, List<TableRow> Values)
            //{
            //    for (int j = i+1; j < Values.Count; j++)
            //    {
            //        if (!string.IsNullOrEmpty(Values[j].XRelParent))
            //        {
            //            return j;
            //        }
            //    }
            //    return GetFirstParentIndex(Values);
            //}

            //private int GetFirstParentIndex(List<TableRow> values)
            //{
            //    return 0;
            //}
        }
    }
}