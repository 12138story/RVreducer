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
//using System.Windows.Forms;
namespace TestWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBox TestBox=new TextBox();
        //DoubleText
        int[] TestData =new int[2] { 1111,2222};
        double[] TestData5 = new double[2] { 1111, 2222 };
        string[] TestData2 = new string[2] { "ttt", "ddd" };
        public MainWindow()
        {
            InitializeComponent();
            Grid.SetRow(TestBox, 0);
            Grid.SetColumn(TestBox, 0);
            TestGrid.Children.Add(TestBox);
            TestBox.SetBinding(TextBox.TextProperty, new Binding("[1]"){Source= TestData5 });
            TestData[0] = 1234;
        }
    }
}