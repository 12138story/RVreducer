using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace RVReducerDesignApp
{
    /// <summary>
    /// BearingRecommondPage.xaml 的交互逻辑
    /// </summary> 
    public partial class BearingRecommondPage : Page
    {
        // 用于数据回传的字典引用
        public CTANDict OutAssignDictPtr { get; set; }

        // 双击选中事件，供外部订阅
        public event EventHandler<BearingData> BearingDoubleClicked;
        public BearingRecommondPage()
        {
            InitializeComponent();
            GenerateColumns(false);
        }
        public void GenerateColumns(bool isTaperedRollerBearing)
        {
            // 清空现有列
            BearingGridView.Columns.Clear();

            // 1. 添加基础公共列
            BearingGridView.Columns.Add(CreateColumn("型号", "Code", 80));
            BearingGridView.Columns.Add(CreateColumn("d", "d", 50));
            BearingGridView.Columns.Add(CreateColumn("D", "D", 50));
            BearingGridView.Columns.Add(CreateColumn("B", "B", 50));
            BearingGridView.Columns.Add(CreateColumn("Cr", "Cr", 70));
            BearingGridView.Columns.Add(CreateColumn("C0r", "C0r", 70));

            // 2. 如果是圆锥滚子轴承，则添加特有列
            if (isTaperedRollerBearing)
            {
                BearingGridView.Columns.Add(CreateColumn("e", "e", 50));
                BearingGridView.Columns.Add(CreateColumn("Y", "Y", 50));
                BearingGridView.Columns.Add(CreateColumn("Yo", "Yo", 50));
            }

            // 3. 添加尾部公共列
            BearingGridView.Columns.Add(CreateColumn("预期寿命", "Life", 100));
        }
        /// <summary>
        /// 辅助方法：快速创建 GridViewColumn
        /// </summary>
        private GridViewColumn CreateColumn(string header, string bindingPath, double width)
        {
            return new GridViewColumn
            {
                Header = header,
                DisplayMemberBinding = new Binding(bindingPath),
                Width = width
            };
        }
        public void LoadData(List<BearingData> bearings)
        {
            BearingListView.ItemsSource = bearings;
        }
        public class BearingData
        {
            public string Code { get; set; }
            public double d { get; set; }
            public double D { get; set; }
            public double B { get; set; }
            public double Cr { get; set; }
            public double C0r { get; set; }
            public double e { get; set; }
            public double Y { get; set; }
            public double Yo { get; set; }
            public double Life { get; set; }
        }
        /// <summary>
        /// ListView 双击事件处理
        /// </summary>
        private void BearingListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 获取选中的项
            BearingData selectedBearing = BearingListView.SelectedItem as BearingData;

            if (selectedBearing == null)
            {
                return; // 没有选中有效项，直接返回
            }

            // 如果有数据字典引用，则进行数据赋值
            if (OutAssignDictPtr != null)
            {
                try
                {
                    // 推荐表格中的 Cr/C0r 已是千牛(kN)，与字典单位一致，直接赋值
                    OutAssignDictPtr["基本额定动载荷Cr"].Value = selectedBearing.Cr;
                    OutAssignDictPtr["基本额定静载荷C0r"].Value = selectedBearing.C0r;

                    OutAssignDictPtr["轴承内径d"].Value = selectedBearing.d;
                    OutAssignDictPtr["轴承外径D"].Value = selectedBearing.D;
                    OutAssignDictPtr["轴承宽度B"].Value = selectedBearing.B;

                    OutAssignDictPtr["轴承内径Fw"].Value = selectedBearing.d;
                    OutAssignDictPtr["轴承外径Ew"].Value = selectedBearing.D;
                    OutAssignDictPtr["轴承宽度Bc"].Value = selectedBearing.B;
                    // 如果是圆锥滚子轴承，还需要设置额外参数
                    if (selectedBearing.e > 0 || selectedBearing.Y > 0 || selectedBearing.Yo > 0)
                    {
                        OutAssignDictPtr["计算系数e"].Value = selectedBearing.e;
                        OutAssignDictPtr["计算系数Y"].Value = selectedBearing.Y;
                        OutAssignDictPtr["计算系数Y0"].Value = selectedBearing.Yo;
                    }

                    MessageBox.Show($"已选择轴承型号: {selectedBearing.Code}\n预期寿命: {selectedBearing.Life:F2} 小时",
                                    "选择成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"数据赋值错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // 触发事件，通知外部
            BearingDoubleClicked?.Invoke(this, selectedBearing);
        }

    }

}
