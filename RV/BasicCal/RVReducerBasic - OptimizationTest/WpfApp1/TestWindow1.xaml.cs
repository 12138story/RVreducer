using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// TestWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow1 : Window
    {
        public TestWindow1()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ListBox l = new ListBox();
            l.Items.Add("Hello");
            l.Items.Add("WPF");
            l.HorizontalAlignment = HorizontalAlignment.Center;
            l.VerticalAlignment= VerticalAlignment.Center;
            l.Visibility = Visibility.Visible;

            this.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
