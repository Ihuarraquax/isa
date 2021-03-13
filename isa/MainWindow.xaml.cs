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
            List<XRepresentation> Values = new();
            for (int i = 0; i < n; i++)
            {
                var value = new XRepresentation();
                value.Index = i + 1;
                value.GeneratedXReal = manager.RandomDecimal();
                value.XIntFromGeneratedXReal = manager.RealToInt(value.GeneratedXReal);
                value.XBinFromXInt = manager.IntToBin(value.XIntFromGeneratedXReal);
                value.XIntFromXBin = manager.BinToInt(value.XBinFromXInt);
                value.XRealFromXInt = manager.IntToReal(value.XIntFromXBin);
                value.XFunction = manager.CalculateFx(value.XRealFromXInt);

                Values.Add(value);
            }
            ValuesListView.ItemsSource = Values;
        }
    }
}