using HandyControl.Expression.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFBearings;
using ZstdSharp.Unsafe;
using RWPF;
using Kitware.VTK;
using static Kitware.VTK.vtkDrawTexturedElements;
using LibVLCSharp.Shared;
using System.Collections;
using Google.Protobuf.WellKnownTypes;

namespace RVReducerDesignApp
{
    public enum PolygonType
    {
        Axis, //轴段       
        Bearing, //轴承
        Gear, //齿轮
        NeedleRoller,//滚针
        None, //空类型
    }
    /// <summary>
    ///定义一个类用于存储滚针信息
    /// </summary>
    public class NeedleRollerSizeStruct
    {
        public double NeedleRollerDistance;
        public double NeedleRollerd;
        public double NeedleRollerD;
        public double NeedleRollerWidth;
        public string NeedleRollerBaseFace;
        public int NeedleRollerIndex;
        public NeedleRollerSizeStruct(double needleRollerDistance, double innerDiameter, double outerDiameter, double Width, string baseFace, int  needleRollerIndex)
        {
            NeedleRollerDistance = needleRollerDistance;
            NeedleRollerd = innerDiameter;
            NeedleRollerD = outerDiameter;
            NeedleRollerWidth = Width;
            NeedleRollerBaseFace = baseFace;
            NeedleRollerIndex = needleRollerIndex;
        }
    }
    /// <summary>
    ///定义一个类用于存储轴段
    /// </summary>
    public class AxisDimensions
    {
        public double Length;
        public double InnerDiameter;
        public double OuterDiameter;
        public bool IsInnerTapered;
        public bool IsOuterTapered;
        public double SmallEndInnerDiameter;
        public double SmallEndOuterDiameter;
        public bool IsDirectioninnerLChecked;
        public bool IsDirectioninnerRChecked;
        public bool IsDirectionouterLChecked;
        public bool IsDirectionouterRChecked;
        public bool IsPeccentricshaft;
        public double EccentricDiameter;
        public double Phase;
        public int AxisIndex;
        public int SectionType;
        public AxisDimensions()
        {

        }
        public AxisDimensions(double length, double innerDiameter, double outerDiameter, bool isInnerTapered, bool isOuterTapered,
                               double smallEndInnerDiameter, double smallEndOuterDiameter, bool isPdirectioninnerL, bool isPdirectioninnerR,
                               bool isPdirectionouterL, bool isPdirectionouterR,
                               bool isPeccentricshaft, double eccentricDiameter, double phase, int sectionType, int axisIndex)
        {
            Length = length;
            InnerDiameter = innerDiameter;
            OuterDiameter = outerDiameter;
            IsInnerTapered = isInnerTapered;
            IsOuterTapered = isOuterTapered;
            SmallEndInnerDiameter = smallEndInnerDiameter;
            SmallEndOuterDiameter = smallEndOuterDiameter;
            IsDirectioninnerLChecked = isPdirectioninnerL;
            IsDirectioninnerRChecked = isPdirectioninnerR;
            IsDirectionouterLChecked = isPdirectionouterL;
            IsDirectionouterRChecked = isPdirectionouterR;
            IsPeccentricshaft = isPeccentricshaft;
            EccentricDiameter = eccentricDiameter;
            Phase = phase;           
            SectionType = sectionType;
            AxisIndex = axisIndex;
        }
        public int GetSectionType()
        {
            return SectionType;
        }
    }
    /// <summary>
    ///定义一个类用于存储齿轮信息
    /// </summary>
    public class GearSizeStruct
    {
        public double GearDistance;
        public double GearWidth;
        public double GearHeight;
        //public VassalData Geardata;
        public string BaseFace;
        public int GearIndex;
        public GearSizeStruct(double gearDistance, double width, double height, string baseFace, int gearIndex)
        {
            GearDistance = gearDistance;
            GearWidth = width;
            GearHeight = height;
            //Geardata = gearData;
            BaseFace = baseFace;
            GearIndex = gearIndex;
        }
    }
    /// <summary>
    ///定义一个结构体用于存储轴承信息
    /// </summary>
    public class BearingStruct
    {
        public double BearingDistance;
        public double Bearingd;
        public double BearingD;
        public double BearingBroad;
        public bool BearingIsTaper;
        public string BearingBase;
        public string BearingOrientation;
        public int BearingIndex;
        public BearingStruct(double bearingDistance, double bearingd, double bearingD, double bearingBroad, bool bearingIsTaper, string bearingBase, string bearingOrientation, int bearingIndex)
        {
            BearingDistance = bearingDistance;
            Bearingd = bearingd;
            BearingD = bearingD;
            BearingBroad = bearingBroad;
            BearingIsTaper = bearingIsTaper;
            BearingBase = bearingBase;
            BearingOrientation = bearingOrientation;
            BearingIndex = bearingIndex;
        }
    }
    /// <summary>
    ///定义一个结构体用于附属图形对应的曲轴（滚针，轴承，齿轮等）位置信息
    /// </summary>
    public struct VassalData
    {
        public Polygon CurrentPolygon { get; set; }
        public int Tag { get; set; }
        public bool IsPeccentricshaft { get; set; }
        public double Phase { get; set; }
        public double EccentricDiameter { get; set; }
        public double EccentricOffset { get; set; }
        public double LeftBase { get; set; }
        public double RightBase { get; set; }
        public double MiddleBase { get; set; }
    }
    interface IDraw
    {
        void DrawBeam(double x, double y, double length, double innerDiameter, double outerDiameter, bool isInnerTapered, bool isOuterTapered, double smallEndInnerDiameter, double smallEndOuterDiameter,
                                bool isPdirectioninnerL, bool isPdirectioninnerR, bool isPdirectionouterL, bool isPdirectionouterR,
                                bool isPeccentricshaft, double eccentricDiameter, double phase, int index);//x,y为绘制轴段图形的起点
        void DrawBearing(double bearinDistance, double innerDiameter, double outerDiameter, double bearingWidth, bool isTaper, string baseFace, string orientation, int index); // 定义轴承接口，但轴承暂时未设置偏心！
        void DrawGear(double gearDistance, double gearWidth, double gearHeight, string baseFace, int index);//定义齿轮绘制的接口
        void DrawNeedleRoller(double distanceFromStart, double d, double D, double B, string baseFace, int index); // 定义滚针接口
    }
    /// <summary>
    /// CTANGirderPage.xaml 的交互逻辑
    /// </summary>
    public partial class CTANGirderPage : Page, IDraw
    {    
        private Canvas dimensionCanvas = new Canvas();// 定义一个Canvas用于存放总尺寸标注的元素，便于后面单独刷新！
        public List<AxisDimensions> axisDimensions = new List<AxisDimensions>();//存放轴段的列表
        private List<GearSizeStruct> gearSizeStructs = new List<GearSizeStruct>();//存放齿轮的列表
        private List<BearingStruct> bearingStructs = new List<BearingStruct>();//存放轴承的列表
        private List<NeedleRollerSizeStruct> needleRollerSizeStructs = new List<NeedleRollerSizeStruct>();//存放滚针的列表
        private ScaleTransform canvasScaleTransform = new ScaleTransform(1.0, 1.0);
        //private double currentScale = 1.0; // 当前缩放比例
        private const double MinScale = 0.1; // 最小缩放比例
        private const double MaxScale = 10.0; // 最大缩放比例
        private double startX = 50; // 初始X坐标
        private double startY;// 初始Y坐标
        private Polygon highlightedPolygon; // 用于存储当前高亮的多边形
        private Rectangle highlightedRectangle;//用于存储当前高亮的矩形
        private bool stopAdd = false;
        //计数
        private int iGear = 0;
        private int iBearing = 0;
        private int iAxis = 0;
        private int iNeedleRoller = 0;
        // 添加父级引用属性
        public CTANCrankShaft ParentCrankShaft { get; set; }

        public CTANGirderPage()
        {
            InitializeComponent();

            Pcanvas.RenderTransform = canvasScaleTransform;

            dimensionCanvas.IsHitTestVisible = false; //禁用 dimensionCanvas 上的所有鼠标事件（例如点击、鼠标悬停等）
            Pcanvas.Children.Add(dimensionCanvas);
        }

        //确定按钮事件
        private void Psurebutton_Click(object sender, RoutedEventArgs e)
        {
            startY = Pcanvas.ActualHeight / 2;
            double innerDiameter = 0; // 默认值设为0
            if (!string.IsNullOrWhiteSpace(Pinterdiametertextbox.Text))// 内径不为空执行
            {
                if (!double.TryParse(Pinterdiametertextbox.Text, out innerDiameter))//内径不正常执行
                {
                    MessageBox.Show("请输入有效的内径尺寸！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (double.TryParse(Plongtextbox.Text, out double axisLength) &&
                double.TryParse(Poutterdiametertextbox.Text, out double outerDiameter))
            {
                if (axisLength < 0 || innerDiameter < 0 || outerDiameter < innerDiameter)
                {
                    MessageBox.Show("输入尺寸不符合实际要求！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //执行轴段的绘制
                // 锥度相关设置
                bool isInnerTapered = Pinterdiametercheckbox.IsChecked == true;
                bool isOuterTapered = Pouterdiametercheckbox.IsChecked == true;
                double smallEndInnerDiameter = isInnerTapered ? double.Parse(Pmininterdiametertextbox.Text) : innerDiameter;
                double smallEndOuterDiameter = isOuterTapered ? double.Parse(Pminouterdiametertextbox.Text) : outerDiameter;
                // 获取方向选择信息
                bool isPdirectioninnerL = PdirectioninnerL.IsChecked == true;
                bool isPdirectioninnerR = PdirectioninnerR.IsChecked == true;
                bool isPdirectionouterL = PdirectionouterL.IsChecked == true;
                bool isPdirectionouterR = PdirectionouterR.IsChecked == true;
                //偏心轴相关设置
                bool isPeccentricshaft = Peccentricshaftcheckbox.IsChecked == true;
                double eccentricDiameter = 0;
                int sectionType = 1; // 默认为1对应截面是同心圆
                if (Peccentricdiametertextbox.IsEnabled)
                {
                    if (double.TryParse(Peccentricdiametertextbox.Text, out double parsedEccentricDiameter))
                    {
                        eccentricDiameter = parsedEccentricDiameter;
                        sectionType = 2; //2对应偏心圆
                    }
                    else
                    {
                        MessageBox.Show("请输入有效的偏心直径！");
                    }
                }
                double phase = 90; // 相位角默认值                
                if (Pphasetextbox.IsEnabled)
                {
                    if (!string.IsNullOrWhiteSpace(Pphasetextbox.Text))
                    {
                        if (double.TryParse(Pphasetextbox.Text, out double parsedPhase))
                        {
                            phase = parsedPhase;
                        }
                        else
                        {
                            MessageBox.Show("请输入有效的相位角！");
                        }
                    }
                }
                if (stopAdd == false) //默认执行下面代码！当执行某个轴段的修改功能时，跳过这段代码
                {
                    // 将尺寸添加到列表中
                    axisDimensions.Add(new AxisDimensions(axisLength, innerDiameter, outerDiameter, isInnerTapered, isOuterTapered,
                        smallEndInnerDiameter, smallEndOuterDiameter, isPdirectioninnerL, isPdirectioninnerR, isPdirectionouterL, isPdirectionouterR,
                        isPeccentricshaft, eccentricDiameter, phase, sectionType, iAxis));
                    iAxis++;
                }              
                dimensionCanvas.Children.Clear();//清除绘制的尺寸标注   
                UpdateDraw(sender, e);//遍历画布列表，更新图形            
            }
            else
            {
                MessageBox.Show("请输入有效的尺寸！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //绘制3D模型,这部分使得绘制卡顿，需要更换在切换3d页面的时候来实现！
            //Test3DPage test3DPage = new Test3DPage();
            Test3DPage test3DPage = this.ParentCrankShaft.m3DTestPage; // 获取CTANCrankShaft中已有的实例
            test3DPage.AxisDimensions = axisDimensions;
            //this.NavigationService.Navigate(test3DPage); //可在第一个界面显示三维模型
            //this.ParentCrankShaft.Target3DFrame.Content = test3DPage;// 直接设置目标Frame的Content
            test3DPage.InitializeVtk(); //先初始化，可避免未点击“3D模型”界面出现“渲染未完成等错误”
            test3DPage.UpdateModelButton_Click(sender, e);


        }
        //绘制多边形(轴段)
        public void DrawBeam(double x, double y, double length, double innerDiameter, double outerDiameter, bool isInnerTapered, bool isOuterTapered, double smallEndInnerDiameter, double smallEndOuterDiameter,
                                bool isPdirectioninnerL, bool isPdirectioninnerR, bool isPdirectionouterL, bool isPdirectionouterR,
                                bool isPeccentricshaft, double eccentricDiameter, double phase, int index)
        {
            // 绘制外径的多边形
            Polygon outerPolygon = new Polygon
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
                Fill = Brushes.White,  // 使用浅色填充外径
                Tag = Tuple.Create(index, PolygonType.Axis)
            };
            // 绘制内径的多边形
            Polygon innerPolygon = new Polygon
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
                Fill = Brushes.White, // 使用白色填充内径
                StrokeDashArray = new DoubleCollection { 2, 2 }, // 设置虚线
                Tag = Tuple.Create(index, PolygonType.Axis)
            };
            double radians = phase * Math.PI / 180;
            //若偏心，则为True
            if (isPeccentricshaft)
            {
                // 判断外径锥度的方向并绘制
                if (isOuterTapered && isPdirectionouterL == true)
                {
                    outerPolygon.Points = new PointCollection
                    {
                        new Point(x, y - outerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),                     // 左上角（大端外径）
                        new Point(x + length, y - smallEndOuterDiameter / 2 - eccentricDiameter * Math.Cos(radians)),     // 右上角（小端外径）
                        new Point(x + length, y + smallEndOuterDiameter / 2 - eccentricDiameter * Math.Cos(radians)),     // 右下角（小端外径）
                        new Point(x, y + outerDiameter / 2 - eccentricDiameter * Math.Cos(radians))                      // 左下角（大端外径）
                    };
                }
                else if (isOuterTapered && isPdirectionouterR == true)
                {
                    outerPolygon.Points = new PointCollection
                    {
                        new Point(x, y - smallEndOuterDiameter / 2 - eccentricDiameter * Math.Cos(radians)),              // 左上角（小端外径）
                        new Point(x + length, y - outerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),             // 右上角（大端外径）
                        new Point(x + length, y + outerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),             // 右下角（大端外径）
                        new Point(x, y + smallEndOuterDiameter / 2 - eccentricDiameter * Math.Cos(radians))               // 左下角（小端外径）
                    };
                }
                else
                {
                    outerPolygon.Points = new PointCollection
                    {
                        new Point(x, y - outerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),
                        new Point(x + length, y - outerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),
                        new Point(x + length, y + outerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),
                        new Point(x, y + outerDiameter / 2 - eccentricDiameter * Math.Cos(radians))
                    };
                }
                // 判断内径锥度的方向并绘制
                if (isInnerTapered && isPdirectioninnerL == true)
                {
                    innerPolygon.Points = new PointCollection
                    {
                        new Point(x, y - innerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),                     // 左上角（大端内径）
                        new Point(x + length, y - smallEndInnerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),     // 右上角（小端内径）
                        new Point(x + length, y + smallEndInnerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),     // 右下角（小端内径）
                        new Point(x, y + innerDiameter / 2 - eccentricDiameter * Math.Cos(radians))                      // 左下角（大端内径）
                    };
                }
                else if (isInnerTapered && isPdirectioninnerR == true)
                {
                    innerPolygon.Points = new PointCollection
                    {
                        new Point(x, y - smallEndInnerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),              // 左上角（小端内径）
                        new Point(x + length, y - innerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),             // 右上角（大端内径）
                        new Point(x + length, y + innerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),             // 右下角（大端内径）
                        new Point(x, y + smallEndInnerDiameter / 2 - eccentricDiameter * Math.Cos(radians))               // 左下角（小端内径）
                    };
                }
                else
                {
                    innerPolygon.Points = new PointCollection
                    {
                        new Point(x, y - innerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),
                        new Point(x + length, y - innerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),
                        new Point(x + length, y + innerDiameter / 2 - eccentricDiameter * Math.Cos(radians)),
                        new Point(x, y + innerDiameter / 2 - eccentricDiameter * Math.Cos(radians))
                    };
                }
            }
            //绘制无偏心距的轴段
            else
            {
                // 判断外径锥度的方向并绘制
                if (isOuterTapered && isPdirectionouterL == true)
                {
                    outerPolygon.Points = new PointCollection
                {
                    new Point(x, y - outerDiameter / 2),                     // 左上角（大端外径）
                    new Point(x + length, y - smallEndOuterDiameter / 2),     // 右上角（小端外径）
                    new Point(x + length, y + smallEndOuterDiameter / 2),     // 右下角（小端外径）
                    new Point(x, y + outerDiameter / 2)                      // 左下角（大端外径）
                };
                }
                else if (isOuterTapered && isPdirectionouterR == true)
                {
                    outerPolygon.Points = new PointCollection
                {
                    new Point(x, y - smallEndOuterDiameter / 2),              // 左上角（小端外径）
                    new Point(x + length, y - outerDiameter / 2),             // 右上角（大端外径）
                    new Point(x + length, y + outerDiameter / 2),             // 右下角（大端外径）
                    new Point(x, y + smallEndOuterDiameter / 2)               // 左下角（小端外径）
                };
                }
                else
                {
                    outerPolygon.Points = new PointCollection
                {
                    new Point(x, y - outerDiameter / 2),
                    new Point(x + length, y - outerDiameter / 2),
                    new Point(x + length, y + outerDiameter / 2),
                    new Point(x, y + outerDiameter / 2)
                };
                }

                // 判断内径锥度的方向并绘制
                if (isInnerTapered && isPdirectioninnerL == true)
                {
                    innerPolygon.Points = new PointCollection
                {
                    new Point(x, y - innerDiameter / 2),                     // 左上角（大端内径）
                    new Point(x + length, y - smallEndInnerDiameter / 2),     // 右上角（小端内径）
                    new Point(x + length, y + smallEndInnerDiameter / 2),     // 右下角（小端内径）
                    new Point(x, y + innerDiameter / 2)                      // 左下角（大端内径）
                };
                }
                else if (isInnerTapered && isPdirectioninnerR == true)
                {
                    innerPolygon.Points = new PointCollection
                {
                    new Point(x, y - smallEndInnerDiameter / 2),              // 左上角（小端内径）
                    new Point(x + length, y - innerDiameter / 2),             // 右上角（大端内径）
                    new Point(x + length, y + innerDiameter / 2),             // 右下角（大端内径）
                    new Point(x, y + smallEndInnerDiameter / 2)               // 左下角（小端内径）
                };
                }
                else
                {
                    innerPolygon.Points = new PointCollection
                {
                    new Point(x, y - innerDiameter / 2),
                    new Point(x + length, y - innerDiameter / 2),
                    new Point(x + length, y + innerDiameter / 2),
                    new Point(x, y + innerDiameter / 2)
                };
                }
            }
            Pcanvas.Children.Add(outerPolygon);// 添加外径到画布
            Pcanvas.Children.Add(innerPolygon); // 添加内径到画布
        }

        //画布的右键触发事件
        private void Pcanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            int index = -1;// 默认值，表示未找到匹配的多边形
            UIElement targetShape = null;
            Point clickPosition = e.GetPosition(Pcanvas);// 获取鼠标点击位置            
            PolygonType? polygonType = null; // 使用可空类型来初始化,该部分是为了区分类型，优化删除操作
            // 遍历所有子元素，检查右键点击位置是否在某个多边形内||是否存在高亮显示的图形
            foreach (var child in Pcanvas.Children)
            {
                if (child is Polygon polygon && polygon.Tag is Tuple<int, PolygonType> tagPolygon)
                {
                    if (IsPointInsidePolygon(polygon, clickPosition))
                    {
                        index = tagPolygon.Item1;
                        targetShape = polygon;
                        polygonType = tagPolygon.Item2;
                        break;
                    }
                    if (highlightedPolygon != null && highlightedPolygon.Tag != null)
                    {
                        var tag1 = highlightedPolygon.Tag as Tuple<int, PolygonType>;
                        index = tag1.Item1; // 找到点击的多边形
                        targetShape = polygon;
                        polygonType = tag1.Item2;
                        break;
                    }
                }
            }
            // 如果没有找到点击的多边形，默认操作最后一个Polygon
            if (index == -1)
            {
                foreach (var child in Pcanvas.Children)
                {
                    if (child is Polygon polygon && polygon.Tag is Tuple<int, PolygonType> tagValue)
                    {
                        index = tagValue.Item1; //获取最后一个多边形的索引
                        targetShape = polygon;
                        polygonType = tagValue.Item2;
                    }
                }
            }
            //根据找到的多边形生成菜单
            if (targetShape != null)
            {
                MenuItem editMenu = new MenuItem { Header = "修改轴段" };
                editMenu.Click += (s, e) => EditBeam(index);
                contextMenu.Items.Add(editMenu);

                MenuItem deleteMenu = new MenuItem { Header = "删除" };
                //deleteMenu.Click += (s, e) => DeleteBeam(index, targetShape, (PolygonType)polygonType, (RectangleType)rectangleType);
                deleteMenu.Click += (s, e) =>
                {
                    if (polygonType.HasValue)
                    {
                        DeleteBeam(index, targetShape, polygonType.Value);
                    }
                    else
                    {
                        MessageBox.Show("无法确定图形类型。");
                    }
                };
                contextMenu.Items.Add(deleteMenu);

                MenuItem addGearMenu = new MenuItem { Header = "添加齿轮" };
                addGearMenu.Click += (s, e) => AddGearBeam();
                contextMenu.Items.Add(addGearMenu);

                MenuItem addBearingsMenu = new MenuItem { Header = "添加轴承" };
                addBearingsMenu.Click += (s, e) => AddBearingsBeam(index);
                contextMenu.Items.Add(addBearingsMenu);

                MenuItem addCycloidalPinwheel = new MenuItem { Header = "添加滚针" };
                addCycloidalPinwheel.Click += (s, e) => AddNeedleRoller();
                contextMenu.Items.Add(addCycloidalPinwheel);

                //测试多边形整体平移
                MenuItem testMenu = new MenuItem { Header = "测试" };
                testMenu.Click += (s, e) => Test();
                contextMenu.Items.Add(testMenu);

            }
            else
            {
                MenuItem defaultMenu = new MenuItem { Header = "不可操作" };
                contextMenu.Items.Add(defaultMenu);
            }
            contextMenu.IsOpen = true; // 显示菜单
        }
        //画布的左键点击事件
        private void Pcanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(Pcanvas);
            foreach (var child in Pcanvas.Children)
            {
                if (child is Polygon polygon && polygon.Tag is Tuple<int, PolygonType> tagPolygon)
                {
                    PolygonType type = tagPolygon.Item2;
                    if (IsPointInsidePolygon(polygon, point))
                    {
                        // 取消之前的高亮
                        if (highlightedPolygon != null)
                        {
                            highlightedPolygon.Stroke = Brushes.Gray;
                            highlightedPolygon.StrokeThickness = 1;
                            highlightedPolygon = null;
                        }
                        if (highlightedRectangle != null)
                        {
                            highlightedRectangle.Stroke = Brushes.Gray;
                            highlightedRectangle.StrokeThickness = 1;
                            highlightedRectangle = null;
                        }
                        // 高亮当前选中的图形
                        highlightedPolygon = polygon;
                        highlightedPolygon.Stroke = Brushes.Red;
                        highlightedPolygon.StrokeThickness = 3;
                        highlightedPolygon.Tag = polygon.Tag;
                        int index = tagPolygon.Item1;
                        //显示尺寸信息
                        if (type == PolygonType.Axis)
                        {
                            UpdateDimensionsInfo(index, axisDimensions[index].Length, axisDimensions[index].InnerDiameter, axisDimensions[index].OuterDiameter, axisDimensions[index].SmallEndInnerDiameter,
                                axisDimensions[index].SmallEndOuterDiameter, axisDimensions[index].EccentricDiameter, axisDimensions[index].Phase, axisDimensions[index].SectionType);
                            break;
                        }
                        else if (type == PolygonType.Bearing)
                        {
                            UpdateBearingDimensionsInfo(index, bearingStructs[index].BearingDistance, bearingStructs[index].Bearingd, bearingStructs[index].BearingD, bearingStructs[index].BearingBroad,
                                bearingStructs[index].BearingIsTaper, bearingStructs[index].BearingBase, bearingStructs[index].BearingOrientation);
                            break;
                        }
                        else if (type == PolygonType.Gear)
                        {
                            UpdateGearDimensionsInfo(index, gearSizeStructs[index].GearDistance, gearSizeStructs[index].GearWidth, gearSizeStructs[index].GearHeight, gearSizeStructs[index].BaseFace);
                            break;
                        }
                        else if (type == PolygonType.NeedleRoller)
                        {
                            UpdateNeedleRollerDimensionsInfo(index, needleRollerSizeStructs[index].NeedleRollerDistance, needleRollerSizeStructs[index].NeedleRollerWidth, needleRollerSizeStructs[index].NeedleRollerBaseFace,
                                needleRollerSizeStructs[index].NeedleRollerd, needleRollerSizeStructs[index].NeedleRollerD);
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Error：画布的左键点击事件中判定高亮Polygon是Axis还是Bearing出现错误！");
                        }
                    }
                }              
            }
        }
        // 判断点是否在多边形内的方法
        private bool IsPointInsidePolygon(Polygon polygon, Point point)
        {
            return polygon.RenderedGeometry.FillContains(point);
        }
        //根据鼠标左键点击的轴段更新页面中的轴段信息
        private void UpdateDimensionsInfo(int index, double length, double innerDiameter, double outerDiameter,
            double smallEndInnerDiameter, double smallEndOuterDiameter, double eccentricDiameter, double phase, int sectionType)
        {
            // 创建数据
            var pageInfoList = new List<DimensionInfo>
            {
                new DimensionInfo { Label = "名称：", Value = $"轴段{index + 1}" },
                new DimensionInfo { Label = "长度：(mm)", Value = length.ToString() },
                new DimensionInfo { Label = "孔径：(mm)", Value = innerDiameter.ToString() },
                new DimensionInfo { Label = "外径：(mm)", Value = outerDiameter.ToString() },
                new DimensionInfo { Label = "小端孔径：(mm)", Value = smallEndInnerDiameter.ToString() },
                new DimensionInfo { Label = "小端外径：(mm)", Value = smallEndOuterDiameter.ToString() },
                new DimensionInfo { Label = "偏心距：(mm)" , Value = eccentricDiameter.ToString() },
                new DimensionInfo { Label = "相位角：(°)", Value = phase.ToString() },
                new DimensionInfo { Label = "截面类型：", Value = sectionType.ToString() }
            };

            // 创建页面实例
            var dimensionPage = new DimensionPage();
            dimensionPage.UpdateDimensions(pageInfoList);            
            Pdefineframe.Navigate(dimensionPage);// 将页面导航到 Frame 中，但会在“参数输入”界面中多出一个选项卡，导致在该界面的canvas画布下移，绘制的图形整体上移
        }
        private void UpdateGearDimensionsInfo(int index, double distanceBase, double gearWidth, double gearHight, string gearBase)
        {
            // 创建数据
            var pageGearInfoList = new List<DimensionInfo>
            {
                new DimensionInfo { Label = "名称：", Value = $"齿轮{index + 1}" },
                new DimensionInfo { Label = "偏离基准：(mm)", Value = distanceBase.ToString() },
                new DimensionInfo { Label = "齿轮宽度：(mm)", Value = gearWidth.ToString() },
                new DimensionInfo { Label = "齿轮高度：(mm)", Value = gearHight.ToString() },
                new DimensionInfo { Label = "基准面：(mm)", Value = gearBase.ToString() },
            };
            // 创建页面实例
            var gearDimensionPage = new DimensionPage();
            gearDimensionPage.UpdateDimensions(pageGearInfoList);           
            Pdefineframe.Navigate(gearDimensionPage);// 将页面导航到 Frame 中
        }
        private void UpdateNeedleRollerDimensionsInfo(int index, double needleRollerDistance, double needleRollerWidth, string needleRollerBaseFace, double needleRollerInnerDiameter, double needleRollerOuterDiameter)
        {
            // 创建数据
            var pageNeedleRollerInfoList = new List<DimensionInfo>
            {
                new DimensionInfo { Label = "名称：", Value = $"滚针{index + 1}" },
                new DimensionInfo { Label = "偏离基准：(mm)", Value = needleRollerDistance.ToString() },
                new DimensionInfo { Label = "滚针宽度：(mm)", Value = needleRollerWidth.ToString() },
                new DimensionInfo { Label = "基准面：", Value = needleRollerBaseFace.ToString() },
                new DimensionInfo { Label = "滚针内径：(mm)", Value = needleRollerInnerDiameter.ToString() },
                new DimensionInfo { Label = "滚针外径：(mm)", Value = needleRollerOuterDiameter.ToString() },
            };
            // 创建页面实例
            var needleRollerDimensionPage = new DimensionPage();
            needleRollerDimensionPage.UpdateDimensions(pageNeedleRollerInfoList);
            Pdefineframe.Navigate(needleRollerDimensionPage);// 将页面导航到 Frame 中
        }
        private void UpdateBearingDimensionsInfo(int index, double bearingDistance, double bearingd, double bearingD, double bearingBroad, bool bearingIsTaper, string bearingBase, string bearingOrientation)
        {
            // 创建数据
            var pageBearingInfoList = new List<DimensionInfo>
            {
                new DimensionInfo { Label = "名称：", Value = $"轴承{index + 1}" },
                new DimensionInfo { Label = "偏离基准：(mm)", Value = bearingDistance.ToString() },
                new DimensionInfo { Label = "d：(mm)", Value = bearingd.ToString() },
                new DimensionInfo { Label = "D：(mm)", Value = bearingD.ToString() },
                new DimensionInfo { Label = "轴承宽度：(mm)", Value = bearingBroad.ToString() },
                new DimensionInfo { Label = "是否为锥形轴承：", Value = bearingIsTaper.ToString() },
                new DimensionInfo { Label = "轴承基准面：", Value = bearingBase.ToString() },
                new DimensionInfo { Label = "轴承方向：", Value = bearingOrientation.ToString() },
            };
            // 创建页面实例
            var bearingDimensionPage = new DimensionPage();
            bearingDimensionPage.UpdateDimensions(pageBearingInfoList);
            Pdefineframe.Navigate(bearingDimensionPage);// 将页面导航到 Frame 中
        }
        //右键菜单修改功能实现
        private void EditBeam(int index)
        {
            // 检查索引是否合法
            if (index < 0 || index >= axisDimensions.Count)
            {
                MessageBox.Show("无效的索引，无法编辑！");
                return;
            }
            // 获取一个对象
            var dimensions = axisDimensions[index];
            List<AxisDimensions> listdimensions = new List<AxisDimensions> { dimensions };
            EditWindow editWindow = new EditWindow(listdimensions);
            // 显示编辑窗口并处理更新
            if (editWindow.ShowDialog() == true)
            {
                // 从 editWindow 中获取更新后的 AxisDimensions 对象并更新 
                axisDimensions[index] = editWindow.UpdatedDimensions[0];
                stopAdd = true;//禁用Psurebutton_Click中部分代码
                Psurebutton_Click(null, null); // 重新绘制
                stopAdd = false;//恢复默认状态
            }
        }
        // 右键菜单删除功能实现
        private void DeleteBeam(int index, UIElement targetShape, PolygonType? polygonType)
        {
            // 根据类型执行相应的操作
            if (polygonType == PolygonType.Axis)
            {
                // 检查索引是否合法
                if (index < 0 || index >= axisDimensions.Count)
                {
                    MessageBox.Show("Error:无效的轴段索引，无法删除！");
                    return;
                }
                // 确认删除操作
                var result = MessageBox.Show($"确定要删除第{index + 1}个轴段吗？", "删除确认", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    axisDimensions.RemoveAt(index);// 删除轴尺寸和对应的UI元素                                                       
                    dimensionCanvas.Children.Clear();// 清除上一次的总尺寸标注
                    UpdateDraw(null, null); // 遍历列表中图形并进行重新绘制
                    MessageBox.Show($"轴段 {index + 1} 已删除！");
                }
            }
            else if (polygonType == PolygonType.Bearing )
            {
                if (index < 0 || index >= bearingStructs.Count)
                {
                    MessageBox.Show("Error:无效的齿轮索引，无法删除！");
                    return;
                }
                var result = MessageBox.Show($"确定要删除第{index + 1}个轴承吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bearingStructs.RemoveAt(index);
                    UpdateDraw(null, null); // 遍历列表中图形并进行重新绘制
                    MessageBox.Show($"轴承{index + 1}已删除！");
                }
            }
            else if (polygonType == PolygonType.Gear)
            {
                if (index < 0 || index >= gearSizeStructs.Count)
                {
                    MessageBox.Show("Error:无效的齿轮索引，无法删除！");
                    return;
                }
                var result = MessageBox.Show($"确定要删除第{index + 1}个齿轮吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    gearSizeStructs.RemoveAt(index);
                    UpdateDraw(null, null); // 遍历列表中图形并进行重新绘制
                    MessageBox.Show($"齿轮{index + 1}已删除！");
                }
            }
            else
            {
                MessageBox.Show("Error:删除操作找不到对应类型！可能为null值");
            }                 
        }
        // 右键齿轮功能实现
        private void AddGearBeam()
        {
            //给定输入参数
            double distanceFromStart = 10;
            double gearWidth = 10;
            double gearHight = 100;
            string baseFace = "右端面";
            
            DrawGear(distanceFromStart, gearWidth, gearHight, baseFace, iGear);
            gearSizeStructs.Add(new GearSizeStruct(distanceFromStart, gearWidth, gearHight, baseFace, iGear));
            iGear++;
        }
        //齿轮绘制函数
        public void DrawGear(double distanceFromStart, double gearWidth, double gearHeight, string baseFace, int index)
        {
            VassalData vassalData = GetVassalGraphics(distanceFromStart);//得到附属图形位置（可选）齿轮
            DrawPolygon(distanceFromStart, 0, gearHeight, gearWidth, baseFace, index, "齿轮", vassalData);
        }
        //右键轴承功能实现
        private void AddBearingsBeam(int index)
        {
            if(this.ParentCrankShaft?.ParentCrankShaftSystem != null)
            {
                var bearingsPara = new BearingsPara();
                bearingsPara.ParentBearingList = this.ParentCrankShaft.ParentCrankShaftSystem.mBearingList;
                bool? result = bearingsPara.ShowDialog();
                if (result == true && bearingsPara.ParentCTANBearing != null)
                {
                    //CTANBearing selectedBearing = bearingsPara.SelectedBearing;
                    double d = bearingsPara.BearingIn;
                    double D = bearingsPara.BearingOut;
                    double B = bearingsPara.BearingBroad;
                    //bool isTaper = bearingsPara.IsTaper == true;
                    //string Orientation = bearingsPara.Orientation;
                    string Base = bearingsPara.Base;//以什么为基准，左端面，右端面，中心。
                    double distanceFromStart = bearingsPara.BearingDistance;// 需要用户提供
                    bool isTaper = false;
                    string Orientation = "null";

                    DrawBearing(distanceFromStart, d, D, B, isTaper, Base, Orientation, iBearing);
                    bearingStructs.Add(new BearingStruct(distanceFromStart, d, D, B, isTaper, Base, Orientation, iBearing));
                    iBearing++;
                }
                else
                {
                    MessageBox.Show("用户取消输入齿轮信息！");
                }
            }
            else
            {
                MessageBox.Show("未找到曲轴系统！");
            }
        }
        // 绘制轴承
        public void DrawBearing(double distanceFromStart, double innerDiameter, double outerDiameter, double B, bool isTaper, string baseFace, string orientation, int index)
        {
            //添加判断是否为锥形轴！
            if (isTaper)
            {
                double y = Pcanvas.ActualHeight / 2;
                // 计算轴承的矩形位置
                double bearingX = startX + distanceFromStart; // 基于基准距离
                double bearingHeight = outerDiameter - innerDiameter; // 轴承高度为内外径之差
                                                                      // 定义斜虚线填充的 DrawingBrush
                DrawingBrush diagonalHatchBrush = new DrawingBrush
                {
                    TileMode = TileMode.Tile,
                    Viewport = new Rect(0, 0, 10, 10), // 视口大小设置为10x10的正方形
                    ViewportUnits = BrushMappingMode.Absolute,
                    Drawing = new GeometryDrawing
                    {
                        Brush = Brushes.Transparent,
                        Pen = new Pen(Brushes.Black, 1),
                        Geometry = new LineGeometry(new Point(0, 10), new Point(10, 0)) // 斜线从左下角到右上角
                    }
                };
                // 绘制外径的多边形
                Polygon upPolygon = new Polygon
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = diagonalHatchBrush, // 使用斜虚线填充
                    Tag = Tuple.Create(index, PolygonType.Bearing)
                };
                // 绘制内径的多边形
                Polygon downPolygon = new Polygon
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = diagonalHatchBrush, // 使用斜虚线填充
                    Tag = Tuple.Create(index, PolygonType.Bearing)
                };
                if (orientation == "左侧受力")
                {
                    switch (baseFace)
                    {
                        case "左端面":
                            upPolygon.Points = new PointCollection
                            {
                                // 定义左侧受力，以左侧为基准的锥齿轮图形，上侧
                                new Point(bearingX, y - outerDiameter / 2),
                                new Point(bearingX + 0.87 * B, y - outerDiameter / 2),
                                new Point(bearingX + 0.87 * B, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX + B, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX + B, y - innerDiameter / 2),
                                new Point(bearingX + 0.13 * B, y - innerDiameter / 2),
                                new Point(bearingX + 0.13 * B, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX, y - innerDiameter / 2 - 0.3 * bearingHeight)
                            };
                            downPolygon.Points = new PointCollection
                            {
                                // 定义左侧受力，以左侧为基准的锥齿轮图形，下侧
                                new Point(bearingX, y + outerDiameter / 2),
                                new Point(bearingX + 0.87 * B, y + outerDiameter / 2),
                                new Point(bearingX + 0.87 * B, y + innerDiameter / 2 + 0.2 * bearingHeight),
                                new Point(bearingX + B, y + innerDiameter / 2 + 0.2 * bearingHeight),
                                new Point(bearingX + B, y + innerDiameter / 2),
                                new Point(bearingX + 0.13 * B, y + innerDiameter / 2),
                                new Point(bearingX + 0.13 * B, y + innerDiameter / 2 + 0.3 * bearingHeight),
                                new Point(bearingX, y + innerDiameter / 2 + 0.3 * bearingHeight),
                            };
                            break;
                        case "中心基准面":
                            upPolygon.Points = new PointCollection
                            {
                                // 定义左侧受力，以中心基准面为基准的锥齿轮图形
                                new Point(bearingX - B / 2, y - outerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B / 2, y - outerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B / 2, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX + B - B / 2, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX + B - B / 2, y - innerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B / 2, y - innerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B / 2, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX - B / 2, y - innerDiameter / 2 - 0.3 * bearingHeight),
                            };
                            downPolygon.Points = new PointCollection
                            {
                                 // 定义左侧受力，以中心基准面为基准的锥齿轮图形
                                new Point(bearingX - B / 2, y + outerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B / 2, y + outerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B / 2, y + innerDiameter / 2 + 0.2 * bearingHeight),
                                new Point(bearingX + B - B / 2, y + innerDiameter / 2 + 0.2 * bearingHeight),
                                new Point(bearingX + B - B / 2, y + innerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B / 2, y + innerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B / 2, y + innerDiameter / 2 + 0.3 * bearingHeight),
                                new Point(bearingX - B / 2, y + innerDiameter / 2 + 0.3 * bearingHeight),
                            };
                            break;
                        case "右端面":
                            upPolygon.Points = new PointCollection
                            {
                                // 定义左侧受力，以右侧为基准的锥齿轮图形
                                new Point(bearingX - B, y + outerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B, y + outerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B, y + innerDiameter / 2 + 0.2 * bearingHeight),
                                new Point(bearingX, y + innerDiameter / 2 + 0.2 * bearingHeight),
                                new Point(bearingX, y + innerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B, y + innerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B, y + innerDiameter / 2 + 0.3 * bearingHeight),
                                new Point(bearingX - B, y + innerDiameter / 2 + 0.3 * bearingHeight),
                            };
                            downPolygon.Points = new PointCollection
                            {
                                new Point(bearingX - B, y - outerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B, y - outerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX, y - innerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B, y - innerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX - B, y - innerDiameter / 2 - 0.3 * bearingHeight),
                            };
                            break;
                    }
                    Pcanvas.Children.Add(upPolygon);
                    Pcanvas.Children.Add(downPolygon);
                }
                else if (orientation == "右侧受力")
                {
                    switch (baseFace)
                    {
                        case "左端面":
                            upPolygon.Points = new PointCollection
                            {
                                // 定义右侧受力，以左侧为基准的锥齿轮图形，上侧
                                new Point(bearingX, y - innerDiameter / 2),
                                new Point(bearingX + 0.87 * B, y - innerDiameter / 2),
                                new Point(bearingX + 0.87 * B, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX + B, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX + B, y - outerDiameter / 2),
                                new Point(bearingX + 0.13 * B, y - outerDiameter / 2),
                                new Point(bearingX + 0.13 * B, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX, y - innerDiameter / 2 - 0.2 * bearingHeight),
                            };
                            downPolygon.Points = new PointCollection
                            {
                                // 定义右侧受力，以左侧为基准的锥齿轮图形，下侧
                                new Point(bearingX, y + innerDiameter / 2),
                                new Point(bearingX + 0.87 * B, y + innerDiameter / 2),
                                new Point(bearingX + 0.87 * B, y + innerDiameter / 2 + 0.3 * bearingHeight),
                                new Point(bearingX + B, y + innerDiameter / 2 + 0.3 * bearingHeight),
                                new Point(bearingX + B, y + outerDiameter / 2),
                                new Point(bearingX + 0.13 * B, y + outerDiameter / 2),
                                new Point(bearingX + 0.13 * B, y + innerDiameter / 2 + 0.2 * bearingHeight),
                                new Point(bearingX, y + innerDiameter / 2 + 0.2 * bearingHeight),
                            };
                            break;
                        case "中心基准面":
                            upPolygon.Points = new PointCollection
                            {
                                new Point(bearingX - B / 2, y - innerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B / 2, y - innerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B / 2, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX + B - B / 2, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX + B - B / 2, y - outerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B / 2, y - outerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B / 2, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX - B / 2, y - innerDiameter / 2 - 0.2 * bearingHeight),
                            };
                            downPolygon.Points = new PointCollection
                            {
                                // 定义右侧受力，以中心基准面为基准的锥齿轮图形，下侧
                                new Point(bearingX - B / 2, y + innerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B / 2, y + innerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B / 2, y + innerDiameter / 2 + 0.3 * bearingHeight),
                                new Point(bearingX + B - B / 2, y + innerDiameter / 2 + 0.3 * bearingHeight),
                                new Point(bearingX + B - B / 2, y + outerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B / 2, y + outerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B / 2, y + innerDiameter / 2 + 0.2 * bearingHeight),
                                new Point(bearingX - B / 2, y + innerDiameter / 2 + 0.2 * bearingHeight),
                            };
                            break;
                        case "右端面":
                            upPolygon.Points = new PointCollection
                            {
                                new Point(bearingX - B, y - innerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B, y - innerDiameter / 2),
                                new Point(bearingX + 0.87 * B - B, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX, y - innerDiameter / 2 - 0.3 * bearingHeight),
                                new Point(bearingX, y - outerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B, y - outerDiameter / 2),
                                new Point(bearingX + 0.13 * B - B, y - innerDiameter / 2 - 0.2 * bearingHeight),
                                new Point(bearingX - B, y - innerDiameter / 2 - 0.2 * bearingHeight),
                            };
                            downPolygon.Points = new PointCollection
                            {
                            // 定义右侧受力，以中心基准面为基准的锥齿轮图形，下侧
                            new Point(bearingX - B, y + innerDiameter / 2),
                            new Point(bearingX + 0.87 * B - B, y + innerDiameter / 2),
                            new Point(bearingX + 0.87 * B - B, y + innerDiameter / 2 + 0.3 * bearingHeight),
                            new Point(bearingX, y + innerDiameter / 2 + 0.3 * bearingHeight),
                            new Point(bearingX, y + outerDiameter / 2),
                            new Point(bearingX + 0.13 * B - B, y + outerDiameter / 2),
                            new Point(bearingX + 0.13 * B - B, y + innerDiameter / 2 + 0.2 * bearingHeight),
                            new Point(bearingX - B, y + innerDiameter / 2 + 0.2 * bearingHeight),
                            };
                            break;
                    }
                    Pcanvas.Children.Add(upPolygon);
                    Pcanvas.Children.Add(downPolygon);
                }
                else
                {
                    MessageBox.Show("Error：请选择受力方向！");
                }
            }
            else//正常轴承
            {
                VassalData vassalData = GetVassalGraphics(distanceFromStart);
                DrawPolygon(distanceFromStart, innerDiameter, outerDiameter, B, baseFace, index, "轴承", vassalData);
            }
        }

        //右键滚针功能实现
        private void AddNeedleRoller()
        {
            //给定输入参数
            double distanceFromStart = 50;
            double d = 40;
            double D = 80;
            double B = 30;
            string baseFace = "右端面";

            DrawNeedleRoller(distanceFromStart, d, D, B, baseFace, iNeedleRoller);
            needleRollerSizeStructs.Add(new NeedleRollerSizeStruct(distanceFromStart, d, D, B, baseFace, iNeedleRoller));
            iNeedleRoller++;
        }
        //滚针绘制函数
        public void DrawNeedleRoller(double distanceFromStart, double d, double D, double B, string baseFace, int index)
        {
            VassalData vassalData = GetVassalGraphics(distanceFromStart);//得到附属图形位置（可选）滚针
            DrawPolygon(distanceFromStart, d, D, B, baseFace, index, "滚针", vassalData);

        }
        //测试整个多边形平移代码
        private void Test()
        {
            var polygon = Pcanvas.Children.OfType<Polygon>();           
            foreach (var poly in polygon)
            {
                //创建平移变换
                TranslateTransform translateTransform = new TranslateTransform
                {
                    X = 0,
                    Y = 20
                };                
                poly.RenderTransform = translateTransform;//应用平移变换               
            }
        }
        //用于更新绘制的图形
        private void UpdateDraw(object sender, RoutedEventArgs e)
        {
            double currentX = startX; // 当前X坐标
            double totalLength = 0; // 累积总长度
            double maxOuterDiameter = 0; //获取最大的外径
            Pcanvas.Children.Clear();// 清除现有的图形
            DrawLine(null, null); //创建中心线并添加进Pcanvas
            for (int i = 0; i < axisDimensions.Count; i++)
            {
                // 绘制每一段轴，传递当前索引和锥度信息
                DrawBeam(currentX, startY, axisDimensions[i].Length, axisDimensions[i].InnerDiameter, axisDimensions[i].OuterDiameter, axisDimensions[i].IsInnerTapered, axisDimensions[i].IsOuterTapered,
                    axisDimensions[i].SmallEndInnerDiameter, axisDimensions[i].SmallEndOuterDiameter, axisDimensions[i].IsDirectioninnerLChecked, axisDimensions[i].IsDirectioninnerRChecked,
                    axisDimensions[i].IsDirectionouterLChecked, axisDimensions[i].IsDirectionouterRChecked,
                    axisDimensions[i].IsPeccentricshaft, axisDimensions[i].EccentricDiameter, axisDimensions[i].Phase, i);
                currentX += axisDimensions[i].Length; // 更新X坐标
                totalLength += axisDimensions[i].Length; // 累计总长度
                if (maxOuterDiameter < axisDimensions[i].OuterDiameter + axisDimensions[i].EccentricDiameter * 2)
                {
                    maxOuterDiameter = axisDimensions[i].OuterDiameter + axisDimensions[i].EccentricDiameter * 2;
                }
            }
            // 显示整根梁的总尺寸
            if (totalLength > 0 && axisDimensions.Count > 0)
            {
                ShowTotalDimensions(startX, startY, totalLength, maxOuterDiameter);
            }
            //绘制齿轮
            for (int i = 0; i < gearSizeStructs.Count; i++)
            {
                DrawGear(gearSizeStructs[i].GearDistance, gearSizeStructs[i].GearWidth, gearSizeStructs[i].GearHeight,gearSizeStructs[i].BaseFace, i);
            }
            //绘制轴承
            for (int i = 0; i < bearingStructs.Count; i++)
            {
                DrawBearing(bearingStructs[i].BearingDistance, bearingStructs[i].Bearingd, bearingStructs[i].BearingD, bearingStructs[i].BearingBroad,
                    bearingStructs[i].BearingIsTaper, bearingStructs[i].BearingBase, bearingStructs[i].BearingOrientation, i);
            }
            //绘制滚针
            for (int i = 0; i < needleRollerSizeStructs.Count; i++)
            {
                DrawNeedleRoller(needleRollerSizeStructs[i].NeedleRollerDistance, needleRollerSizeStructs[i].NeedleRollerd, needleRollerSizeStructs[i].NeedleRollerD, needleRollerSizeStructs[i].NeedleRollerWidth, needleRollerSizeStructs[i].NeedleRollerBaseFace, i);
            }
            //更新highlight
            highlightedPolygon = null;
            highlightedRectangle = null;
            //更新缩放
        }
        //中心线绘制
        private void DrawLine(object sender, RoutedEventArgs e)
        {
            // 创建中心线
            Line centerLine = new Line
            {
                X1 = 50,                // 起点X坐标
                Y1 = Pcanvas.ActualHeight / 2, // 起点Y坐标 (Canvas垂直中心)
                X2 = Pcanvas.ActualWidth,      // 终点X坐标
                Y2 = Pcanvas.ActualHeight / 2, // 终点Y坐标 (Canvas垂直中心)
                Stroke = Brushes.Blue,  // 线条颜色
                StrokeThickness = 0.5,    // 线条粗细
                StrokeDashArray = new DoubleCollection() { 1, 2 } // 设置为点划线，1是点的长度，2是间隔
            };
            // 创建垂直中心线
            Line verticalCenterLine = new Line
            {
                X1 = 50,  // 起点X坐标 (Canvas水平中心)
                Y1 = 0,                // 起点Y坐标
                X2 = 50,  // 终点X坐标 (Canvas水平中心)
                Y2 = Pcanvas.ActualHeight,     // 终点Y坐标
                Stroke = Brushes.Blue, // 线条颜色
                StrokeThickness = 0.5    // 线条粗细
            };

            // 将垂直中心线添加到Canvas
            Pcanvas.Children.Add(verticalCenterLine);
            // 将中心线添加到Canvas
            Pcanvas.Children.Add(centerLine);
            Panel.SetZIndex(verticalCenterLine, int.MaxValue);
            Panel.SetZIndex(centerLine, int.MaxValue); // 确保它显示在最上层
        }
        // 显示整根轴的总尺寸
        private void ShowTotalDimensions(double startX, double startY, double totalLength, double outerDiameter)
        {
            double offset = 20; // 总尺寸线的偏移量
            double arrowSize = 5; // 箭头大小
            // 绘制总尺寸线
            Line totalLine = new Line
            {
                X1 = startX,
                Y1 = startY - outerDiameter / 2 - offset,
                X2 = startX + totalLength,
                Y2 = startY - outerDiameter / 2 - offset,
                Stroke = Brushes.Blue,
                StrokeThickness = 1
            };
            dimensionCanvas.Children.Add(totalLine);
            // 绘制左侧箭头
            DrawArrow(dimensionCanvas, startX, startY - outerDiameter / 2 - offset, arrowSize, true);
            // 绘制右侧箭头
            DrawArrow(dimensionCanvas, startX + totalLength, startY - outerDiameter / 2 - offset, arrowSize, false);
            // 显示总尺寸文本
            TextBlock totalDimensionText = new TextBlock
            {
                Text = $"总长度: {totalLength}",
                Foreground = Brushes.Blue,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(totalDimensionText, startX + totalLength / 2 - 20);
            Canvas.SetTop(totalDimensionText, startY - outerDiameter / 2 - offset - 20);
            dimensionCanvas.Children.Add(totalDimensionText);

            // 添加dimensionCanvas到Pcanvas中
            if (!Pcanvas.Children.Contains(dimensionCanvas))
            {
                Pcanvas.Children.Add(dimensionCanvas);
            }
        }
        //绘制箭头
        private void DrawArrow(Canvas canvas, double x, double y, double size, bool isLeft)
        {
            Line arrowLine1 = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = isLeft ? x + size : x - size,
                Y2 = y - size,
                Stroke = Brushes.Blue,
                StrokeThickness = 1
            };
            Line arrowLine2 = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = isLeft ? x + size : x - size,
                Y2 = y + size,
                Stroke = Brushes.Blue,
                StrokeThickness = 1
            };
            canvas.Children.Add(arrowLine1);
            canvas.Children.Add(arrowLine2);
        }
       
        //画布鼠标滚轮缩放大小
        private void Pcanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scaleFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;

            double newScale = canvasScaleTransform.ScaleX * scaleFactor;

            if (newScale < MinScale || newScale > MaxScale)
                return;

            canvasScaleTransform.ScaleX *= scaleFactor;
            canvasScaleTransform.ScaleY *= scaleFactor;
            ////Point mousePosition = e.GetPosition(Pcanvas);
            //double scaleFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;// 缩放因子
            //double newScale = currentScale * scaleFactor;
            //// 限制缩放范围
            //if (newScale < MinScale || newScale > MaxScale)
            //{
            //    return; // 如果超出范围，直接返回
            //}
            //currentScale = newScale;
            //ScaleCanvasElementsFromStartPoint(Pcanvas, dimensionCanvas, scaleFactor, startX, startY);
        }

        //private void ScaleCanvasElementsFromStartPoint(Canvas canvas, Canvas canvas1, double scaleFactor, double startX, double startY)
        //{
        //    currentScale *= scaleFactor;// 更新全局缩放比例
        //    // 遍历Pcanvas画布上的所有子元素
        //    foreach (UIElement element in canvas.Children)
        //    {
        //        // 对多边形进行缩放
        //        if (element is Polygon polygon)
        //        {
        //            ScalePolygon(polygon, scaleFactor, startX, startY, canvas);
        //        }
        //        // 对直线进行缩放
        //        else if (element is Line line)
        //        {
        //            ScaleLine(line, scaleFactor, startX, startY, canvas);
        //        }
        //    }
        //    // 遍历dimensionCanvas画布上的所有子元素
        //    foreach (UIElement element in canvas1.Children)
        //    {
        //        // 对直线、TextBlock等其他形状进行缩放
        //        if (element is Line line)
        //        {
        //            ScaleLine(line, scaleFactor, startX, startY, canvas1);
        //        }
        //        else if (element is TextBlock textBlock)
        //        {
        //            ScaleTextBlock(textBlock, scaleFactor, startX, startY, canvas1);
        //        }
        //    }
        //}
        // 缩放多边形
        //private void ScalePolygon(Polygon polygon, double scaleFactor, double startX, double startY, Canvas canvas)
        //{
        //    // 缩放多边形
        //    for (int i = 0; i < polygon.Points.Count; i++)
        //    {
        //        Point oldPoint = polygon.Points[i];
        //        polygon.Points[i] = new Point(
        //            startX + (oldPoint.X - startX) * scaleFactor,
        //            startY + (oldPoint.Y - startY) * scaleFactor
        //        );
        //    }
        //}
        //// 缩放直线
        //private void ScaleLine(Line line, double scaleFactor, double startX, double startY, Canvas canvas)
        //{
        //    // 缩放直线
        //    line.X1 = startX + (line.X1 - startX) * scaleFactor;
        //    line.Y1 = startY + (line.Y1 - startY) * scaleFactor;
        //    line.X2 = startX + (line.X2 - startX) * scaleFactor;
        //    line.Y2 = startY + (line.Y2 - startY) * scaleFactor;
        //}
        //// 缩放TextBlock及其字体
        //private void ScaleTextBlock(TextBlock textBlock, double scaleFactor, double startX, double startY, Canvas canvas)
        //{
        //    // 缩放文本
        //    double oldLeft = Canvas.GetLeft(textBlock);
        //    double oldTop = Canvas.GetTop(textBlock);
        //    // 缩放字体大小
        //    textBlock.FontSize *= scaleFactor;
        //    // 缩放位置
        //    Canvas.SetLeft(textBlock, startX + (oldLeft - startX) * scaleFactor);
        //    Canvas.SetTop(textBlock, startY + (oldTop - startY) * scaleFactor);
        //}
        ////缩放矩形
        //private void ScaleRectangle(Rectangle rectangle, double scaleFactor, double startX, double startY, Canvas canvas)
        //{
        //    double oldLeft = Canvas.GetLeft(rectangle);
        //    double oldTop = Canvas.GetTop(rectangle);
        //    rectangle.Width *= scaleFactor;
        //    rectangle.Height *= scaleFactor;
        //    Canvas.SetLeft(rectangle, startX + (oldLeft - startX) * scaleFactor);
        //    Canvas.SetTop(rectangle, startY + (oldTop - startY) * scaleFactor);
        //}
        //private void Pcanvas_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (!isDragging) return;

        //    Point currentPosition = e.GetPosition(this);
        //    Vector delta = currentPosition - lastMousePosition;

        //    double newX = canvasTranslateTransform.X + delta.X;
        //    double newY = canvasTranslateTransform.Y + delta.Y;

        //    // 限制边界：确保画布不会拖出 Border 区域
        //    double minX = borderWidth - canvasWidth;
        //    double minY = borderHeight - canvasHeight;

        //    // 边界判断
        //    if (canvasWidth > borderWidth)
        //        newX = Math.Max(minX, Math.Min(0, newX));
        //    else
        //        newX = 0;  // 如果画布比 border 小，禁止横向移动

        //    if (canvasHeight > borderHeight)
        //        newY = Math.Max(minY, Math.Min(0, newY));
        //    else
        //        newY = 0;  // 如果画布比 border 小，禁止纵向移动

        //    canvasTranslateTransform.X = newX;
        //    canvasTranslateTransform.Y = newY;

        //    lastMousePosition = currentPosition;
        //}
        //private void Pcanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    isDragging = true;
        //    lastMousePosition = e.GetPosition(this);
        //    Pcanvas.CaptureMouse();
        //}

        //private void Pcanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    isDragging = false;
        //    Pcanvas.ReleaseMouseCapture();
        //}

        //根据缩放动态设置并调整Canvas裁剪区域
        private void Pcanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 获取画布的当前尺寸
            double canvasWidth = Pcanvas.ActualWidth;
            double canvasHeight = Pcanvas.ActualHeight;
            // 设置裁剪区域
            RectangleGeometry clipGeometry = new RectangleGeometry(new Rect(0, 0, canvasWidth, canvasHeight));
            Pcanvas.Clip = clipGeometry;
        }
        //绘制一个普通多边形（可偏心），是在添加滚针这步加上的
        private void DrawPolygon(double distanceFromStart, double d, double D, double B, string baseFase, int tag, string type, VassalData vassalData)
        {
            //恢复原始大小
            canvasScaleTransform.ScaleX = 1.0;
            canvasScaleTransform.ScaleY = 1.0;
            //获取填充图片
            Brush FillShape = null; 
            PolygonType shape = PolygonType.None;
            switch (type)//根据附属图形类型获取填充图案
            {
                case "滚针":
                    FillShape = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(@"D:\Workspace\softdevelop\RV\BasicCal\RVReducerBasic\RVReducerDesignApp\Resource\Figure\Shaft\centerLine.png", UriKind.RelativeOrAbsolute)),
                        Stretch = Stretch.Fill
                    };
                    shape = PolygonType.NeedleRoller;
                    break;

                case "齿轮":
                    shape = PolygonType.Gear;
                    break;

                case "轴承":
                    FillShape = new DrawingBrush
                    {
                        TileMode = TileMode.Tile,
                        Viewport = new Rect(0, 0, 10, 10), // 视口大小设置为10x10的正方形
                        ViewportUnits = BrushMappingMode.Absolute,
                        Drawing = new GeometryDrawing
                        {
                            Brush = Brushes.Transparent,
                            Pen = new Pen(Brushes.Black, 1),
                            Geometry = new LineGeometry(new Point(0, 10), new Point(10, 0)) // 斜线从左下角到右上角
                        }
                    };
                    shape = PolygonType.Bearing;
                    break;
            }
            double baseDisatance = 0;
            switch(baseFase)//根据定位轴段基准面进行定位//之前是根据附属图形自身的基准面来定位的
            {
                case "左端面":
                    baseDisatance = vassalData.LeftBase;
                    break;
                case "右端面":
                    baseDisatance = vassalData.RightBase - B;
                    break;
                case "中心基准面":
                    baseDisatance = vassalData.MiddleBase - B / 2;
                    break;
            }
            //创建多边形
            Polygon upPolygon = new Polygon()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = FillShape,
                Tag = Tuple.Create(tag, shape)
            };
            Polygon downPolygon = new Polygon()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = FillShape,
                Tag = Tuple.Create(tag, shape)
            };
            Polygon onePolygon = new Polygon()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = FillShape,
                Tag = Tuple.Create(tag, shape)
            };
            if (vassalData.IsPeccentricshaft == true && vassalData.CurrentPolygon != null)//根据偏心量来绘制附属多边形（偏心改变y轴坐标）
            {
                //一个图形即无内径为0
                if (shape == PolygonType.Gear)
                {
                    onePolygon.Points = new PointCollection()
                    {
                        new Point(baseDisatance,     startY - D/2 - vassalData.EccentricOffset),  // 左上
                        new Point(baseDisatance + B, startY - D/2 - vassalData.EccentricOffset),  // 右上
                        new Point(baseDisatance + B, startY + D/2 - vassalData.EccentricOffset),// 右下
                        new Point(baseDisatance,     startY + D/2 - vassalData.EccentricOffset)// 左下
                    };
                }
                else //上下两个图形
                {
                    upPolygon.Points = new PointCollection()
                    {
                        new Point(baseDisatance,     startY - D/2 - vassalData.EccentricOffset),  // 左上
                        new Point(baseDisatance + B, startY - D/2 - vassalData.EccentricOffset),  // 右上
                        new Point(baseDisatance + B, startY - d/2 - vassalData.EccentricOffset),  // 右下
                        new Point(baseDisatance,     startY - d/2 - vassalData.EccentricOffset)   // 左下
                    };
                    downPolygon.Points = new PointCollection()
                    {
                        new Point(baseDisatance,     startY + d/2 - vassalData.EccentricOffset),// 左上
                        new Point(baseDisatance + B, startY + d/2 - vassalData.EccentricOffset),// 右上
                        new Point(baseDisatance + B, startY + D/2 - vassalData.EccentricOffset),// 右下
                        new Point(baseDisatance,     startY + D/2 - vassalData.EccentricOffset) // 左下
                    };
                }

            }
            else //绘制一般（正心）多边形（可能是附属，也可能是轴段吧）
            {
                if (shape == PolygonType.Gear)
                {
                    upPolygon.Points = new PointCollection()
                    {
                        new Point(baseDisatance,     startY - D/2),  // 左上
                        new Point(baseDisatance + B, startY - D/2),  // 右上
                        new Point(baseDisatance + B, startY + D/2),  // 右下
                        new Point(baseDisatance,     startY + D/2)   // 左下
                    };
                }
                else
                {
                    upPolygon.Points = new PointCollection()
                    {
                        new Point(baseDisatance,     startY - D/2),  // 左上
                        new Point(baseDisatance + B, startY - D/2),  // 右上
                        new Point(baseDisatance + B, startY - d/2),  // 右下
                        new Point(baseDisatance,     startY - d/2)   // 左下
                    };
                    downPolygon.Points = new PointCollection()
                    {
                        new Point(baseDisatance,     startY + d/2),
                        new Point(baseDisatance + B, startY + d/2),
                        new Point(baseDisatance + B, startY + D/2),
                        new Point(baseDisatance,     startY + D/2)
                    };
                }

            }
            //将多边形添加到画布中
            Pcanvas.Children.Add(upPolygon);
            Pcanvas.Children.Add(downPolygon);
            Pcanvas.Children.Add(onePolygon);
        }    
        //获取附属图形的曲轴位置信息
        private VassalData GetVassalGraphics(double distance)
        {
            VassalData data = new VassalData();
            Polygon lastAxisPolygon = null;
            bool found = false;
            //遍历当前已经绘制的Polygon，找到该齿轮所属的轴段
            foreach (var polygon in Pcanvas.Children.OfType<Polygon>())
            {
                // 仅处理Tag为Axis类型的Polygon
                if (polygon.Tag is Tuple<int, PolygonType> tagPolygonFound && tagPolygonFound.Item2 == PolygonType.Axis)
                {
                    lastAxisPolygon = polygon; // 记录最后一个Polygon                                             
                    double leftBoundary = polygon.Points.Min(p => p.X);
                    double rightBoundary = polygon.Points.Max(p => p.X);
                    bool centerInPolygon = distance + startX >= leftBoundary && distance + startX <= rightBoundary;
                    if (centerInPolygon)
                    {
                        data.Tag = tagPolygonFound.Item1; //曲轴的序号
                        data.IsPeccentricshaft = axisDimensions[data.Tag].IsPeccentricshaft;
                        data.EccentricDiameter = axisDimensions[data.Tag].EccentricDiameter;
                        data.Phase = axisDimensions[data.Tag].Phase;
                        data.CurrentPolygon = polygon;
                        data.LeftBase = leftBoundary;
                        data.RightBase = rightBoundary;
                        data.MiddleBase = (leftBoundary + rightBoundary) / 2;
                        if (data.IsPeccentricshaft == true)
                        {
                            double radians = data.Phase * Math.PI / 180;
                            data.EccentricOffset = data.EccentricDiameter * Math.Cos(radians); //偏心量
                        }

                        found = true;
                        break;//找到第一个匹配的多边形就退出循环  //return data?;
                    }
                }                
            }
            // 如果没有找到匹配的Polygon，且存在最后一个Polygon
            if (!found && lastAxisPolygon != null && lastAxisPolygon.Tag is Tuple<int, PolygonType> tagPolygon)
            {
                double leftBoundary = lastAxisPolygon.Points.Min(p => p.X);
                double rightBoundary = lastAxisPolygon.Points.Max(p => p.X);

                data.Tag = tagPolygon.Item1;
                data.IsPeccentricshaft = axisDimensions[data.Tag].IsPeccentricshaft;
                data.EccentricDiameter = axisDimensions[data.Tag].EccentricDiameter;
                data.Phase = axisDimensions[data.Tag].Phase;
                data.CurrentPolygon = lastAxisPolygon;
                data.LeftBase = leftBoundary;
                data.RightBase = rightBoundary;
                data.MiddleBase = (leftBoundary + rightBoundary) / 2;

                if (data.IsPeccentricshaft == true)
                {
                    double radians = data.Phase * Math.PI / 180;
                    data.EccentricOffset = data.EccentricDiameter * Math.Cos(radians);
                }
            }
            return data;
        }

        /// <summary>
        /// 锥形外径/锥形孔径的控制事件
        /// </summary>
        private void Pinterdiametercheckbox_Checked(object sender, RoutedEventArgs e)
        {
            PdirectioninnerL.IsEnabled = true;
            PdirectioninnerR.IsEnabled = true;
            Pmininterdiametertextbox.IsEnabled = true;
        }
        private void Pinterdiametercheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            PdirectioninnerL.IsEnabled = false;
            PdirectioninnerR.IsEnabled = false;
            Pmininterdiametertextbox.IsEnabled = false;
        }
        private void Pouterdiametercheckbox_Checked(object sender, RoutedEventArgs e)
        {
            PdirectionouterL.IsEnabled = true;
            PdirectionouterR.IsEnabled = true;
            Pminouterdiametertextbox.IsEnabled = true;
        }
        private void Pouterdiametercheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            PdirectionouterL.IsEnabled = false;
            PdirectionouterR.IsEnabled = false;
            Pminouterdiametertextbox.IsEnabled = false;
        }
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
