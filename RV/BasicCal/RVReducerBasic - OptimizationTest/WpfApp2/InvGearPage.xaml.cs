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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RVReducerDesignApp
{
    /// <summary>
    /// InvGearPage.xaml 的交互逻辑
    /// </summary>
    public partial class InvGearPage : Page
    {
        string[] mHeadName = new string[]{"名称","单位",
            "齿数","个",
            "法向模数", "毫米" ,
            "法向压力角", "度",
            "螺旋角", "度",
            "旋向", "",
            "中心距", "毫米",
            "齿数", "个",
            "设计齿宽", "毫米",
            "齿顶高系数", "",
            "齿廓变位系数","",
            "顶隙系数", "",
            "齿根圆角系数", "",
            "挖根量", "毫米",
            "齿顶修形量", "微米",
            "齿根修形量", "微米",
            "是否使用滚动角","",
            "跨齿数", "",
            "齿顶倒角量", "毫米",
            "轮幅外圆直径","毫米",
            "齿根高系数","",
            "刀具的齿顶高系数","",
            "刀具类型","",
            "插齿刀齿数","",
            "插齿刀加工中心距","毫米",
            "插齿刀具参数d_a0","毫米",
            "插齿刀基圆直径","毫米",
            "插齿刀齿顶形状直径","毫米",
            "插齿刀具参数alpha_wt0","度",
            "刀具的齿顶圆半径系数rho_aP0","",
            "齿厚的偏差上限","",
            "齿厚的偏差下限","",
            "机械加工余量q_max","",
            "机械加工余量q_min","",
            "齿厚公差T_sn","",
            "齿厚公差T_snv","",
            "h_FaP0","",
            "齿顶修正系数","",
            "Ada_upper","",
            "Ada_lower","",
            "d_y","",
            "齿轮副类型","",
            "行星轮个数","",
            "驱动齿轮","",
            "触点间隙","",};
        public InvGearPage()
        {
            InitializeComponent();

        }
    }
}
