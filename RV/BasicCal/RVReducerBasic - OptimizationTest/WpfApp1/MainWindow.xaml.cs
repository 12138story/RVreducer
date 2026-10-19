using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double TestData;
        double TestValue2;
        TextBox TestTextBox=new TextBox();
        public MainWindow()
        {
            InitializeComponent();
            //MessageBox.Show("完成了");963.
            //this.Visibility = Visibility.Hidden;
            // this.Visibility = Visibility.Visible;
            


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           // string aaaa=sender.ToString();
           // Button btn = (Button)sender;
           // btn.Name = "bbbb";
           // TestData = 2;
           // Button cmd = (Button)e.OriginalSource;
           //this.Visibility = Visibility.Hidden;

           // btn.Content = TestData.ToString();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}