using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        MainWindow MyWindow1=new MainWindow();
        TestWindow1 MyWindow2 = new TestWindow1();
        public static double[] sharedValue=new double[2] { 1,2};

        //public static double[] TestData=new double[2] { 1,2};

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //MainWindow Wnd=new MainWindow();
            //Wnd.Title = "龙哥";
            //Wnd.Show();
            MyWindow1.Show();
            MyWindow1.AddHandler(Button.ClickEvent, new RoutedEventHandler(Button_Click1));
        }
        public void Button_Click1(object sender, RoutedEventArgs e)
        {
            //string aaaa = sender.ToString();
            //Button btn = (Button)sender;
            //btn.Name = "bbbb";
            //TestData = 2;
            Button cmd = (Button)e.OriginalSource;
            if (cmd.Name == "RightBtn") 
            {
                MyWindow1.Hide();
                MyWindow2.Title = "RightClick";
                MyWindow2.Show();
            }
            if (cmd.Name == "LeftBtn")
            {
                MyWindow1.Hide();
                MyWindow2.Title = "LeftClick";
                MyWindow2.Show();
            }
            //this.Visibility = Visibility.Hidden;

            //btn.Content = TestData.ToString();



        }
    }

}
