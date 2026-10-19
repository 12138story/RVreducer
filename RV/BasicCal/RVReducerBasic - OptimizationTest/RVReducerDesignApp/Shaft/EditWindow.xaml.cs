using RVReducerDesignApp;
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

namespace RWPF
{
    /// <summary>
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        public List<DrawAxisDimension> UpdatedDimensions { get; private set; }
        public int axisIndex { get; set; }
        public EditWindow(List<DrawAxisDimension> initialDimensions)
        {
            //InitializeComponent();
            //Plongtextbox.Text = initialDimensions[0].Length.ToString();
            //Pinterdiametertextbox.Text = initialDimensions[0].InnerDiameter.ToString();
            //Poutterdiametertextbox.Text = initialDimensions[0].OuterDiameter.ToString();
            //Pinterdiametercheckbox.IsChecked = initialDimensions[0].IsInnerTapered;
            //Pouterdiametercheckbox.IsChecked = initialDimensions[0].IsOuterTapered;
            //Pmininterdiametertextbox.Text = initialDimensions[0].SmallEndInnerDiameter.ToString();
            //Pminouterdiametertextbox.Text = initialDimensions[0].SmallEndOuterDiameter.ToString();
            //PdirectioninnerL.IsChecked = initialDimensions[0].IsDirectioninnerLChecked;
            //PdirectioninnerR.IsChecked = initialDimensions[0].IsDirectioninnerRChecked;
            //PdirectionouterL.IsChecked = initialDimensions[0].IsDirectionouterLChecked;
            //PdirectionouterR.IsChecked = initialDimensions[0].IsDirectionouterRChecked;
            //Peccentricshaftcheckbox.IsChecked = initialDimensions[0].IsPeccentricshaft;
            //Peccentricdiametertextbox.Text = initialDimensions[0].EccentricDiameter.ToString();
            //Pphasetextbox.Text = initialDimensions[0].Phase.ToString();
            //axisIndex = initialDimensions[0].AxisIndex;
        }

        private void Peditbutton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Plongtextbox.Text, out double newLength) &&
                double.TryParse(Pinterdiametertextbox.Text, out double newInnerDiameter) &&
                double.TryParse(Poutterdiametertextbox.Text, out double newOuterDiameter))
            {
                if (newLength < 0 || newInnerDiameter < 0 || newOuterDiameter < newInnerDiameter)
                {
                    MessageBox.Show("请输入有效的尺寸", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // 锥度相关设置
                bool isInnerTapered = Pinterdiametercheckbox.IsChecked == true;
                bool isOuterTapered = Pouterdiametercheckbox.IsChecked == true;
                double smallEndInnerDiameter = isInnerTapered ? double.Parse(Pmininterdiametertextbox.Text) : newInnerDiameter;
                double smallEndOuterDiameter = isOuterTapered ? double.Parse(Pminouterdiametertextbox.Text) : newOuterDiameter;
                // 获取方向选择信息
                bool isPdirectioninnerL = PdirectioninnerL.IsChecked == true;
                bool isPdirectioninnerR = PdirectioninnerR.IsChecked == true;
                bool isPdirectionouterL = PdirectionouterL.IsChecked == true;
                bool isPdirectionouterR = PdirectionouterR.IsChecked == true;
                //偏心轴相关设置
                bool isPeccentricshaft = Peccentricshaftcheckbox.IsChecked == true;
                //double eccentricDiameter = isPeccentricshaft ? double.Parse(Peccentricdiametertextbox.Text) : 0;
                //double phase = isPeccentricshaft ? double.Parse(Pphasetextbox.Text) : 90;
                double eccentricDiameter = 0;
                if (Peccentricdiametertextbox.IsEnabled)
                {
                    if (double.TryParse(Peccentricdiametertextbox.Text, out double parsedEccentricDiameter))
                    {
                        eccentricDiameter = parsedEccentricDiameter;
                    }
                    else
                    {
                        MessageBox.Show("请输入有效的偏心直径！");
                    }
                }

                double phase = 0; // 默认值
                int sectionType = 1; // 默认值
                if (Pphasetextbox.IsEnabled)
                {
                    if (!string.IsNullOrWhiteSpace(Pphasetextbox.Text))
                    {
                        if (double.TryParse(Pphasetextbox.Text, out double parsedPhase))
                        {
                            sectionType = 2;
                            phase = parsedPhase;
                        }
                        else
                        {
                            MessageBox.Show("请输入有效的相位角！");
                        }
                    }
                }
                // 创建一个 AxisDimensions 对象
                DrawAxisDimension updatedDimension = new DrawAxisDimension(new object[] {
                    newLength,
                    newInnerDiameter,
                    newOuterDiameter,
                    isInnerTapered,
                    isOuterTapered,
                    smallEndInnerDiameter,
                    smallEndOuterDiameter,
                    isPdirectioninnerL,
                    isPdirectioninnerR,
                    isPdirectionouterL,
                    isPdirectionouterR,
                    isPeccentricshaft,
                    eccentricDiameter,
                    phase,
                    sectionType,
                    axisIndex
                });
                // 将该对象添加到 UpdatedDimensions 列表中
                UpdatedDimensions = new List<DrawAxisDimension> { updatedDimension };
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("请输入有效的尺寸", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// <!-- 锥形外径 CheckBox -->
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pouterdiametercheckbox_Checked(object sender, RoutedEventArgs e)
        {
            // 启用方向选择和小端直径输入框
            PdirectionouterL.IsEnabled = true;
            PdirectionouterR.IsEnabled = true;
            Pminouterdiametertextbox.IsEnabled = true;
        }

        private void Pouterdiametercheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            // 禁用方向选择和小端直径输入框
            PdirectionouterL.IsEnabled = false;
            PdirectionouterR.IsEnabled = false;
            Pminouterdiametertextbox.IsEnabled = false;
        }
        /// <summary>
        /// <!-- 锥形内径 CheckBox -->
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pinterdiametercheckbox_Checked(object sender, RoutedEventArgs e)
        {
            // 启用方向选择和小端直径输入框
            PdirectioninnerL.IsEnabled = true;
            PdirectioninnerR.IsEnabled = true;
            Pmininterdiametertextbox.IsEnabled = true;
        }

        private void Pinterdiametercheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            // 禁用方向选择和小端直径输入框
            PdirectioninnerL.IsEnabled = false;
            PdirectioninnerR.IsEnabled = false;
            Pmininterdiametertextbox.IsEnabled = false;
        }
        /// <summary>
        /// <!-- 偏心距 CheckBox -->
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Peccentricshaftcheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Pphasetextbox.IsEnabled = true;
            Peccentricdiametertextbox.IsEnabled = true;
        }

        private void Peccentricshaftcheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Pphasetextbox.IsEnabled = false;
            Peccentricdiametertextbox.IsEnabled = false;
        }
    }

}
