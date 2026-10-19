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
    /// UIMainBearing.xaml 的交互逻辑
    /// </summary>
    public partial class UIMainBearing : Page
    {
        private Canvas mCanvas;
        private Border MainBearingBorder;
        private DrawBearing mDrawBearing = new DrawBearing();

        // 添加缩放相关字段
        private ScaleTransform canvasScaleTransform = new ScaleTransform(1.0, 1.0);
        private TranslateTransform canvasTranslateTransform = new TranslateTransform();
        private TransformGroup canvasTransformGroup = new TransformGroup();
        private const double MinScale = 0.1;
        private const double MaxScale = 10.0;

        public UIMainBearing()
        {
            InitializeComponent();
            InitializeCanvasAndBorder();
        }

        private void InitializeCanvasAndBorder()
        {
            // 先初始化变换组
            canvasTransformGroup.Children.Add(canvasScaleTransform);
            canvasTransformGroup.Children.Add(canvasTranslateTransform);

            // 创建 Canvas
            mCanvas = new Canvas
            {
                Name = "canvas",
                Background = Brushes.White,
                RenderTransformOrigin = new Point(0.5, 0.5), // 以中心为缩放原点
                RenderTransform = canvasTransformGroup,
                // 关键：设置 HorizontalAlignment 和 VerticalAlignment 让 Canvas 填充
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            // 绑定事件
            mCanvas.MouseWheel += MainBearingCanvas_MouseWheel;
            mCanvas.MouseMove += MainBearingCanvas_MouseMove;

            // 创建 Border
            MainBearingBorder = new Border
            {
                Name = "MainBearingBorder",
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(5),
                Margin = new Thickness(4),
                ClipToBounds = true,
                Child = mCanvas
            };

            Grid.SetColumn(MainBearingBorder, 0);
            Grid.SetRow(MainBearingBorder, 0);
            MainGrid.Children.Add(MainBearingBorder);
            //在 Border 上也监听 MouseWheel 事件，因为 Canvas 可能不会填满
            MainBearingBorder.MouseWheel += MainBearingCanvas_MouseWheel;
        }

        private void MainBearingCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scaleFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;
            double newScale = canvasScaleTransform.ScaleX * scaleFactor;

            if (newScale < MinScale || newScale > MaxScale)
                return;

            canvasScaleTransform.ScaleX = newScale;
            canvasScaleTransform.ScaleY = newScale;

            e.Handled = true; // 标记事件已处理，防止冒泡
        }

        private void MainBearingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // 实现鼠标移动逻辑
        }

        public void DrawMainBearingPage(double[] doubles, double point)
        {
            DrawMainBearing(doubles, point);
        }

        private void DrawMainBearing(double[] doubles, double point)
        {
            mCanvas.Children.Clear();
            string tmpBearingType = "圆锥滚子轴承";
            if (doubles[20] == 1004)
            {
                tmpBearingType = "角接触球轴承";
            }

            object[] data1 = new object[]
            {tmpBearingType, doubles[2], doubles[3], doubles[8], doubles[9], doubles[10], doubles[11], doubles[12], doubles[4]};
            object[] data2 = new object[]
            {tmpBearingType, doubles[5], doubles[6], doubles[8], doubles[9], doubles[10], doubles[11], doubles[12], doubles[7]};

            //string tmpBearingType = "角接触球轴承";

            //object[] data1 = new object[]
            //{tmpBearingType, -1, 50, 50, 100, 50, 0.6, 0, 0};
            //object[] data2 = new object[]
            //{tmpBearingType, 1, 80, 50, 100, 50, 0.5, 0, 0};
            if (mCanvas.ActualWidth == 0)
            {
                RoutedEventHandler handler = null;
                handler = (s, e) =>
                {
                    mDrawBearing.DefineFromArray(data1);
                    mDrawBearing.BasicDrawing(mCanvas);
                    mDrawBearing.DefineFromArray(data2);
                    mDrawBearing.BasicDrawing(mCanvas);

                    DrawPoint(point);
                    DrawCenterLine();
                    mCanvas.Loaded -= handler;
                };
                mCanvas.Loaded += handler;
            }
            else
            {
                mDrawBearing.DefineFromArray(data1);
                mDrawBearing.BasicDrawing(mCanvas);
                mDrawBearing.DefineFromArray(data2);
                mDrawBearing.BasicDrawing(mCanvas);
                DrawPoint(point);
                DrawCenterLine();
            }
        }

        private void DrawPoint(double mPointLocation)
        {
            double mBaseOffsetY = mCanvas.ActualHeight / 2;
            double mBaseOffsetX = mPointLocation + mCanvas.ActualWidth / 2;

            Ellipse pointMarker = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(pointMarker, mBaseOffsetX - pointMarker.Width / 2);
            Canvas.SetTop(pointMarker, mBaseOffsetY - pointMarker.Height / 2);

            mCanvas.Children.Add(pointMarker);
        }

        private void DrawCenterLine()
        {
            Canvas inCanvas = new Canvas();
            GraphicAnnotation.DrawLine(inCanvas, mCanvas);
            mCanvas.Children.Add(inCanvas);
        }
    }
}