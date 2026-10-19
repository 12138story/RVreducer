using RVReducerDesignApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace RWPF
{
    /// <summary>
    /// GearPara.xaml 的交互逻辑
    /// </summary>
    public partial class BearingsPara : Window
    {
        public double BearingOut { get; set; }
        public double BearingIn { get; set; }
        public double BearingBroad { get; set; }
        public double BearingDistance {  get; set; }

        public string Orientation { get; private set; }
        public string Base { get; set; }
        public bool IsTaper { get; set; }
        //获取mTreeViewItem
        private ObservableCollection<CTANBearing> _parentBearingList;
        public ObservableCollection<CTANBearing> ParentBearingList
        {
            get { return _parentBearingList; }
            set
            {
                _parentBearingList = value;
                // 直接设置ComboBox的ItemsSource，无需等待Loaded事件
                bearingsselectcombobox.ItemsSource = _parentBearingList;
                bearingsselectcombobox.DisplayMemberPath = "mTreeViewItem.Header";
                
            }
        }

        public CTANBearing ParentCTANBearing { get; set; }//便于后续获取数据存储字典-轴承数据                                                        
        //public CTANBearing SelectedBearing { get; private set; } // 新增属性，保存用户选中的轴承

        public BearingsPara()
        {
            InitializeComponent();           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double.TryParse(distancetextbox.Text, out double distanceTextbox);
            //double.TryParse(innertextbox.Text, out double innerTextbox);
            //double.TryParse(outertextbox.Text, out double outerTextbox);
            //double.TryParse(broadtextbox.Text, out double broadTextbox);
            BearingDistance = distanceTextbox;
            // 添加详细的空值检查



            //BearingOut = outerTextbox;
            //BearingBroad = broadTextbox;
            //IsTaper = tapercheckbox.IsChecked == true;

            //if (directionRadiol.IsChecked == true)
            //{
            //    this.Orientation = "左侧受力";
            //}
            //else if (directionRadior.IsChecked == true)
            //{
            //    this.Orientation = "右侧受力";
            //}
            //else
            //{
            //    this.Orientation = "null";
            //}
            //if(lcrcobobox.IsEnabled == false)//默认未启用taper时候的基准面为轴承中心！
            //{
            //    this.Base = "轴承中心";
            //}
            this.DialogResult = true;
            this.Close();
        }

        //private void tapercheckbox_Checked(object sender, RoutedEventArgs e)
        //{
        //    directionRadiol.IsEnabled = true;
        //    directionRadior.IsEnabled = true;
        //    lcrcobobox.IsEnabled = true;
        //}

        //private void tapercheckbox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    directionRadiol.IsEnabled = false;
        //    directionRadior.IsEnabled = false;
        //    lcrcobobox.IsEnabled = false;
        //}

        private void lcrcobobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedContent = selectedItem.Content.ToString();

                switch (selectedContent)
                {
                    case "左端面":
                        this.Base = "左端面";
                        break;
                    case "中心基准面":
                        this.Base = "中心基准面";
                        break;
                    case "右端面":
                        this.Base = "右端面";
                        break;
                    default:
                        this.Base = "中心基准面";
                        break;
                }
            }
            else
            {
                this.Base = "中心基准面";
                MessageBox.Show("请选择基准面！");
            }
        }

        private void bearingsselectcombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBearing = bearingsselectcombobox.SelectedItem as CTANBearing; //***
            if (selectedBearing != null)
            {
                //MessageBox.Show($"你选择了: {selectedBearing.mTreeViewItem.Header}");
                //SelectedBearing = selectedBearing; // 更新选中轴承
                ParentCTANBearing = selectedBearing; // 直接关联

                if (ParentCTANBearing == null)
                {
                    MessageBox.Show("父轴承对象未设置！");
                    return;
                }

                if (ParentCTANBearing.mDataStorageDict == null)
                {
                    MessageBox.Show("父轴承的数据字典未初始化！");
                    return;
                }

                // 确保键存在
                if (ParentCTANBearing.mDataStorageDict.ContainsKey("轴承宽度B") &&
                    ParentCTANBearing.mDataStorageDict.ContainsKey("轴承内径d") &&
                    ParentCTANBearing.mDataStorageDict.ContainsKey("轴承外径D"))
                {
                    BearingBroad = ParentCTANBearing.mDataStorageDict["轴承宽度B"].Value;
                    BearingIn = ParentCTANBearing.mDataStorageDict["轴承内径d"].Value;
                    BearingOut = ParentCTANBearing.mDataStorageDict["轴承外径D"].Value;
                }
                else
                {
                    MessageBox.Show("字典中缺少必要的键！");
                }

            }
            //var bearingsSelectComboBox = sender as ComboBox;
            //List<CTANBearing> mBearingSelectList = this.ParentBearingList;
            //BearingDistance = mBearingSelect.Distance;
            //BearingIn = mBearingSelect.InnerDiameter;
        }
    }
}
