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
            var manager = new NumberFormatService(a, b, d);


            List<TableRow> Values = new();
            for (int i = 0; i < n; i++)
            {
                var value = new TableRow();
                value.Index = i + 1;
                value.GeneratedXReal = manager.RandomDecimal();
                value.Fx = manager.CalculateFx(value.GeneratedXReal);

                Values.Add(value);
            }


            var fxMin = Values.Select(_ => _.Fx).Min();

            // gx
            Values.ForEach(v =>
            {
                v.Gx = v.Fx - fxMin + manager.D;
            });

            // p
            var gxSum = Values.Select(v => v.Gx).Sum();

            Values.ForEach(v =>
            {
                v.P = v.Gx / gxSum;
            });


            // q
            for (int i = 0; i < Values.Count; i++)
            {
                if(i == Values.Count - 1)
                {
                    Values[i].Qx = 1;
                } else
                {
                    if (i > 0)
                    {
                        Values[i].Qx = Values[i].P + Values[i - 1].Qx;
                    }
                    else
                    {
                        Values[i].Qx = Values[i].P;
                    }
                }
            }

            // r 
            Values.ForEach(_ =>
            {
                _.R = Convert.ToDecimal(new Random().NextDouble());
            });

            for (int i = 0; i < Values.Count; i++)
            {
                Values[i].XRel = Values.First(_ => _.Qx > Values[i].R).GeneratedXReal;
            }




            ValuesListView.ItemsSource = Values;
        }
    }
}