using Google.Protobuf.WellKnownTypes;
using HandyControl.Controls;
using HandyControl.Expression.Media;
using LibVLCSharp.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using RVReducerDesignApp.TANUI;
using RWPF;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.Schema;
using WPFBearings;
using ZstdSharp.Unsafe;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using static RVReducerDesignApp.CTANGirderPage;
using static RVReducerDesignApp.Shaft;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Taskbar;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Page = System.Windows.Controls.Page;
using Panel = System.Windows.Controls.Panel;
using Path = System.Windows.Shapes.Path;

namespace RVReducerDesignApp
{
    public enum PolygonType
    {
        Axis, //轴段       
        Bearing, //轴承
        Gear, //齿轮
        None, //空类型
        CycloidWheel,
    }
    public class DrawBase
    {
        public double BaseInnerDiameter { get; set; }
        public double BaseOuterDiameter { get; set; }
        public double BaseWidth { get; set; }
        public double BaseOffsetX { get; set; }
        public int BaseFace { get; set; }
        public int BaseIndex { get; set; }
        public double BaseOffsetY { get; set; }
        public PolygonType cPolygonType { get; set; }//类型
        public Brush FillShape { get; set; }//填充
        public DrawBase()
        { }
        public virtual void DefineFromArray(object[] data)//内径；外径；宽度；偏移画布左端X坐标；基准面
        {
            BaseInnerDiameter = Convert.ToDouble(data[0]);
            BaseOuterDiameter = Convert.ToDouble(data[1]);
            BaseWidth = Convert.ToDouble(data[2]);
            BaseOffsetX = Convert.ToDouble(data[3]);
            BaseFace = Convert.ToInt32(data[4]);
        }
        public virtual void BasicDrawing(Canvas container)//基本绘图函数
        {
            Polygon polygon = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(-BaseWidth/2, -BaseOuterDiameter/2),  // 左上角
                    new Point(BaseWidth/2, -BaseOuterDiameter/2),  // 右上角
                    new Point(BaseWidth/2, BaseOuterDiameter/2),   // 右下角
                    new Point(-BaseWidth/2, BaseOuterDiameter/2)    // 左下角
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Tag = new Shaft.ComponentKey(cPolygonType, BaseIndex)
            };
            Panel.SetZIndex(polygon, 1);
            container.Children.Add(polygon);
        }
        public double GetComponentsLocateMidpoint(int baseFace)
        {
            if (BaseFace == -1)
            {
                return BaseOffsetX - BaseWidth / 2;
            }
            else if (BaseFace == 1)
            {
                return BaseOffsetX + BaseWidth / 2;
            }
            else if (BaseFace == 0)
            {
                return BaseOffsetX;
            }
            else
            {
                return 0;
            }
        }
        public void GetComponentsLocate(Canvas container)
        {
            // 根据基准面调整容器位置

            switch (BaseFace)
            {
                case -1:
                    BaseOffsetX -= BaseWidth / 2;
                    break;
                case 1:
                    BaseOffsetX += BaseWidth / 2;
                    break;
                case 0:
                    // 不需要调整
                    break;
            }
            Canvas.SetLeft(container, BaseOffsetX); //containerOffsetX为容器中心x的坐标
            Canvas.SetTop(container, BaseOffsetY); //容器中心（X,Y）相对于画布左上角（0，0）的坐标

            //高亮背景偏移设置
            container.Width = BaseWidth;
            container.Height = BaseOuterDiameter;
            if (cPolygonType == PolygonType.Bearing)
            {
                container.MinHeight = BaseOffsetY - BaseOuterDiameter / 2;  //容器的最小Y值，用于判断摆线轮是否包含该滚针
                container.MinWidth = BaseOffsetX - BaseWidth / 2;      //容器的最小X值
            }
            Rectangle backgroundRect = new Rectangle
            {
                Width = container.Width,
                Height = container.Height,
                Fill = Brushes.Transparent,
                Name = "HighlightLayer",
                IsHitTestVisible = false
            };
            backgroundRect.RenderTransform = new TranslateTransform(-BaseWidth / 2, -BaseOuterDiameter / 2);
            container.Children.Insert(0, backgroundRect);
        }
    }
    /// <summary>
    /// 齿轮绘制
    /// </summary>
    public class DrawGear : DrawBase
    {
        public override void DefineFromArray(object[] data)//齿高；齿宽；偏移画布左端X坐标；偏移画布中心Y坐标；基准面
        {
            BaseOuterDiameter = Convert.ToDouble(data[0]);
            BaseWidth = Convert.ToDouble(data[1]);
            BaseOffsetX = Convert.ToDouble(data[2]);
            BaseOffsetY = Convert.ToDouble(data[3]);
            BaseFace = Convert.ToInt32(data[4]);
        }
        public override void BasicDrawing(Canvas canvas)
        {
            cPolygonType = PolygonType.Gear;

            double componentsLocateMidpoint = GetComponentsLocateMidpoint(BaseFace);
            Canvas container = new Canvas();
            container.Tag = new Shaft.ComponentKey(cPolygonType, BaseIndex);

            double tmpBaseOffsetY = BaseOffsetY;
            double tmpBaseOffsetX = BaseOffsetX;
            BaseOffsetY = canvas.ActualHeight / 2 - tmpBaseOffsetY;
            BaseOffsetX = BaseOffsetX + canvas.ActualWidth / 2;
            GetComponentsLocate(container); //齿轮图元的位置必须得在BaseOffsetY = canvas.ActualHeight / 2 - tmpBaseOffsetY;后面
            base.BasicDrawing(container);
            BaseOffsetY = tmpBaseOffsetY;//还原全局变量的值
            BaseOffsetX = tmpBaseOffsetX;
            canvas.Children.Insert(0, container);
        }
    }
    /// <summary>
    /// 摆线轮绘制类 
    /// </summary>
    public class DrawCycloidWheel : DrawBase
    {
        // 摆线轮特有参数
        public double RootRadius { get; set; }      // 齿根圆半径 rf
        public double TipRadius { get; set; }       // 齿顶圆半径 ra
        public double HoleRadius { get; set; }      // 孔半径 r1
        public double YDirectionOffset { get; set; } // 新增：Y方向偏移量

        private const double OUTLINE_THICKNESS = 0.2;   // 轮廓线粗细
        private const double HATCHING_THICKNESS = 0.1;  // 剖面线粗细
        private const double CENTERLINE_THICKNESS = 0.1; // 中心线粗细

        private List<CenterLineInfo> centerLinesToDraw = new List<CenterLineInfo>();

        // 内部类：存储中心线信息
        private class CenterLineInfo
        {
            public double CenterX { get; set; }
            public double CenterY { get; set; }
            public double Radius { get; set; }
            public string HoleName { get; set; }
        }

        public override void DefineFromArray(object[] data)
        {
            // 参数顺序：齿顶圆直径；齿宽；偏移画布左端X坐标；偏移画布中心Y坐标；基准面
            BaseOuterDiameter = Convert.ToDouble(data[0]);  // 齿顶圆直径
            BaseWidth = Convert.ToDouble(data[1]);          // 齿宽 B
            BaseOffsetX = Convert.ToDouble(data[2]);        // X偏移
            YDirectionOffset = Convert.ToDouble(data[3]);        // Y偏移
            BaseFace = Convert.ToInt32(data[4]);            // 基准面

            // 计算齿顶圆半径
            TipRadius = BaseOuterDiameter / 2;              // ra = 直径/2
            RootRadius = Convert.ToDouble(data[5]) / 2;          // Convert.ToDouble(data[5])为齿根圆直径 
            HoleRadius = 0;                                  // 默认自动计算

            BaseInnerDiameter = Convert.ToDouble(data[6]);  //为内孔直径，取滚针轴承的外径
        }

        public override void BasicDrawing(Canvas canvas)
        {
            cPolygonType = PolygonType.CycloidWheel;
            centerLinesToDraw.Clear();

            double componentsLocateMidpoint = GetComponentsLocateMidpoint(BaseFace);
            Canvas container = new Canvas();
            container.Tag = new Shaft.ComponentKey(cPolygonType, BaseIndex);

            double tmpBaseOffsetX = BaseOffsetX;

            // 计算绘制中心点
            double drawCenterX = 0;  // 容器内的相对中心X
            double drawCenterY = 0;  // 容器内的相对中心Y

            BaseOffsetY = canvas.ActualHeight / 2 - YDirectionOffset;
            BaseOffsetX = BaseOffsetX + canvas.ActualWidth / 2;

            GetComponentsLocate(container);

            // 在容器内绘制摆线轮剖面
            DrawCycloidWheelProfile(container, drawCenterX, drawCenterY);

            //// 处理滚针轴承的背景线
            //EnsureBackgroundLineAtY(canvas, container, componentsLocateMidpoint + canvas.ActualWidth / 2, YDirectionOffset);

            BaseOffsetY = YDirectionOffset;
            BaseOffsetX = tmpBaseOffsetX;
            canvas.Children.Insert(0, container);
        }
        /// <summary>
        /// 更新被摆线轮覆盖的轴段的Y方向偏移
        /// </summary>
        private void UpdateCoveredAxisSections(Canvas canvas, double componentsLocateMidpoint)
        {
            double cycloidLeft = componentsLocateMidpoint - BaseWidth / 2;
            double cycloidRight = componentsLocateMidpoint + BaseWidth / 2;

            // 遍历所有轴段多边形，找到被摆线轮覆盖的部分
            foreach (UIElement child in canvas.Children)
            {
                if (child is Polygon polygon && polygon.Tag is AxisTagWithParam axisTag)
                {
                    double axisLeft = polygon.Points.Min(p => p.X) - canvas.ActualWidth / 2;
                    double axisRight = polygon.Points.Max(p => p.X) - canvas.ActualWidth / 2;

                    // 检查轴段是否与摆线轮有重叠
                    if (axisRight > cycloidLeft && axisLeft < cycloidRight)
                    {
                        // 更新轴段的偏心参数以匹配摆线轮的Y偏移
                        // 注意：这里我们通过修改Tag来标记需要偏移的轴段
                        if (Math.Abs(YDirectionOffset) > 0.001)  // 有Y偏移时才更新
                        {
                            axisTag.IsPeccentricshaft = true;
                            axisTag.EccentricDiameter = YDirectionOffset;
                            axisTag.Phase = 90;  // 90度表示纯Y方向偏移
                        }
                    }
                }
            }
        }
        //绘制摆线轮剖面
        private void DrawCycloidWheelProfile(Canvas container, double centerX, double centerY)
        {
            if (BaseInnerDiameter > 0)
            {
                DrawWithInnerHole(container, centerX, centerY);
            }
            else
            {
                DrawWithoutInnerHole(container, centerX, centerY);
            }

            // 绘制所有中心线
            DrawAllCenterLines(container);
        }
        //绘制有内孔的摆线轮剖面
        private void DrawWithInnerHole(Canvas container, double centerX, double centerY)
        {
            double actual_r1 = CalculateHoleRadiusWithGap();
            double holeDiameter = 2 * actual_r1;
            double innerHoleDiameter = BaseInnerDiameter;
            double toothHeight = TipRadius - RootRadius;
            double availableHeight = TipRadius - BaseInnerDiameter / 2 - toothHeight - holeDiameter;
            double gap = availableHeight / 2;

            double startY = centerY - TipRadius;
            double viewWidth = BaseWidth;
            double currentY = startY;

            // 按顺序分段绘制
            // 齿顶段
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, toothHeight, false);
            currentY += toothHeight;

            // 上方间隙（带剖面线）
            DrawGapSegment(container, centerX - viewWidth / 2, currentY, viewWidth, gap);
            currentY += gap;

            // 上方孔
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, holeDiameter, false);
            centerLinesToDraw.Add(new CenterLineInfo
            {
                CenterX = centerX,
                CenterY = currentY + holeDiameter / 2,
                Radius = actual_r1,
                HoleName = "上方孔"
            });
            currentY += holeDiameter;

            // 上孔到内孔间隙
            DrawGapSegment(container, centerX - viewWidth / 2, currentY, viewWidth, gap);
            currentY += gap;

            // 内孔
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, innerHoleDiameter, false);
            centerLinesToDraw.Add(new CenterLineInfo
            {
                CenterX = centerX,
                CenterY = currentY + innerHoleDiameter / 2,
                Radius = innerHoleDiameter / 2,
                HoleName = "内孔"
            });
            currentY += innerHoleDiameter;

            // 内孔到下孔间隙
            DrawGapSegment(container, centerX - viewWidth / 2, currentY, viewWidth, gap);
            currentY += gap;

            // 下方孔
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, holeDiameter, false);
            centerLinesToDraw.Add(new CenterLineInfo
            {
                CenterX = centerX,
                CenterY = currentY + holeDiameter / 2,
                Radius = actual_r1,
                HoleName = "下方孔"
            });
            currentY += holeDiameter;

            // 下孔到齿间隙
            DrawGapSegment(container, centerX - viewWidth / 2, currentY, viewWidth, gap);
            currentY += gap;

            // 齿根段
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, toothHeight, false);
        }
        //绘制无内孔的摆线轮剖面
        private void DrawWithoutInnerHole(Canvas container, double centerX, double centerY)
        {
            double actual_r1 = CalculateHoleRadiusNoInner();
            double holeDiameter = 2 * actual_r1;
            double toothHeight = TipRadius - RootRadius;
            double totalHeight = 2 * TipRadius;
            double remainingSpace = totalHeight - 2 * toothHeight - 2 * holeDiameter;
            double gapHeight = remainingSpace / 3;

            double startY = centerY - TipRadius;
            double viewWidth = BaseWidth;
            double currentY = startY;

            // 齿顶段
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, toothHeight, false);
            currentY += toothHeight;

            // 上方间隙
            DrawGapSegment(container, centerX - viewWidth / 2, currentY, viewWidth, gapHeight);
            currentY += gapHeight;

            // 上方孔
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, holeDiameter, false);
            centerLinesToDraw.Add(new CenterLineInfo
            {
                CenterX = centerX,
                CenterY = currentY + holeDiameter / 2,
                Radius = actual_r1,
                HoleName = "上方孔"
            });
            currentY += holeDiameter;

            // 中间间隙
            DrawGapSegment(container, centerX - viewWidth / 2, currentY, viewWidth, gapHeight);
            currentY += gapHeight;

            // 下方孔
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, holeDiameter, false);
            centerLinesToDraw.Add(new CenterLineInfo
            {
                CenterX = centerX,
                CenterY = currentY + holeDiameter / 2,
                Radius = actual_r1,
                HoleName = "下方孔"
            });
            currentY += holeDiameter;

            // 下方间隙
            DrawGapSegment(container, centerX - viewWidth / 2, currentY, viewWidth, gapHeight);
            currentY += gapHeight;

            // 齿根段
            DrawRectangleSegment(container, centerX - viewWidth / 2, currentY, viewWidth, toothHeight, false);
        }
        //计算有内孔时的孔半径
        private double CalculateHoleRadiusWithGap()
        {
            if (HoleRadius > 0) return HoleRadius;

            double minGap = 10;
            double toothHeight = TipRadius - RootRadius;
            double coefficient = 0.25;
            double calculated_r1, availableHeight, gap;

            do
            {
                calculated_r1 = TipRadius * coefficient;
                double holeDiameter = 2 * calculated_r1;
                availableHeight = TipRadius - BaseInnerDiameter / 2 - toothHeight - holeDiameter;
                gap = availableHeight / 2;
                if (gap < minGap)
                    coefficient = Math.Max(0.05, coefficient - 0.05);
            } while (gap < minGap && coefficient > 0.05);

            return calculated_r1;
        }
        //计算无内孔时的孔半径
        private double CalculateHoleRadiusNoInner()
        {
            if (HoleRadius > 0) return HoleRadius;

            double minGap = 10;
            double toothHeight = TipRadius - RootRadius;
            double coefficient = 0.25;
            double calculated_r1, remainingSpace, gapHeight;

            do
            {
                calculated_r1 = TipRadius * coefficient;
                double holeDiameter = 2 * calculated_r1;
                double availableSpace = 2 * TipRadius - 2 * holeDiameter;
                remainingSpace = availableSpace - 2 * toothHeight;
                gapHeight = remainingSpace / 3;
                if (gapHeight < minGap)
                    coefficient = Math.Max(0.05, coefficient - 0.05);
            } while (gapHeight < minGap && coefficient > 0.05);

            return calculated_r1;
        }
        //绘制矩形段
        private void DrawRectangleSegment(Canvas container, double x, double y, double width, double height, bool drawHatching)
        {
            // 上边
            container.Children.Add(CreateLine(x, y, x + width, y));
            // 下边
            container.Children.Add(CreateLine(x, y + height, x + width, y + height));
            // 左边
            container.Children.Add(CreateLine(x, y, x, y + height));
            // 右边
            container.Children.Add(CreateLine(x + width, y, x + width, y + height));

            if (drawHatching)
            {
                DrawHatchingForRectangle(container, x, y, width, height);
            }
        }
        //绘制间隙段（仅左右竖线 + 剖面线）
        private void DrawGapSegment(Canvas container, double x, double y, double width, double height)
        {
            double effectiveHeight = Math.Max(height, 1);

            // 左边
            container.Children.Add(CreateLine(x, y, x, y + effectiveHeight));
            // 右边
            container.Children.Add(CreateLine(x + width, y, x + width, y + effectiveHeight));

            // 绘制剖面线
            DrawHatchingForRectangle(container, x, y, width, effectiveHeight);
        }
        //创建线条
        private Line CreateLine(double x1, double y1, double x2, double y2)
        {
            return new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Black,
                StrokeThickness = OUTLINE_THICKNESS,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
        }
        //绘制矩形区域的剖面线
        private void DrawHatchingForRectangle(Canvas container, double x, double y, double width, double height)
        {
            if (width <= 0 || height <= 0) return;

            double left = x, top = y, right = x + width, bottom = y + height;
            PathGeometry geometry = new PathGeometry();
            double spacing = 4;
            double minOffset = top - right;
            double maxOffset = bottom - left;

            for (double offset = minOffset - spacing; offset <= maxOffset + spacing; offset += spacing)
            {
                List<Point> intersections = new List<Point>();

                double xTop = top - offset;
                if (xTop >= left && xTop <= right)
                    intersections.Add(new Point(xTop, top));

                double xBottom = bottom - offset;
                if (xBottom >= left && xBottom <= right)
                    intersections.Add(new Point(xBottom, bottom));

                double yLeft = left + offset;
                if (yLeft >= top && yLeft <= bottom)
                    intersections.Add(new Point(left, yLeft));

                double yRight = right + offset;
                if (yRight >= top && yRight <= bottom)
                    intersections.Add(new Point(right, yRight));

                intersections = RemoveDuplicatePoints(intersections);
                intersections.Sort((p1, p2) => p1.X.CompareTo(p2.X));

                if (intersections.Count == 2)
                {
                    PathFigure figure = new PathFigure
                    {
                        StartPoint = intersections[0],
                        IsClosed = false
                    };
                    figure.Segments.Add(new LineSegment(intersections[1], true));
                    geometry.Figures.Add(figure);
                }
            }

            if (geometry.Figures.Count > 0)
            {
                Path path = new Path
                {
                    Data = geometry,
                    Stroke = Brushes.Black,
                    StrokeThickness = HATCHING_THICKNESS,
                    SnapsToDevicePixels = true,
                    UseLayoutRounding = true
                };
                Panel.SetZIndex(path, 0);
                container.Children.Add(path);
            }
        }
        //移除重复点
        private List<Point> RemoveDuplicatePoints(List<Point> points)
        {
            List<Point> result = new List<Point>();
            double tolerance = 0.001;
            foreach (Point point in points)
            {
                if (!result.Any(p => Math.Abs(p.X - point.X) < tolerance && Math.Abs(p.Y - point.Y) < tolerance))
                {
                    result.Add(point);
                }
            }
            return result;
        }

        //绘制所有中心线
        private void DrawAllCenterLines(Canvas container)
        {
            foreach (var centerLine in centerLinesToDraw)
            {
                DrawStandardCenterLine(container, centerLine.CenterX, centerLine.CenterY, centerLine.Radius);
            }
        }
        //绘制标准中心线（点划线）
        private void DrawStandardCenterLine(Canvas container, double centerX, double centerY, double radius)
        {
            double extensionLength = 1;
            double centerLineLength = BaseWidth + 2 * extensionLength;
            //double minCenterLineLength = 50;
            //centerLineLength = Math.Max(centerLineLength, minCenterLineLength);

            Line horizontalLine = new Line
            {
                X1 = centerX - centerLineLength / 2,
                Y1 = centerY,
                X2 = centerX + centerLineLength / 2,
                Y2 = centerY,
                Stroke = Brushes.Red,
                StrokeThickness = CENTERLINE_THICKNESS,
                StrokeDashArray = new DoubleCollection { 10, 3, 2, 3 },
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            Panel.SetZIndex(horizontalLine, 5);
            container.Children.Add(horizontalLine);
        }

        /// <summary>
        /// 确保滚针轴承的背景线
        /// </summary>
        public void EnsureBackgroundLineAtY(Canvas canvas, Canvas container, double componentsLocateMidpoint, double tmpBaseOffsetY)
        {
            bool isCovered = false;
            double top = 0;
            double bottom = 0;//滚针
            double cycloidLeft = 0;
            double cycloidRight = 0;//摆线轮
            double tmp = 0;
            double tmpOffsetY = 0;

            foreach (UIElement child in canvas.Children)
            {
                if (child is FrameworkElement fe && fe.Tag is Shaft.ComponentKey key && key.Type == PolygonType.Bearing)
                {
                    double left = fe.MinWidth;
                    double right = left + fe.Width; //这两行为滚针的数据

                    cycloidLeft = componentsLocateMidpoint - BaseWidth / 2;
                    cycloidRight = cycloidLeft + BaseWidth;

                    if (cycloidLeft <= left && cycloidRight >= right)
                    {
                        top = fe.MinHeight;
                        bottom = top + fe.Height;//这两行也为滚针的数据

                        double cycloidtop = BaseOffsetY - BaseOuterDiameter / 2;
                        double cycloidbottom = cycloidtop + BaseOuterDiameter;

                        if (cycloidtop <= top && cycloidbottom >= bottom)
                        {
                            tmp = fe.Height / 2;
                            tmpOffsetY = (double)fe.ToolTip;
                            isCovered = true;
                        }
                    }
                }
            }

            if (isCovered)
            {
                Line backgroundLine1 = new Line
                {
                    X1 = -BaseWidth / 2,
                    X2 = BaseWidth / 2,
                    Y1 = -tmp + tmpBaseOffsetY - tmpOffsetY,
                    Y2 = -tmp + tmpBaseOffsetY - tmpOffsetY,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.2,
                };
                Panel.SetZIndex(backgroundLine1, 20); // 设置为底层
                container.Children.Add(backgroundLine1);

                Line backgroundLine2 = new Line
                {
                    X1 = -BaseWidth / 2,
                    X2 = BaseWidth / 2,
                    Y1 = tmp + tmpBaseOffsetY - tmpOffsetY,
                    Y2 = tmp + tmpBaseOffsetY - tmpOffsetY,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.2,
                };
                Panel.SetZIndex(backgroundLine2, 20); // 设置为底层
                container.Children.Add(backgroundLine2);
            }
        }
    }
    /// <summary>
    /// 轴承绘制
    /// </summary>
    public class DrawBearing : DrawBase
    {
        public string BearingType { get; set; }

        //public int ParentAxisIndex { get; set; }  // 明确记录依附在哪个轴段上
        public bool BearingIsTaper { get; set; }
        public string BearingOrientation { get; set; }
        public double YDirectionOffset { get; set; }  // 新增：Y方向偏移量

        private double Per = 0.31;//绘制深沟球轴承所需参数
        public double TaperedRollerT { get; set; }
        public double TaperedRollerB { get; set; }
        public double TaperedRollerC { get; set; }
        public int TaperedRollerForceDirection { get; set; }
        public double ContactAngle { get; set; } // 角接触球轴承接触角
        public override void DefineFromArray(object[] data)
        {
            BearingType = data[0].ToString();
            BaseFace = Convert.ToInt32(data[1]);
            BaseOffsetX = Convert.ToDouble(data[2]);
            BaseInnerDiameter = Convert.ToDouble(data[3]);
            BaseOuterDiameter = Convert.ToDouble(data[4]);
            BaseWidth = Convert.ToDouble(data[5]);

            if (BearingType == "圆锥滚子轴承")
            {
                TaperedRollerT = Convert.ToDouble(data[5]);
                TaperedRollerB = Convert.ToDouble(data[6]);
                TaperedRollerC = Convert.ToDouble(data[7]);
                TaperedRollerForceDirection = Convert.ToInt32(data[8]);
            }
            // 新增：如果有第9个参数（对于滚针轴承），则为Y方向偏移
            if (BearingType == "滚针轴承" && data.Length > 6)
            {
                YDirectionOffset = Convert.ToDouble(data[6]);
            }
            if (BearingType == "角接触球轴承")
            {
                ContactAngle = Convert.ToDouble(data[6]);
            }
        }
        public override void BasicDrawing(Canvas canvas)
        {
            cPolygonType = PolygonType.Bearing;

            double componentsLocateMidpoint = GetComponentsLocateMidpoint(BaseFace);
            // 修改：滚针轴承使用指定的Y偏移，其他轴承使用轴段偏移
            if (BearingType == "滚针轴承" && Math.Abs(YDirectionOffset) > 0.001)
            {
                BaseOffsetY = canvas.ActualHeight / 2 - YDirectionOffset;
            }
            else
            {
                BaseOffsetY = canvas.ActualHeight / 2 - CanvasHelper.GetAxisOffsetY(canvas, componentsLocateMidpoint);
            }
            //ParentAxisIndex = CanvasHelper.GetAxisIndex(canvas, componentsLocateMidpoint);

            switch (BearingType)
            {
                case "深沟球轴承":
                    DrawDeepBall(canvas);
                    break;
                case "圆锥滚子轴承":
                    DrawTaperedRoller(canvas);
                    break;
                case "滚针轴承":
                    DrawNeedleBearing(canvas);
                    break;
                case "角接触球轴承":
                    DrawAngularContactBall(canvas);
                    break;
                    // 其他轴承类型的处理
            }
        }

#region//下面为角接触球轴承绘制
        public void DrawAngularContactBall(Canvas canvas)
        {
            Shaft.ComponentKey tag = new Shaft.ComponentKey(cPolygonType, BaseIndex);
            // 创建容器Canvas，作为轴承图元
            Canvas container = new Canvas();
            container.Tag = tag;
            double tmpBaseOffsetX = BaseOffsetX;
            BaseOffsetX = BaseOffsetX + canvas.ActualWidth / 2;
            GetComponentsLocate(container);

            // 在容器内绘制所有部件
            DrawDiameterJ(container, tag);
            DrawBallJ(container, tag);
            DrawlinesJ(container, tag);

            // 将容器添加到主画布
            BaseOffsetX = tmpBaseOffsetX;
            canvas.Children.Add(container);
        }
        private void DrawDiameterJ(Canvas container, Shaft.ComponentKey tag)
        {
            double B = Per * (BaseOuterDiameter - BaseInnerDiameter) / 2;

            // 接触角偏移计算
            double rad = ContactAngle;//弧度
            double tanValue = Math.Tan(rad);
            double effectiveWidth = BaseWidth / 2;
            double i = effectiveWidth * tanValue;
            i = Math.Min(i, B);

            // 防止超出外圈边界
            double outerTopY = -BaseOuterDiameter / 2;
            double targetTopY = -BaseOuterDiameter / 2 + B - i;
            if (targetTopY < outerTopY)
                i = B;

            // 外圈内侧倒角（带角接触斜边）
            Polygon polygon = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(-BaseWidth/2, -BaseOuterDiameter/2 + B),
                    new Point(0,           -BaseOuterDiameter/2 + B),
                    new Point(BaseWidth/2, -BaseOuterDiameter/2 + B - i),
                    new Point(BaseWidth/2, BaseOuterDiameter/2 - B + i),
                    new Point(0,           BaseOuterDiameter/2 - B),
                    new Point(-BaseWidth/2, BaseOuterDiameter/2 - B)
                },
                Stroke = Brushes.Black,
                Fill = Brushes.White,
                StrokeThickness = 0.2,
                Tag = tag
            };
            Panel.SetZIndex(polygon, 3);
            container.Children.Add(polygon);

            // 外圈外轮廓
            Polygon polygon1 = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(-BaseWidth/2, -BaseOuterDiameter/2),
                    new Point(BaseWidth/2, -BaseOuterDiameter/2),
                    new Point(BaseWidth/2, BaseOuterDiameter/2),
                    new Point(-BaseWidth/2, BaseOuterDiameter/2)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Tag = tag
            };
            Panel.SetZIndex(polygon1, 3);
            container.Children.Add(polygon1);

            // 内圈外侧倒角
            Polygon polygon2 = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(-BaseWidth/2, -BaseInnerDiameter/2 - B),
                    new Point(BaseWidth/2, -BaseInnerDiameter/2 - B),
                    new Point(BaseWidth/2, BaseInnerDiameter/2 + B),
                    new Point(-BaseWidth/2, BaseInnerDiameter/2 + B)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Tag = tag
            };
            Panel.SetZIndex(polygon2, 4);
            container.Children.Add(polygon2);

            // 内圈内轮廓
            Polygon polygon3 = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(-BaseWidth/2, -BaseInnerDiameter/2),
                    new Point(BaseWidth/2, -BaseInnerDiameter/2),
                    new Point(BaseWidth/2, BaseInnerDiameter/2),
                    new Point(-BaseWidth/2, BaseInnerDiameter/2)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Tag = tag
            };
            Panel.SetZIndex(polygon3, 5);
            container.Children.Add(polygon3);
        }
        private void DrawBallJ(Canvas container, Shaft.ComponentKey tag)
        {
            double P = 0.1 * (BaseOuterDiameter - BaseInnerDiameter);
            double N = 0.03 * (BaseOuterDiameter - BaseInnerDiameter);
            double R = (BaseOuterDiameter - BaseInnerDiameter) * (1 - 2 * Per) / 4;

            // 上滚珠
            Ellipse circle = new Ellipse
            {
                Width = 2 * R + P,
                Height = 2 * R + P,
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White,
                Tag = tag
            };
            Canvas.SetLeft(circle, -R - P / 2);
            Canvas.SetTop(circle, -R - BaseInnerDiameter / 2 - (BaseOuterDiameter - BaseInnerDiameter) / 4 - N);
            Panel.SetZIndex(circle, 99);
            container.Children.Add(circle);

            // 下滚珠
            Ellipse circle1 = new Ellipse
            {
                Width = 2 * R + P,
                Height = 2 * R + P,
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White,
                Tag = tag
            };
            Canvas.SetLeft(circle1, -R - P / 2);
            Canvas.SetTop(circle1, -R + (3 * BaseInnerDiameter - BaseOuterDiameter) / 4 + Per * (BaseOuterDiameter - BaseInnerDiameter) + P + N);
            Panel.SetZIndex(circle1, 99);
            container.Children.Add(circle1);
        }
        private void DrawlinesJ(Canvas container, Shaft.ComponentKey tag)
        {
            double X = -BaseWidth / 2;
            double length = Per * (BaseOuterDiameter - BaseInnerDiameter) / 2;
            double spacing = CalculateSpacing(BaseOuterDiameter);
            int count = (int)(BaseWidth / spacing) + 3;
            int n = 1;

            // 下半部分斜线
            double Y1 = BaseInnerDiameter / 2;
            double Z1 = BaseInnerDiameter / 2 + (BaseOuterDiameter - BaseInnerDiameter) / 4;
            DrawDiagonalLines1J(container, new Point(X, Y1), length, spacing, count, n, tag);
            DrawMirroredDiagonalLinesJ(container, new Point(X, Y1), length, spacing, count, n, Z1, tag);

            // 上半部分斜线
            double Y2 = -BaseInnerDiameter / 2 - Per * (BaseOuterDiameter - BaseInnerDiameter) / 2;
            double Z2 = -BaseInnerDiameter / 2 - (BaseOuterDiameter - BaseInnerDiameter) / 4;
            DrawDiagonalLines1J(container, new Point(X, Y2), length, spacing, count, n, tag);
            DrawMirroredDiagonalLinesJ(container, new Point(X, Y2), length, spacing, count, n, Z2, tag);
        }
        private void DrawDiagonalLines1J(Canvas container, Point startPoint, double length, double spacing, int count, int n, Shaft.ComponentKey tag)
        {
            double rightBound = BaseWidth / 2;
            double leftBound = -BaseWidth / 2;

            switch (n)
            {
                case 1:
                    for (int i = 0; i < count; i++)
                    {
                        Point currentStart = new Point(startPoint.X + i * spacing, startPoint.Y);
                        double endX = currentStart.X + length;
                        double endY = currentStart.Y + length;

                        if (endX >= rightBound)
                        {
                            double remain = rightBound - currentStart.X;
                            endX = rightBound;
                            endY = currentStart.Y + remain;
                        }

                        Line line = new Line
                        {
                            X1 = currentStart.X,
                            Y1 = currentStart.Y,
                            X2 = endX,
                            Y2 = endY,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.2,
                            Tag = tag
                        };
                        Panel.SetZIndex(line, 5);
                        container.Children.Add(line);

                        if (endX >= rightBound) break;
                    }
                    break;

                case 2:
                    for (int i = 0; i < count; i++)
                    {
                        Point currentStart = new Point(startPoint.X + i * spacing, startPoint.Y);
                        double endX = currentStart.X - length;
                        double endY = currentStart.Y + length;

                        if (endX < leftBound)
                        {
                            double remain = currentStart.X - leftBound;
                            endX = leftBound;
                            endY = currentStart.Y + remain;
                        }

                        if (currentStart.X > rightBound) continue;

                        Line line = new Line
                        {
                            X1 = endX,
                            Y1 = endY,
                            X2 = currentStart.X,
                            Y2 = currentStart.Y,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.2,
                            Tag = tag
                        };
                        Panel.SetZIndex(line, 5);
                        container.Children.Add(line);
                    }
                    break;
            }
        }
        private void DrawMirroredDiagonalLinesJ(Canvas container, Point startPoint, double length, double spacing, int count, int n, double mirrorY, Shaft.ComponentKey tag)
        {
            if (n != 1) return;

            double rightBound = BaseWidth / 2;

            for (int i = 0; i < count; i++)
            {
                Point currentStart = new Point(startPoint.X + i * spacing, startPoint.Y);
                double endX = currentStart.X + length;
                double endY = currentStart.Y + length;

                if (endX >= rightBound)
                {
                    double remain = rightBound - currentStart.X;
                    endX = rightBound;
                    endY = currentStart.Y + remain;
                }

                double mStartY = 2 * mirrorY - currentStart.Y;
                double mEndY = 2 * mirrorY - endY;

                Line line = new Line
                {
                    X1 = currentStart.X,
                    Y1 = mStartY,
                    X2 = endX,
                    Y2 = mEndY,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.2,
                    Tag = tag
                };
                Panel.SetZIndex(line, 2);
                container.Children.Add(line);

                if (endX >= rightBound) break;
            }
        }
#endregion
#region//下面为滚针轴承绘制
        private void DrawNeedleBearing(Canvas canvas)
        {
            double componentsLocateMidpoint = GetComponentsLocateMidpoint(BaseFace);

            Canvas container = new Canvas();
            container.Tag = new Shaft.ComponentKey(cPolygonType, BaseIndex);
            container.ToolTip = CanvasHelper.GetAxisOffsetY(canvas, componentsLocateMidpoint); // 用于设置摆线轮补充滚针线条的坐标调整
            double tmpBaseOffsetX = BaseOffsetX;
            BaseOffsetX = BaseOffsetX + canvas.ActualWidth / 2;
            GetComponentsLocate(container);

            base.BasicDrawing(container);
            double tmpBaseOuterDiameter = BaseOuterDiameter;
            BaseOuterDiameter = BaseInnerDiameter;
            base.BasicDrawing(container);
            BaseOuterDiameter = tmpBaseOuterDiameter;
            DrawLines(container, new Shaft.ComponentKey(cPolygonType, BaseIndex));
            BaseOffsetX = tmpBaseOffsetX;
            canvas.Children.Insert(0, container);
        }
        private void DrawLines(Canvas container, Shaft.ComponentKey tag)
        {
            double CenterLineExtension = BaseWidth / 20; // 中心线超出滚针的长度
            double tmp = (BaseOuterDiameter - BaseInnerDiameter) / 4;


            // 第一条中心线（上方）
            Line upperLine = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 0.2,
                StrokeDashArray = new DoubleCollection { 3, 2 },
                X1 = -BaseWidth / 2 - CenterLineExtension,
                Y1 = -BaseInnerDiameter / 2 - tmp,
                X2 = BaseWidth / 2 + CenterLineExtension,
                Y2 = -BaseInnerDiameter / 2 - tmp,
                Tag = tag
            };

            // 第二条中心线（下方）
            Line lowerLine = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 0.2,
                StrokeDashArray = new DoubleCollection { 3, 2 },
                X1 = -BaseWidth / 2 - CenterLineExtension,
                Y1 = BaseInnerDiameter / 2 + tmp,
                X2 = BaseWidth / 2 + CenterLineExtension,
                Y2 = BaseInnerDiameter / 2 + tmp,
                Tag = tag
            };

            // 设置渲染层级
            Panel.SetZIndex(upperLine, 2);  // 在滚针图形之上
            Panel.SetZIndex(lowerLine, 2);

            container.Children.Add(upperLine);
            container.Children.Add(lowerLine);
        }
#endregion
#region//下面为圆锥滚子轴承绘制
        private void DrawTaperedRoller(Canvas canvas)
        {
            Shaft.ComponentKey tag = new Shaft.ComponentKey(cPolygonType, BaseIndex);
            // 创建容器Canvas，作为轴承图元
            Canvas container = new Canvas();
            container.Tag = tag;
            double tmpBaseOffsetX = BaseOffsetX;
            BaseOffsetX = BaseOffsetX + canvas.ActualWidth / 2;
            //BaseWidth = TaperedRollerT;
            GetComponentsLocate(container);//圆锥滚子轴承组件定位
            YZGZ(container, tag);
            BaseOffsetX = tmpBaseOffsetX;
            // 将容器添加到主画布
            canvas.Children.Insert(0, container);
        }
        private void YZGZ(Canvas container, Shaft.ComponentKey tag)
        {
            // 原有YZGZ逻辑保留
            double T = TaperedRollerT;
            double B = TaperedRollerB;
            double C = TaperedRollerC;
            double InnerDiameter = BaseInnerDiameter;
            double OuterDiameter = BaseOuterDiameter;
            double A = (OuterDiameter - InnerDiameter) / 2;
            double E = (T - B) / 2;
            if (T < C && T > B)
                DrawDiameterYZ1(container, tag, A, E);
            else if (T > C && T < B)
                DrawDiameterYZ2(container, tag, A, E);
            else
                DrawDiameterYZ(container, tag, A, E);
        }
        private double TransX(double x)
        {
            int SetA = TaperedRollerForceDirection;
            double CenterX = 0;
            if (SetA == 1)

                return x;
            else if (SetA == -1)
                return (2 * CenterX - x);
            else
            {
                MessageBox.Show("error");
                return 0;
            }
        }
        private void DrawDiameterYZ1(Canvas container, Shaft.ComponentKey tag, double A, double E)
        {
            double T = TaperedRollerT;
            double B = TaperedRollerB;
            double C = TaperedRollerC;
            double InnerDiameter = BaseInnerDiameter;
            double OuterDiameter = BaseOuterDiameter;
            int SetA = TaperedRollerForceDirection;
            double CenterX = 0;
            double CenterY = 0;
            double Ta1, X1, X2, Y2, Y1;
            if (SetA == 1)
                Ta1 = 15;
            else if (SetA == -1)
                Ta1 = -15;
            else
            {
                MessageBox.Show("error");
                return;
            }
            Polygon polygon = new Polygon
            {
                Points = new PointCollection
            {
                 new Point(TransX(CenterX -B+T/2),CenterY -InnerDiameter/2),  // 左上角
                 new Point(TransX(CenterX+ T/2), CenterY -InnerDiameter/2),  // 右上角
                 new Point(TransX(CenterX +T/2), CenterY + InnerDiameter/2),  // 右下角
                 new Point(TransX(CenterX -B+T/2), CenterY + InnerDiameter/2)   // 左下角
            },

                //设置填充颜色和边框
                Stroke = Brushes.Black,
                StrokeThickness = 0.2
            };
            Panel.SetZIndex(polygon, 5);//内圈
            container.Children.Add(polygon);


            Polygon polygon1 = new Polygon
            {
                Points = new PointCollection
        {
            new Point(CenterX - C/5, CenterY - InnerDiameter/2 - A/2 - A/4),  // 左上角
            new Point(CenterX + C/5, CenterY - InnerDiameter/2 - A/2 - A/4),  // 右上角
            new Point(CenterX + C/5, CenterY - InnerDiameter/2 - A/2 + A/4),  // 原始右下角
            new Point(CenterX - C/5, CenterY - InnerDiameter/2 - A/2 + A/4)   // 左下角
        },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White
            };

            RotateTransform rotateTransform = new RotateTransform
            {
                Angle = -Ta1,
                CenterX = CenterX,
                CenterY = CenterY - InnerDiameter / 2 - A / 2
            };
            polygon1.RenderTransform = rotateTransform;
            Panel.SetZIndex(polygon1, 3);
            container.Children.Add(polygon1);
            if (SetA == 1)
            {
                // 计算旋转后右下角坐标
                Point originalBottomRight = polygon1.Points[2]; // 原始右下角
                Point rotateCenter = new Point(rotateTransform.CenterX, rotateTransform.CenterY);
                double angleRadians = rotateTransform.Angle * Math.PI / 180;

                // 旋转后坐标计算
                double rotatedX = (originalBottomRight.X - rotateCenter.X) * Math.Cos(angleRadians)
                                - (originalBottomRight.Y - rotateCenter.Y) * Math.Sin(angleRadians)
                                + rotateCenter.X;
                double rotatedY = (originalBottomRight.X - rotateCenter.X) * Math.Sin(angleRadians)
                                + (originalBottomRight.Y - rotateCenter.Y) * Math.Cos(angleRadians)
                                + rotateCenter.Y;

                // 核心：将旋转后右下角的Y值赋值给Y1
                Y1 = rotatedY;  // Y1即为旋转后右下角的Y坐标
                X1 = rotatedX;
            }
            else if (SetA == -1)
            {
                // 计算旋转后右下角坐标
                Point originalBottomRight = polygon1.Points[3]; // 原始右下角
                Point rotateCenter = new Point(rotateTransform.CenterX, rotateTransform.CenterY);
                double angleRadians = rotateTransform.Angle * Math.PI / 180;

                // 旋转后坐标计算
                double rotatedX = (originalBottomRight.X - rotateCenter.X) * Math.Cos(angleRadians)
                                - (originalBottomRight.Y - rotateCenter.Y) * Math.Sin(angleRadians)
                                + rotateCenter.X;
                double rotatedY = (originalBottomRight.X - rotateCenter.X) * Math.Sin(angleRadians)
                                + (originalBottomRight.Y - rotateCenter.Y) * Math.Cos(angleRadians)
                                + rotateCenter.Y;

                // 核心：将旋转后右下角的Y值赋值给Y1
                Y1 = rotatedY;  // Y1即为旋转后右下角的Y坐标
                X1 = rotatedX;
            }
            else
            {
                MessageBox.Show("error"); MessageBox.Show("error");
                return;
            }
            Polygon polygon11 = new Polygon
            {
                Points = new PointCollection
    {
        new Point(CenterX - C/5, CenterY + InnerDiameter/2 + A/2 + A/4),  // 0:左上角
        new Point(CenterX + C/5, CenterY + InnerDiameter/2 + A/2 + A/4),  // 1:右上角（目标顶点）
        new Point(CenterX + C/5, CenterY + InnerDiameter/2 + A/2 - A/4),  // 2:右下角
        new Point(CenterX - C/5, CenterY + InnerDiameter/2 + A/2 - A/4)   // 3:左下角
    },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White
            };

            // polygon11的旋转变换（顺时针15度）
            RotateTransform rotateTransform1 = new RotateTransform
            {
                Angle = Ta1,  // 自身旋转角度：15度（顺时针）
                CenterX = CenterX,  // 旋转中心X
                CenterY = CenterY + InnerDiameter / 2 + A / 2  // 旋转中心Y
            };
            polygon11.RenderTransform = rotateTransform1;
            Panel.SetZIndex(polygon11, 3);
            container.Children.Add(polygon11);
            if (SetA == 1)
            {
                Point originalTopRight11 = polygon11.Points[2];
                Point rotateCenter11 = new Point(rotateTransform1.CenterX, rotateTransform1.CenterY);
                double angleRadians11 = rotateTransform1.Angle * Math.PI / 180;  // 修正：用rotateTransform1.Angle
                                                                                 // 旋转后X = (原始X - 旋转中心X) * cos(弧度) - (原始Y - 旋转中心Y) * sin(弧度) + 旋转中心X
                                                                                 // 旋转后Y = (原始X - 旋转中心X) * sin(弧度) + (原始Y - 旋转中心Y) * cos(弧度) + 旋转中心Y
                double rotatedTopRightX =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Cos(angleRadians11) -
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Sin(angleRadians11) +
                    rotateCenter11.X;
                double rotatedTopRightY =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Sin(angleRadians11) +
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Cos(angleRadians11) +
                    rotateCenter11.Y;

                X2 = rotatedTopRightX;  // 最终需要的右上角X
                Y2 = rotatedTopRightY;
            }
            else if (SetA == -1)
            {
                Point originalTopRight11 = polygon11.Points[3];
                Point rotateCenter11 = new Point(rotateTransform1.CenterX, rotateTransform1.CenterY);
                double angleRadians11 = rotateTransform1.Angle * Math.PI / 180;  // 修正：用rotateTransform1.Angle
                                                                                 // 旋转后X = (原始X - 旋转中心X) * cos(弧度) - (原始Y - 旋转中心Y) * sin(弧度) + 旋转中心X
                                                                                 // 旋转后Y = (原始X - 旋转中心X) * sin(弧度) + (原始Y - 旋转中心Y) * cos(弧度) + 旋转中心Y
                double rotatedTopRightX =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Cos(angleRadians11) -
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Sin(angleRadians11) +
                    rotateCenter11.X;
                double rotatedTopRightY =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Sin(angleRadians11) +
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Cos(angleRadians11) +
                    rotateCenter11.Y;

                X2 = rotatedTopRightX;  // 最终需要的右上角X
                Y2 = rotatedTopRightY;
            }
            else
            {
                MessageBox.Show("error"); MessageBox.Show("error");
                return;
            }
            Polygon polygon2 = new Polygon
            {
                Points = new PointCollection
            {
                 new Point(TransX(CenterX -B+T/2) ,CenterY -InnerDiameter/2),  // 上
                 new Point(TransX(CenterX -B+T/2),CenterY -InnerDiameter/2-A/4),  // 上1
                 new Point(TransX(CenterX -B/2+T/4),CenterY -InnerDiameter/2-A/4),  // 右1
                 new Point(X1,Y1),  // 右上1(根号3）
                 new Point(X1 ,CenterY -InnerDiameter/2-A/2),  // 上1
                 new Point(TransX(CenterX-T/2+C),CenterY -InnerDiameter/2-A/2),  // 上1
                 new Point(TransX(CenterX-T/2+C),Y1-A/2/Math.Cos(15 * Math.PI / 180)),  // 上1(cos15)
                 new Point(TransX(CenterX-T/2),Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180)),   // 左下1(tan15)
                 new Point(TransX(CenterX-T/2),2*CenterY -(Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180))),   // 下1(tan15)
                 new Point(TransX(CenterX-T/2+C),Y2+A/2/Math.Cos(15 * Math.PI / 180)),  // 上1(cos15)
                 new Point(TransX(CenterX-T/2+C),CenterY +InnerDiameter/2+A/2),  // 上1
                new Point(X2,CenterY +InnerDiameter/2+A/2),  // 上1
                 new Point(X2,Y2),  // 右上1(根号3）
                 new Point(TransX(CenterX -B/2+T/4),CenterY+InnerDiameter/2+A/4),  // 右1
                 new Point(TransX(CenterX -B+T/2),CenterY +InnerDiameter/2+A/4),  // 上1
                 new Point(TransX(CenterX -B+T/2) ,CenterY +InnerDiameter/2),  // 上
                
            },

                // 设置填充颜色和边框0
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,

            };
            Panel.SetZIndex(polygon2, 1);
            container.Children.Add(polygon2);//空白斜线

            Polygon polygon3 = new Polygon
            {
                Points = new PointCollection
                {
                 new Point(TransX(CenterX-T/2+C),Y1-A/2/Math.Cos(15 * Math.PI / 180)),  // 右下角
                 new Point(TransX(CenterX-T/2+C),CenterY-OuterDiameter/2),  // 右上角
                 new Point(TransX(CenterX-T/2),CenterY-OuterDiameter/2),   // 左上角
                 new Point(TransX(CenterX-T/2),Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180))   // 左下角
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: 45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon3, 1);
            container.Children.Add(polygon3);//外侧俩3

            Polygon polygon31 = new Polygon
            {
                Points = new PointCollection
                {
                 new Point(TransX(CenterX-T/2+C),CenterY+OuterDiameter/2),  // 右下角
                 new Point(TransX(CenterX-T/2+C),Y2+A/2/Math.Cos(15 * Math.PI / 180)),  // 右上角
                 new Point(TransX(CenterX-T/2),Y2+A/2/Math.Cos(15 * Math.PI / 180)-C*Math.Tan(15 * Math.PI / 180)),   // 左上角
                 new Point(TransX(CenterX-T/2),CenterY+OuterDiameter/2)   // 左下角
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: -45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon31, 1);
            container.Children.Add(polygon31);//外侧俩


            Polygon polygon4 = new Polygon
            {
                Points = new PointCollection//下
                {
                 new Point(X2,Y2),
                 new Point(X2,CenterY+InnerDiameter/2+A/2),
                 new Point(TransX(CenterX+T/2),CenterY+InnerDiameter/2+A/2),
                 new Point(TransX(CenterX+T/2),CenterY +InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY +InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY +InnerDiameter/2+A/4),
                 new Point(TransX(CenterX -B/2+T/4),CenterY+InnerDiameter/2+A/4)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: 45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon4, 1);
            container.Children.Add(polygon4);//内侧俩

            Polygon polygon41 = new Polygon
            {
                Points = new PointCollection//下
                {
                 new Point(X1,Y1),
                 new Point(X1,CenterY-InnerDiameter/2-A/2),
                 new Point(TransX(CenterX+T/2),CenterY-InnerDiameter/2-A/2),
                 new Point(TransX(CenterX+T/2),CenterY -InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY-InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY -InnerDiameter/2-A/4),
                new Point(TransX(CenterX -B/2+T/4),CenterY-InnerDiameter/2-A/4)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: -45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon41, 1);
            container.Children.Add(polygon41);//内侧俩

        }
        private void DrawDiameterYZ(Canvas container, Shaft.ComponentKey tag, double A, double E)
        {
            double T = TaperedRollerT;
            double B = TaperedRollerB;
            double C = TaperedRollerC;
            double InnerDiameter = BaseInnerDiameter;
            double OuterDiameter = BaseOuterDiameter;
            int SetA = TaperedRollerForceDirection;
            double CenterX = 0;
            double CenterY = 0;
            double Ta1, X1, X2, Y2, Y1;
            if (SetA == 1)
                Ta1 = 15;
            else if (SetA == -1)
                Ta1 = -15;
            else
            {
                MessageBox.Show("error");
                return;
            }
            Polygon polygon = new Polygon
            {
                Points = new PointCollection
            {
                 new Point(TransX(CenterX -B+T/2),CenterY -InnerDiameter/2),  // 左上角
                 new Point(TransX(CenterX+ T/2), CenterY -InnerDiameter/2),  // 右上角
                 new Point(TransX(CenterX +T/2), CenterY + InnerDiameter/2),  // 右下角
                 new Point(TransX(CenterX -B+T/2), CenterY + InnerDiameter/2)   // 左下角
            },

                //设置填充颜色和边框
                Stroke = Brushes.Black,
                StrokeThickness = 0.2
            };
            Panel.SetZIndex(polygon, 5);//内圈
            container.Children.Add(polygon);


            Polygon polygon1 = new Polygon
            {
                Points = new PointCollection
        {
            new Point(CenterX - C/3, CenterY - InnerDiameter/2 - A/2 - A/4),  // 左上角
            new Point(CenterX + C/3, CenterY - InnerDiameter/2 - A/2 - A/4),  // 右上角
            new Point(CenterX + C/3, CenterY - InnerDiameter/2 - A/2 + A/4),  // 原始右下角
            new Point(CenterX - C/3, CenterY - InnerDiameter/2 - A/2 + A/4)   // 左下角
        },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White
            };

            RotateTransform rotateTransform = new RotateTransform
            {
                Angle = -Ta1,
                CenterX = CenterX,
                CenterY = CenterY - InnerDiameter / 2 - A / 2
            };
            polygon1.RenderTransform = rotateTransform;
            Panel.SetZIndex(polygon1, 3);
            container.Children.Add(polygon1);

            if (SetA == 1)
            {
                // 计算旋转后右下角坐标
                Point originalBottomRight = polygon1.Points[2]; // 原始右下角
                Point rotateCenter = new Point(rotateTransform.CenterX, rotateTransform.CenterY);
                double angleRadians = rotateTransform.Angle * Math.PI / 180;

                // 旋转后坐标计算
                double rotatedX = (originalBottomRight.X - rotateCenter.X) * Math.Cos(angleRadians)
                                - (originalBottomRight.Y - rotateCenter.Y) * Math.Sin(angleRadians)
                                + rotateCenter.X;
                double rotatedY = (originalBottomRight.X - rotateCenter.X) * Math.Sin(angleRadians)
                                + (originalBottomRight.Y - rotateCenter.Y) * Math.Cos(angleRadians)
                                + rotateCenter.Y;

                // 核心：将旋转后右下角的Y值赋值给Y1
                Y1 = rotatedY;  // Y1即为旋转后右下角的Y坐标
                X1 = rotatedX;
            }
            else if (SetA == -1)
            {
                // 计算旋转后右下角坐标
                Point originalBottomRight = polygon1.Points[3]; // 原始右下角
                Point rotateCenter = new Point(rotateTransform.CenterX, rotateTransform.CenterY);
                double angleRadians = rotateTransform.Angle * Math.PI / 180;

                // 旋转后坐标计算
                double rotatedX = (originalBottomRight.X - rotateCenter.X) * Math.Cos(angleRadians)
                                - (originalBottomRight.Y - rotateCenter.Y) * Math.Sin(angleRadians)
                                + rotateCenter.X;
                double rotatedY = (originalBottomRight.X - rotateCenter.X) * Math.Sin(angleRadians)
                                + (originalBottomRight.Y - rotateCenter.Y) * Math.Cos(angleRadians)
                                + rotateCenter.Y;

                // 核心：将旋转后右下角的Y值赋值给Y1
                Y1 = rotatedY;  // Y1即为旋转后右下角的Y坐标
                X1 = rotatedX;
            }
            else
            {
                MessageBox.Show("error");
                return;
            }

            Polygon polygon11 = new Polygon
            {
                Points = new PointCollection
    {
        new Point(CenterX - C/3, CenterY + InnerDiameter/2 + A/2 + A/4),  // 0:左上角
        new Point(CenterX + C/3, CenterY + InnerDiameter/2 + A/2 + A/4),  // 1:右上角（目标顶点）
        new Point(CenterX + C/3, CenterY + InnerDiameter/2 + A/2 - A/4),  // 2:右下角
        new Point(CenterX - C/3, CenterY + InnerDiameter/2 + A/2 - A/4)   // 3:左下角
    },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White
            };

            // polygon11的旋转变换（顺时针15度）
            RotateTransform rotateTransform1 = new RotateTransform
            {
                Angle = Ta1,  // 自身旋转角度：15度（顺时针）
                CenterX = CenterX,  // 旋转中心X
                CenterY = CenterY + InnerDiameter / 2 + A / 2  // 旋转中心Y
            };
            polygon11.RenderTransform = rotateTransform1;
            Panel.SetZIndex(polygon11, 3);
            container.Children.Add(polygon11);

            if (SetA == 1)
            {
                Point originalTopRight11 = polygon11.Points[2];
                Point rotateCenter11 = new Point(rotateTransform1.CenterX, rotateTransform1.CenterY);
                double angleRadians11 = rotateTransform1.Angle * Math.PI / 180;  // 修正：用rotateTransform1.Angle
                                                                                 // 旋转后X = (原始X - 旋转中心X) * cos(弧度) - (原始Y - 旋转中心Y) * sin(弧度) + 旋转中心X
                                                                                 // 旋转后Y = (原始X - 旋转中心X) * sin(弧度) + (原始Y - 旋转中心Y) * cos(弧度) + 旋转中心Y
                double rotatedTopRightX =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Cos(angleRadians11) -
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Sin(angleRadians11) +
                    rotateCenter11.X;
                double rotatedTopRightY =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Sin(angleRadians11) +
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Cos(angleRadians11) +
                    rotateCenter11.Y;

                X2 = rotatedTopRightX;  // 最终需要的右上角X
                Y2 = rotatedTopRightY;
            }
            else if (SetA == -1)
            {
                Point originalTopRight11 = polygon11.Points[3];
                Point rotateCenter11 = new Point(rotateTransform1.CenterX, rotateTransform1.CenterY);
                double angleRadians11 = rotateTransform1.Angle * Math.PI / 180;  // 修正：用rotateTransform1.Angle
                                                                                 // 旋转后X = (原始X - 旋转中心X) * cos(弧度) - (原始Y - 旋转中心Y) * sin(弧度) + 旋转中心X
                                                                                 // 旋转后Y = (原始X - 旋转中心X) * sin(弧度) + (原始Y - 旋转中心Y) * cos(弧度) + 旋转中心Y
                double rotatedTopRightX =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Cos(angleRadians11) -
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Sin(angleRadians11) +
                    rotateCenter11.X;
                double rotatedTopRightY =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Sin(angleRadians11) +
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Cos(angleRadians11) +
                    rotateCenter11.Y;

                X2 = rotatedTopRightX;  // 最终需要的右上角X
                Y2 = rotatedTopRightY;
            }
            else
            {
                MessageBox.Show("error"); MessageBox.Show("error");
                return;
            }
            Polygon polygon2 = new Polygon
            {
                Points = new PointCollection
            {
                 new Point(TransX(CenterX -B+T/2) ,CenterY -InnerDiameter/2),  // 上
                 new Point(TransX(CenterX -B+T/2),CenterY -InnerDiameter/2-A/4),  // 上1
                 new Point(TransX(CenterX -B/2+T/4),CenterY -InnerDiameter/2-A/4),  // 右1
                 new Point(X1,Y1),  // 右上1(根号3）
                 new Point(X1,CenterY -InnerDiameter/2-A/2),  // 上1
                 new Point(X1,Y1-A/2/Math.Cos(15 * Math.PI / 180)),  // 上1(cos15)
                 new Point(TransX(CenterX-T/2),Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180)),   // 左下1(tan15)
                 new Point(TransX(CenterX-T/2),2*CenterY -(Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180))),   // 下1(tan15)
                 new Point(X2,Y2+A/2/Math.Cos(15 * Math.PI / 180)),  // 上1(cos15)
                new Point(X2,CenterY +InnerDiameter/2+A/2),  // 上1
                 new Point(X2,Y2),  // 右上1(根号3）
                 new Point(TransX(CenterX -B/2+T/4),CenterY+InnerDiameter/2+A/4),  // 右1
                 new Point(TransX(CenterX -B+T/2),CenterY +InnerDiameter/2+A/4),  // 上1
                 new Point(TransX(CenterX -B+T/2),CenterY +InnerDiameter/2),  // 上
                
            },

                // 设置填充颜色和边框0
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,

            };
            Panel.SetZIndex(polygon2, 1);
            container.Children.Add(polygon2);//空白斜线

            Polygon polygon3 = new Polygon
            {
                Points = new PointCollection
                {
                 new Point(X1,Y1-A/2/Math.Cos(15 * Math.PI / 180)),  // 右下角
                 new Point(X1,CenterY-OuterDiameter/2),  // 右上角
                 new Point(TransX(CenterX-T/2),CenterY-OuterDiameter/2),   // 左上角
                 new Point(TransX(CenterX-T/2),Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180))   // 左下角
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: 45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon3, 1);
            container.Children.Add(polygon3);//外侧俩3

            Polygon polygon31 = new Polygon
            {
                Points = new PointCollection
                {
                 new Point(X2,CenterY+OuterDiameter/2),  // 右下角
                 new Point(X2,Y2+A/2/Math.Cos(15 * Math.PI / 180)),  // 右上角
                 new Point(TransX(CenterX-T/2),Y2+A/2/Math.Cos(15 * Math.PI / 180)-C*Math.Tan(15 * Math.PI / 180)),   // 左上角
                 new Point(TransX(CenterX-T/2),CenterY+OuterDiameter/2)   // 左下角
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: -45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon31, 1);
            container.Children.Add(polygon31);//外侧俩


            Polygon polygon4 = new Polygon
            {
                Points = new PointCollection//下
                {
                 new Point(X2,Y2),
                 new Point(X2,CenterY+InnerDiameter/2+A/2),
                 new Point(TransX(CenterX+T/2),CenterY+InnerDiameter/2+A/2),
                 new Point(TransX(CenterX+T/2),CenterY +InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY +InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY +InnerDiameter/2+A/4),
                 new Point(TransX(CenterX-B/2+T/4),CenterY+InnerDiameter/2+A/4)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: 45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon4, 1);
            container.Children.Add(polygon4);//内侧俩

            Polygon polygon41 = new Polygon
            {
                Points = new PointCollection//下
                {
                 new Point(X1,Y1),
                 new Point(X1,CenterY-InnerDiameter/2-A/2),
                 new Point(TransX(CenterX+T/2),CenterY-InnerDiameter/2-A/2),
                 new Point(TransX(CenterX+T/2),CenterY -InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY-InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY -InnerDiameter/2-A/4),
                 new Point(TransX(CenterX-B/2+T/4),CenterY-InnerDiameter/2-A/4)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: -45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon41, 1);
            container.Children.Add(polygon41);//内侧俩

        }
        private void DrawDiameterYZ2(Canvas container, Shaft.ComponentKey tag, double A, double E)
        {
            double T = TaperedRollerT;
            double B = TaperedRollerB;
            double C = TaperedRollerC;
            double InnerDiameter = BaseInnerDiameter;
            double OuterDiameter = BaseOuterDiameter;
            int SetA = TaperedRollerForceDirection;
            double CenterX = 0;
            double CenterY = 0;
            double Ta1, X1, X2, Y2, Y1;
            if (SetA == 1)
                Ta1 = 15;
            else if (SetA == -1)
                Ta1 = -15;
            else
            {
                MessageBox.Show("error");
                return;
            }
            Polygon polygon = new Polygon
            {
                Points = new PointCollection
            {
                 new Point(TransX(CenterX -B+T/2),CenterY -InnerDiameter/2),  // 左上角
                 new Point(TransX(CenterX+ T/2), CenterY -InnerDiameter/2),  // 右上角
                 new Point(TransX(CenterX +T/2), CenterY + InnerDiameter/2),  // 右下角
                 new Point(TransX(CenterX -B+T/2), CenterY + InnerDiameter/2)   // 左下角
            },

                //设置填充颜色和边框
                Stroke = Brushes.Black,
                StrokeThickness = 0.2
            };
            Panel.SetZIndex(polygon, 5);//内圈
            container.Children.Add(polygon);


            Polygon polygon1 = new Polygon
            {
                Points = new PointCollection
        {
            new Point(CenterX - C/3, CenterY - InnerDiameter/2 - A/2 - A/4),  // 左上角
            new Point(CenterX + C/3, CenterY - InnerDiameter/2 - A/2 - A/4),  // 右上角
            new Point(CenterX + C/3, CenterY - InnerDiameter/2 - A/2 + A/4),  // 原始右下角
            new Point(CenterX - C/3, CenterY - InnerDiameter/2 - A/2 + A/4)   // 左下角
        },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White
            };

            RotateTransform rotateTransform = new RotateTransform
            {
                Angle = -Ta1,
                CenterX = CenterX,
                CenterY = CenterY - InnerDiameter / 2 - A / 2
            };
            polygon1.RenderTransform = rotateTransform;
            Panel.SetZIndex(polygon1, 3);
            container.Children.Add(polygon1);

            if (SetA == 1)
            {
                // 计算旋转后右下角坐标
                Point originalBottomRight = polygon1.Points[2]; // 原始右下角
                Point rotateCenter = new Point(rotateTransform.CenterX, rotateTransform.CenterY);
                double angleRadians = rotateTransform.Angle * Math.PI / 180;

                // 旋转后坐标计算
                double rotatedX = (originalBottomRight.X - rotateCenter.X) * Math.Cos(angleRadians)
                                - (originalBottomRight.Y - rotateCenter.Y) * Math.Sin(angleRadians)
                                + rotateCenter.X;
                double rotatedY = (originalBottomRight.X - rotateCenter.X) * Math.Sin(angleRadians)
                                + (originalBottomRight.Y - rotateCenter.Y) * Math.Cos(angleRadians)
                                + rotateCenter.Y;

                // 核心：将旋转后右下角的Y值赋值给Y1
                Y1 = rotatedY;  // Y1即为旋转后右下角的Y坐标
                X1 = rotatedX;
            }
            else if (SetA == -1)
            {
                // 计算旋转后右下角坐标
                Point originalBottomRight = polygon1.Points[3]; // 原始右下角
                Point rotateCenter = new Point(rotateTransform.CenterX, rotateTransform.CenterY);
                double angleRadians = rotateTransform.Angle * Math.PI / 180;

                // 旋转后坐标计算
                double rotatedX = (originalBottomRight.X - rotateCenter.X) * Math.Cos(angleRadians)
                                - (originalBottomRight.Y - rotateCenter.Y) * Math.Sin(angleRadians)
                                + rotateCenter.X;
                double rotatedY = (originalBottomRight.X - rotateCenter.X) * Math.Sin(angleRadians)
                                + (originalBottomRight.Y - rotateCenter.Y) * Math.Cos(angleRadians)
                                + rotateCenter.Y;

                // 核心：将旋转后右下角的Y值赋值给Y1
                Y1 = rotatedY;  // Y1即为旋转后右下角的Y坐标
                X1 = rotatedX;
            }
            else
            {
                MessageBox.Show("error"); MessageBox.Show("error");
                return;
            }
            Polygon polygon11 = new Polygon
            {
                Points = new PointCollection
    {
        new Point(CenterX - C/3, CenterY + InnerDiameter/2 + A/2 + A/4),  // 0:左上角
        new Point(CenterX + C/3, CenterY + InnerDiameter/2 + A/2 + A/4),  // 1:右上角（目标顶点）
        new Point(CenterX + C/3, CenterY + InnerDiameter/2 + A/2 - A/4),  // 2:右下角
        new Point(CenterX - C/3, CenterY + InnerDiameter/2 + A/2 - A/4)   // 3:左下角
    },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White
            };

            // polygon11的旋转变换（顺时针15度）
            RotateTransform rotateTransform1 = new RotateTransform
            {
                Angle = Ta1,  // 自身旋转角度：15度（顺时针）
                CenterX = CenterX,  // 旋转中心X
                CenterY = CenterY + InnerDiameter / 2 + A / 2  // 旋转中心Y
            };
            polygon11.RenderTransform = rotateTransform1;
            Panel.SetZIndex(polygon11, 3);
            container.Children.Add(polygon11);
            if (SetA == 1)
            {
                Point originalTopRight11 = polygon11.Points[2];
                Point rotateCenter11 = new Point(rotateTransform1.CenterX, rotateTransform1.CenterY);
                double angleRadians11 = rotateTransform1.Angle * Math.PI / 180;  // 修正：用rotateTransform1.Angle
                                                                                 // 旋转后X = (原始X - 旋转中心X) * cos(弧度) - (原始Y - 旋转中心Y) * sin(弧度) + 旋转中心X
                                                                                 // 旋转后Y = (原始X - 旋转中心X) * sin(弧度) + (原始Y - 旋转中心Y) * cos(弧度) + 旋转中心Y
                double rotatedTopRightX =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Cos(angleRadians11) -
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Sin(angleRadians11) +
                    rotateCenter11.X;
                double rotatedTopRightY =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Sin(angleRadians11) +
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Cos(angleRadians11) +
                    rotateCenter11.Y;

                X2 = rotatedTopRightX;  // 最终需要的右上角X
                Y2 = rotatedTopRightY;
            }
            else if (SetA == -1)
            {
                Point originalTopRight11 = polygon11.Points[3];
                Point rotateCenter11 = new Point(rotateTransform1.CenterX, rotateTransform1.CenterY);
                double angleRadians11 = rotateTransform1.Angle * Math.PI / 180;  // 修正：用rotateTransform1.Angle
                                                                                 // 旋转后X = (原始X - 旋转中心X) * cos(弧度) - (原始Y - 旋转中心Y) * sin(弧度) + 旋转中心X
                                                                                 // 旋转后Y = (原始X - 旋转中心X) * sin(弧度) + (原始Y - 旋转中心Y) * cos(弧度) + 旋转中心Y
                double rotatedTopRightX =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Cos(angleRadians11) -
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Sin(angleRadians11) +
                    rotateCenter11.X;
                double rotatedTopRightY =
                    (originalTopRight11.X - rotateCenter11.X) * Math.Sin(angleRadians11) +
                    (originalTopRight11.Y - rotateCenter11.Y) * Math.Cos(angleRadians11) +
                    rotateCenter11.Y;

                X2 = rotatedTopRightX;  // 最终需要的右上角X
                Y2 = rotatedTopRightY;
            }
            else
            {
                MessageBox.Show("error"); MessageBox.Show("error");
                return;
            }
            Polygon polygon2 = new Polygon
            {
                Points = new PointCollection
            {
                 new Point(TransX(CenterX -B+T/2) ,CenterY -InnerDiameter/2),  // 上
                 new Point(TransX(CenterX -B+T/2),CenterY -InnerDiameter/2-A/4),  // 上1
                 new Point(TransX(CenterX -B/2+T/4),CenterY -InnerDiameter/2-A/4),  // 右1
                 new Point(X1,Y1),  // 右上1(根号3）
                 new Point(X1,CenterY -InnerDiameter/2-A/2),  // 上1
                 new Point(X1,Y1-A/2/Math.Cos(15 * Math.PI / 180)),  // 上1(cos15)
                 new Point(TransX(CenterX-T/2),Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180)),   // 左下1(tan15)
                 new Point(TransX(CenterX-T/2),CenterY - InnerDiameter / 2-A/4),   // 下
                 new Point(TransX(CenterX - B + T / 2), CenterY - InnerDiameter / 2-A/4),
                 new Point(TransX(CenterX - B + T / 2), CenterY + InnerDiameter / 2+A/4),
                 new Point(TransX(CenterX-T/2),CenterY+ InnerDiameter / 2+A/4),
                 new Point(TransX(CenterX-T/2),2*CenterY -(Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180))),   // 下1(tan15)
                 new Point(X2,Y2+A/2/Math.Cos(15 * Math.PI / 180)),  // 上1(cos15)
                 new Point(X2,CenterY +InnerDiameter/2+A/2),  // 上1
                 new Point(X2,Y2),  // 右上1(根号3）
                 new Point(TransX(CenterX -B/2+T/4),CenterY+InnerDiameter/2+A/4),  // 右1
                 new Point(TransX(CenterX -B+T/2),CenterY +InnerDiameter/2+A/4),  // 上1
                 new Point(TransX(CenterX -B+T/2) ,CenterY +InnerDiameter/2),  // 上
                
            },
                // 设置填充颜色和边框0
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,

            };
            Panel.SetZIndex(polygon2, 1);
            container.Children.Add(polygon2);//空白斜线

            Polygon polygon3 = new Polygon
            {
                Points = new PointCollection
                {
                 new Point(X1,Y1-A/2/Math.Cos(15 * Math.PI / 180)),  // 右下角
                 new Point(X1,CenterY-OuterDiameter/2),  // 右上角
                 new Point(TransX(CenterX-T/2),CenterY-OuterDiameter/2),   // 左上角
                 new Point(TransX(CenterX-T/2),Y1-A/2/Math.Cos(15 * Math.PI / 180)+C*Math.Tan(15 * Math.PI / 180))   // 左下角
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: 45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon3, 1);
            container.Children.Add(polygon3);//外侧俩3

            Polygon polygon31 = new Polygon
            {
                Points = new PointCollection
                {
                 new Point(X2,CenterY+OuterDiameter/2),  // 右下角
                 new Point(X2,Y2+A/2/Math.Cos(15 * Math.PI / 180)),  // 右上角
                 new Point(TransX(CenterX-T/2),Y2+A/2/Math.Cos(15 * Math.PI / 180)-C*Math.Tan(15 * Math.PI / 180)),   // 左上角
                 new Point(TransX(CenterX-T/2),CenterY+OuterDiameter/2)   // 左下角
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: -45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon31, 1);
            container.Children.Add(polygon31);//外侧俩


            Polygon polygon4 = new Polygon
            {
                Points = new PointCollection//下
                {
                 new Point(X2,Y2),
                 new Point(X2,CenterY+InnerDiameter/2+A/2),
                 new Point(TransX(CenterX+T/2),CenterY+InnerDiameter/2+A/2),
                 new Point(TransX(CenterX+T/2),CenterY +InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY +InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY +InnerDiameter/2+A/4),
                new Point(TransX(CenterX-B/2+T/4),CenterY+InnerDiameter/2+A/4)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: 45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon4, 1);
            container.Children.Add(polygon4);//内侧俩

            Polygon polygon41 = new Polygon
            {
                Points = new PointCollection//下
                {
                 new Point(X1,Y1),
                 new Point(X1,CenterY-InnerDiameter/2-A/2),
                 new Point(TransX(CenterX+T/2),CenterY-InnerDiameter/2-A/2),
                 new Point(TransX(CenterX+T/2),CenterY -InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY-InnerDiameter/2),
                 new Point(TransX(CenterX-B+T/2),CenterY -InnerDiameter/2-A/4),
                new Point(TransX(CenterX -B/2+T/4),CenterY-InnerDiameter/2-A/4)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = CreateDiagonalBrush(
                angle: -45,        // 45度斜线
                lineThickness: 0.2, // 线条粗细
                lineColor: Brushes.Gray, // 线条颜色
                spacing: CalculateSpacing(OuterDiameter))
            };
            Panel.SetZIndex(polygon41, 1);
            container.Children.Add(polygon41);//内侧俩}

        }
        private DrawingBrush CreateDiagonalBrush(double angle = 45, double lineThickness = 1, Brush lineColor = null, double spacing = 10)//顺-，逆+,填充斜线
        {
            lineColor ??= Brushes.Black;
            DrawingBrush diagonalBrush = new DrawingBrush
            {
                Viewport = new Rect(0, 0, spacing, spacing), // 图案单元大小
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode = TileMode.Tile, // 平铺填充
                Stretch = Stretch.None
            };

            // 定义线条（根据角度直接设置起点终点，避免旋转误差）
            LineGeometry lineGeometry;
            if (angle == -45 || angle == 135) // -45度和135度是同方向斜线
            {
                // 直接定义从右上角到左下角的斜线（无需旋转）
                lineGeometry = new LineGeometry(
                    new Point(spacing, 0),   // 起点：图案单元右上角
                    new Point(0, spacing)    // 终点：图案单元左下角
                );
            }
            else if (angle == 45 || angle == -135)
            {
                // 45度斜线（默认）
                lineGeometry = new LineGeometry(
                    new Point(0, 0),         // 起点：图案单元左上角
                    new Point(spacing, spacing) // 终点：图案单元右下角
                );
            }
            else
            {
                // 其他角度仍使用旋转（保持兼容）
                lineGeometry = new LineGeometry(new Point(0, 0), new Point(spacing, spacing));
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform(-spacing / 2, -spacing / 2));
                transformGroup.Children.Add(new RotateTransform(angle));
                transformGroup.Children.Add(new TranslateTransform(spacing / 2, spacing / 2));
                lineGeometry.Transform = transformGroup;
            }

            // 应用线条样式
            diagonalBrush.Drawing = new GeometryDrawing
            {
                Geometry = lineGeometry,
                Pen = new Pen(lineColor, lineThickness)
                {
                    // 优化线条清晰度
                    StartLineCap = PenLineCap.Flat,
                    EndLineCap = PenLineCap.Flat
                }
            };

            return diagonalBrush;
        }
#endregion
#region//下面为深沟球轴承绘制
        public void DrawDeepBall(Canvas canvas)
        {
            Shaft.ComponentKey tag = new Shaft.ComponentKey(cPolygonType, BaseIndex);
            // 创建容器Canvas，作为轴承图元
            Canvas container = new Canvas();
            container.Tag = tag;
            double tmpBaseOffsetX = BaseOffsetX;
            BaseOffsetX = BaseOffsetX + canvas.ActualWidth / 2;
            GetComponentsLocate(container);

            // 在容器内绘制所有部件
            DrawDiameter(container, tag);
            DrawBall(container, tag);
            Drawlines(container, tag);
            // 将容器添加到主画布
            BaseOffsetX = tmpBaseOffsetX;
            canvas.Children.Add(container);
        }
        private void DrawDiameter(Canvas container, Shaft.ComponentKey tag)
        {
            double B = Per * (BaseOuterDiameter - BaseInnerDiameter) / 2;

            // 所有坐标基于容器原点(0,0)
            Polygon polygon = new Polygon
            {
                Points = new PointCollection
            {
                new Point(-BaseWidth/2, -BaseOuterDiameter/2 + B),  // 左上角
                new Point(BaseWidth/2, -BaseOuterDiameter/2 + B),    // 右上角
                new Point(BaseWidth/2, BaseOuterDiameter/2 - B),     // 右下角
                new Point(-BaseWidth/2, BaseOuterDiameter/2 - B)    // 左下角
            },
                Stroke = Brushes.Black,
                Tag = tag,
                StrokeThickness = 0.2
            };
            Panel.SetZIndex(polygon, 1);
            container.Children.Add(polygon);

            Polygon polygon1 = new Polygon
            {
                Points = new PointCollection
            {
                new Point(-BaseWidth/2, -BaseOuterDiameter/2),  // 左上角
                new Point(BaseWidth/2, -BaseOuterDiameter/2),  // 右上角
                new Point(BaseWidth/2, BaseOuterDiameter/2),   // 右下角
                new Point(-BaseWidth/2, BaseOuterDiameter/2)    // 左下角
            },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Tag = tag
            };
            Panel.SetZIndex(polygon1, 1);
            container.Children.Add(polygon1);

            Polygon polygon2 = new Polygon
            {
                Points = new PointCollection
            {
                new Point(-BaseWidth/2, -BaseInnerDiameter/2 - B),  // 左上角
                new Point(BaseWidth/2, -BaseInnerDiameter/2 - B),    // 右上角
                new Point(BaseWidth/2, BaseInnerDiameter/2 + B),      // 右下角
                new Point(-BaseWidth/2, BaseInnerDiameter/2 + B)      // 左下角
            },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Tag = tag
            };
            Panel.SetZIndex(polygon2, 1);
            container.Children.Add(polygon2);

            Polygon polygon3 = new Polygon
            {
                Points = new PointCollection
            {
                new Point(-BaseWidth/2, -BaseInnerDiameter/2),  // 左上角
                new Point(BaseWidth/2, -BaseInnerDiameter/2),   // 右上角
                new Point(BaseWidth/2, BaseInnerDiameter/2),    // 右下角
                new Point(-BaseWidth/2, BaseInnerDiameter/2)    // 左下角
            },
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Tag = tag
            };
            Panel.SetZIndex(polygon3, 1);
            container.Children.Add(polygon3);
        }
        private void DrawBall(Canvas container, Shaft.ComponentKey tag)
        {
            double P = 0.1 * (BaseOuterDiameter - BaseInnerDiameter);
            double N = 0.03 * (BaseOuterDiameter - BaseInnerDiameter); // 偏离量
            double R = (BaseOuterDiameter - BaseInnerDiameter) * (1 - 2 * Per) / 4;

            // 第一个滚珠
            Ellipse circle = new Ellipse
            {
                Width = 2 * R + P,
                Height = 2 * R + P,
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White,
                Tag = tag
            };

            // 设置相对于容器的位置
            Canvas.SetLeft(circle, -R - P / 2);
            Canvas.SetTop(circle, -R - BaseInnerDiameter / 2 - (BaseOuterDiameter - BaseInnerDiameter) / 4 - N);
            Panel.SetZIndex(circle, 2);
            container.Children.Add(circle);

            // 第二个滚珠
            Ellipse circle1 = new Ellipse
            {
                Width = 2 * R + P,
                Height = 2 * R + P,
                Stroke = Brushes.Black,
                StrokeThickness = 0.2,
                Fill = Brushes.White,
                Tag = tag
            };

            Canvas.SetLeft(circle1, -R - P / 2);
            Canvas.SetTop(circle1, -R + (3 * BaseInnerDiameter - BaseOuterDiameter) / 4 + Per * (BaseOuterDiameter - BaseInnerDiameter) + P + N);
            Panel.SetZIndex(circle1, 2);
            container.Children.Add(circle1);
        }
        private void Drawlines(Canvas container, Shaft.ComponentKey tag)
        {
            double X = -BaseWidth / 2; // 基于容器原点
            double Y = BaseInnerDiameter / 2; // 下边界
            double length = Per * (BaseOuterDiameter - BaseInnerDiameter) / 2;
            double spacing = CalculateSpacing(BaseOuterDiameter);
            int count = (int)(BaseWidth / spacing) + 3;
            int n = 1; // 斜线方向

            // 镜像线位置（基于容器原点）
            double Z1 = BaseInnerDiameter / 2 + (BaseOuterDiameter - BaseInnerDiameter) / 4;
            DrawDiagonalLines1(container, new Point(X, Y), length, spacing, count, n, tag);
            DrawMirroredDiagonalLines(container, new Point(X, Y), length, spacing, count, n, Z1, tag);

            Y = -BaseInnerDiameter / 2 - Per * (BaseOuterDiameter - BaseInnerDiameter) / 2;
            DrawDiagonalLines1(container, new Point(X, Y), length, spacing, count, n, tag);

            double Z2 = -BaseInnerDiameter / 2 - (BaseOuterDiameter - BaseInnerDiameter) / 4;
            DrawMirroredDiagonalLines(container, new Point(X, Y), length, spacing, count, n, Z2, tag);
        }
        private void DrawDiagonalLines1(Canvas container, Point startPoint, double length, double spacing, int count, int n, Shaft.ComponentKey tag)
        {
            double rightBound = BaseWidth / 2; // 容器内的右边界
            double leftBound = -BaseWidth / 2; // 容器内的左边界

            switch (n)
            {
                case 1: // 左上点定位，右斜向下
                    for (int i = 0; i < count; i++)
                    {
                        Point currentStart = new Point(
                            startPoint.X + i * spacing,
                            startPoint.Y
                        );

                        double endX = currentStart.X + length;
                        double endY = currentStart.Y + length;

                        // 边界检查
                        if (endX >= rightBound)
                        {
                            double remainingLength = rightBound - currentStart.X;
                            endX = rightBound;
                            endY = currentStart.Y + remainingLength;
                        }

                        Line line = new Line
                        {
                            X1 = currentStart.X,
                            Y1 = currentStart.Y,
                            X2 = endX,
                            Y2 = endY,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.2,
                            Tag = tag
                        };

                        Panel.SetZIndex(line, -2);
                        container.Children.Add(line);

                        if (endX >= rightBound) break;
                    }
                    break;

                case 2: // 左下点定位，左斜向上
                    for (int i = 0; i < count; i++)
                    {
                        Point currentStart = new Point(
                            startPoint.X + i * spacing,
                            startPoint.Y
                        );

                        double endX = currentStart.X - length;
                        double endY = currentStart.Y + length;

                        if (endX < leftBound)
                        {
                            double remainingLength = currentStart.X - leftBound;
                            endX = leftBound;
                            endY = currentStart.Y + remainingLength;
                        }

                        if (currentStart.X > rightBound) continue;

                        Line line = new Line
                        {
                            X1 = endX,
                            Y1 = endY,
                            X2 = currentStart.X,
                            Y2 = currentStart.Y,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.2,
                            Tag = tag
                        };

                        Panel.SetZIndex(line, -2);
                        container.Children.Add(line);
                    }
                    break;
            }
        }
        private void DrawMirroredDiagonalLines(Canvas container, Point startPoint, double length, double spacing, int count, int n, double mirrorY, Shaft.ComponentKey tag)
        {
            double rightBound = BaseWidth / 2; // 容器内的右边界

            switch (n)
            {
                case 1: // 镜像处理右上到左下的斜线
                    for (int i = 0; i < count; i++)
                    {
                        Point currentStart = new Point(
                            startPoint.X + i * spacing,
                            startPoint.Y
                        );

                        double endX = currentStart.X + length;
                        double endY = currentStart.Y + length;

                        if (endX >= rightBound)
                        {
                            double remainingLength = rightBound - currentStart.X;
                            endX = rightBound;
                            endY = currentStart.Y + remainingLength;
                        }

                        // 计算镜像坐标
                        double mirroredStartY = 2 * mirrorY - currentStart.Y;
                        double mirroredEndY = 2 * mirrorY - endY;

                        Line line = new Line
                        {
                            X1 = currentStart.X,
                            Y1 = mirroredStartY,
                            X2 = endX,
                            Y2 = mirroredEndY,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.2,
                            Tag = tag
                        };

                        Panel.SetZIndex(line, -2);
                        container.Children.Add(line);

                        if (endX >= rightBound) break;
                    }
                    break;
            }
        }
        private double CalculateSpacing(double outerDiameter)
        {
            return outerDiameter > 0 && outerDiameter <= 10 ? 0.5 :
                   (outerDiameter > 10 && outerDiameter <= 30 ? 1 :
                   (outerDiameter > 30 && outerDiameter <= 50 ? 3 :
                   (outerDiameter > 50 && outerDiameter <= 200 ? 5 :
                   (outerDiameter > 200 && outerDiameter <= 1000 ? 10 : 100))));
        }
#endregion
    }
    /// <summary>
    /// 轴类
    /// </summary>
    public class Shaft
    {
        // 原始轴段数据（用户输入的，不会被拆分修改）
        public List<DrawAxisDimension> OriginalAxisSections { get; set; } = new List<DrawAxisDimension>();
        // 用于绘制的轴段（可能是拆分后的）
        public List<DrawAxisDimension> AxisSections { get; set; } = new List<DrawAxisDimension>();
        public List<DrawBearing> Bearings { get; set; } = new List<DrawBearing>();
        public List<DrawGear> Gears { get; set; } = new List<DrawGear>();
        public List<DrawCycloidWheel> CycloidWheels { get; set; } = new List<DrawCycloidWheel>();

        //包含所有的轴段以及附属图形
        public record struct ComponentKey(PolygonType Type, int Index);
        [JsonIgnore]
        public static Dictionary<ComponentKey, DrawBase> componentDict = new();
        public Shaft() { }
        public int ShaftId { get; set; }

        // 构造函数
        public Shaft(int shaftId)
        {
            ShaftId = shaftId;
        }
        /// <summary>
        /// 添加原始轴段（用户输入时调用）
        /// </summary>
        public void AddOriginalAxisSection(DrawAxisDimension section)
        {
            section.BaseIndex = OriginalAxisSections.Count;
            section.OriginalIndex = OriginalAxisSections.Count; // 记录原始索引
            OriginalAxisSections.Add(section);
        }
        /// <summary>
        /// 删除原始轴段
        /// </summary>
        public void RemoveOriginalAxisSection(int originalIndex)
        {
            if (originalIndex >= 0 && originalIndex < OriginalAxisSections.Count)
            {
                OriginalAxisSections.RemoveAt(originalIndex);
                // 重新编号
                for (int i = 0; i < OriginalAxisSections.Count; i++)
                {
                    OriginalAxisSections[i].BaseIndex = i;
                    OriginalAxisSections[i].OriginalIndex = i;
                }
            }
        }
        /// <summary>
        /// 获取拆分轴段对应的原始轴段索引
        /// </summary>
        public int GetOriginalIndexFromSplitIndex(int splitIndex)
        {
            if (splitIndex >= 0 && splitIndex < AxisSections.Count)
            {
                return AxisSections[splitIndex].OriginalIndex;
            }
            return -1;
        }
        // 绘制整根轴(仅包括轴段)
        public void DrawShaft(Canvas annotationCanvas, Canvas canvas)
        {
            double currentLength = canvas.ActualWidth / 2;
            foreach (var section in AxisSections)
            {
                section.BasicDrawing(canvas, currentLength);
                currentLength += section.BaseWidth;
                section.DrawDimension(annotationCanvas, canvas, currentLength);
            }
            if (!canvas.Children.Contains(annotationCanvas))
            {
                canvas.Children.Add(annotationCanvas);
            }
        }
        public void RebuildComponentDict()
        {
            Shaft.componentDict.Clear();
            // 轴段
            for (int i = 0; i < AxisSections.Count; i++)
            {
                Shaft.componentDict[new ComponentKey(PolygonType.Axis, i)] = AxisSections[i];
            }
            // 轴承
            for (int i = 0; i < Bearings.Count; i++)
            {
                Shaft.componentDict[new ComponentKey(PolygonType.Bearing, i)] = Bearings[i];
            }
            // 齿轮
            for (int i = 0; i < Gears.Count; i++)
            {
                Shaft.componentDict[new ComponentKey(PolygonType.Gear, i)] = Gears[i];
            }
            // 摆线轮
            for (int i = 0; i < CycloidWheels.Count; i++)
            {
                Shaft.componentDict[new ComponentKey(PolygonType.CycloidWheel, i)] = CycloidWheels[i];
            }
        }
        public void DrawShaftWithComponents(Canvas annotationCanvas, Canvas canvas)
        {
            canvas.Children.Clear();
            annotationCanvas.Children.Clear();

            // 每次绘制时，从原始轴段数据重新计算拆分
            RecalculateAxisSectionsFromCycloidWheels();

            DrawShaft(annotationCanvas, canvas);//必须先绘制轴段，不然无法正确得到轴段的Y轴偏移
            // 重新绘制除轴段外的所有组件
            foreach (var bearing in Bearings)
            {
                bearing.BasicDrawing(canvas);
            }
            foreach (var gear in Gears)
            {
                gear.BasicDrawing(canvas);
            }
            foreach (var cycloidWheel in CycloidWheels)
            {
                cycloidWheel.BasicDrawing(canvas);
            }
        }
        /// <summary>
        /// 从原始轴段数据重新计算拆分（每次绘制时调用）
        /// 根据摆线轮的位置和Y偏移自动拆分轴段
        /// </summary>
        public void RecalculateAxisSectionsFromCycloidWheels()
        {
            // 清空当前绘制用的轴段列表
            AxisSections.Clear();

            // 如果没有原始轴段，直接返回
            if (OriginalAxisSections.Count == 0)
            {
                return;
            }

            // 收集所有摆线轮的覆盖区域（包含Y偏移和对应的滚针轴承内径）
            var coverageRegions = new List<(double Left, double Right, double YOffset, double NeedleBearingInnerDiameter)>();
            foreach (var cycloidWheel in CycloidWheels)
            {
                //if (Math.Abs(cycloidWheel.YDirectionOffset) < 0.001) continue;

                double cycloidMidpoint = cycloidWheel.GetComponentsLocateMidpoint(cycloidWheel.BaseFace);
                double cycloidLeft = cycloidMidpoint - cycloidWheel.BaseWidth / 2;
                double cycloidRight = cycloidMidpoint + cycloidWheel.BaseWidth / 2;

                // 查找对应的滚针轴承内径
                double needleInnerDiameter = 0;
                foreach (var bearing in Bearings)
                {
                    if (bearing.BearingType == "滚针轴承")
                    {
                        double bearingMidpoint = bearing.GetComponentsLocateMidpoint(bearing.BaseFace);
                        double bearingLeft = bearingMidpoint - bearing.BaseWidth / 2;
                        double bearingRight = bearingMidpoint + bearing.BaseWidth / 2;

                        // 检查滚针轴承是否在摆线轮范围内
                        if (bearingLeft >= cycloidLeft - 0.001 && bearingRight <= cycloidRight + 0.001)
                        {
                            needleInnerDiameter = bearing.BaseInnerDiameter;
                            break;
                        }
                    }
                }

                coverageRegions.Add((cycloidLeft, cycloidRight, cycloidWheel.YDirectionOffset, needleInnerDiameter));
            }           

            //// 如果没有需要偏移的摆线轮，直接复制原始轴
            //if (coverageRegions.Count == 0)
            //{
            //    for (int i = 0; i < OriginalAxisSections.Count; i++)
            //    {
            //        var copy = CloneAxisSection(OriginalAxisSections[i]);
            //        copy.BaseIndex = i;
            //        copy.OriginalIndex = i;
            //        AxisSections.Add(copy);
            //    }
            //    return;
            //}

            // 遍历原始轴段，进行拆分
            double accumulatedLength = 0;
            for (int origIdx = 0; origIdx < OriginalAxisSections.Count; origIdx++)
            {
                var originalSection = OriginalAxisSections[origIdx];
                double axisLeft = accumulatedLength;
                double axisRight = accumulatedLength + originalSection.BaseWidth;

                // 查找与当前轴段重叠的摆线轮区域
                var overlappingRegions = coverageRegions
                    .Where(r => r.Right > axisLeft && r.Left < axisRight)
                    .OrderBy(r => r.Left)
                    .ToList();

                if (overlappingRegions.Count == 0)
                {
                    // 没有重叠，复制原始轴段
                    var copy = CloneAxisSection(originalSection);
                    copy.BaseIndex = AxisSections.Count;
                    copy.OriginalIndex = origIdx;
                    AxisSections.Add(copy);
                }
                else
                {
                    // 需要拆分轴段
                    var splitSections = SplitAxisSection(originalSection, axisLeft, axisRight, overlappingRegions, origIdx);
                    foreach (var section in splitSections)
                    {
                        section.BaseIndex = AxisSections.Count;
                        AxisSections.Add(section);
                    }
                }

                accumulatedLength += originalSection.BaseWidth;
            }
        }
        // 摆线轮覆盖区域信息类
        private class CycloidCoverageRegion
        {
            public double Left { get; set; }
            public double Right { get; set; }
            public double YOffset { get; set; }
            public double BearingInnerDiameter { get; set; }  // 对应滚针轴承的内径
        }


        /// <summary>
        /// 查找与摆线轮位置匹配的滚针轴承
        /// </summary>
        private DrawBearing FindMatchingNeedleBearing(double cycloidMidpoint, double cycloidWidth)
        {
            double cycloidLeft = cycloidMidpoint - cycloidWidth / 2;
            double cycloidRight = cycloidMidpoint + cycloidWidth / 2;

            foreach (var bearing in Bearings)
            {
                if (bearing.BearingType != "滚针轴承") continue;

                double bearingMidpoint = bearing.GetComponentsLocateMidpoint(bearing.BaseFace);
                double bearingLeft = bearingMidpoint - bearing.BaseWidth / 2;
                double bearingRight = bearingMidpoint + bearing.BaseWidth / 2;

                // 检查滚针轴承是否在摆线轮范围内
                if (bearingLeft >= cycloidLeft - 0.001 && bearingRight <= cycloidRight + 0.001)
                {
                    return bearing;
                }
            }
            return null;
        }

        /// <summary>
        /// 克隆轴段（深拷贝）
        /// </summary>
        private DrawAxisDimension CloneAxisSection(DrawAxisDimension original)
        {
            return new DrawAxisDimension
            {
                BaseWidth = original.BaseWidth,
                BaseInnerDiameter = original.BaseInnerDiameter,
                BaseOuterDiameter = original.BaseOuterDiameter,
                IsInnerTapered = original.IsInnerTapered,
                IsOuterTapered = original.IsOuterTapered,
                SmallEndInnerDiameter = original.SmallEndInnerDiameter,
                SmallEndOuterDiameter = original.SmallEndOuterDiameter,
                IsDirectionInnerLChecked = original.IsDirectionInnerLChecked,
                IsDirectionInnerRChecked = original.IsDirectionInnerRChecked,
                IsDirectionOuterLChecked = original.IsDirectionOuterLChecked,
                IsDirectionOuterRChecked = original.IsDirectionOuterRChecked,
                IsPeccentricshaft = original.IsPeccentricshaft,
                EccentricDiameter = original.EccentricDiameter,
                Phase = original.Phase,
                SectionType = original.SectionType,
                OriginalIndex = original.OriginalIndex
            };
        }
        /// <summary>
        /// 将一个轴段根据覆盖区域拆分为多个子轴段
        /// </summary>
        private List<DrawAxisDimension> SplitAxisSection(
            DrawAxisDimension original,
            double axisLeft,
            double axisRight,
            List<(double Left, double Right, double YOffset, double NeedleBearingInnerDiameter)> overlappingRegions,
            int originalIndex)
        {
            var result = new List<DrawAxisDimension>();

            // 收集所有分割点
            var splitPoints = new SortedSet<double> { axisLeft, axisRight };
            foreach (var region in overlappingRegions)
            {
                if (region.Left > axisLeft && region.Left < axisRight)
                    splitPoints.Add(region.Left);
                if (region.Right > axisLeft && region.Right < axisRight)
                    splitPoints.Add(region.Right);
            }

            var pointList = splitPoints.ToList();

            // 为每个子区间创建轴段
            for (int i = 0; i < pointList.Count - 1; i++)
            {
                double segLeft = pointList[i];
                double segRight = pointList[i + 1];
                double segWidth = segRight - segLeft;

                if (segWidth < 0.001) continue; // 跳过太小的段

                // 检查这个子区间是否被摆线轮覆盖
                var coveringRegion = overlappingRegions
                    .FirstOrDefault(r => r.Left <= segLeft + 0.001 && r.Right >= segRight - 0.001);

                bool isCovered = coveringRegion != default;
                double yOffset = isCovered ? coveringRegion.YOffset : 0;
                double needleInnerDiameter = isCovered ? coveringRegion.NeedleBearingInnerDiameter : 0;

                // 创建新的轴段
                var newSection = CreateSplitSection(original, segWidth, isCovered, yOffset, needleInnerDiameter, originalIndex);
                result.Add(newSection);
            }

            return result;
        }

        /// <summary>
        /// 创建拆分后的轴段
        /// </summary>
        private DrawAxisDimension CreateSplitSection(
            DrawAxisDimension original,
            double newWidth,
            bool isCovered,
            double yOffset,
            double needleInnerDiameter,
            int originalIndex)
        {
            var newSection = new DrawAxisDimension
            {
                BaseWidth = newWidth,
                BaseInnerDiameter = original.BaseInnerDiameter,
                // 如果被覆盖且有滚针轴承内径，则使用滚针轴承内径作为外径
                BaseOuterDiameter = (isCovered && needleInnerDiameter > 0) ? needleInnerDiameter : original.BaseOuterDiameter,
                // 拆分后不再保持锥形（简化处理）
                IsInnerTapered = false,
                IsOuterTapered = false,
                SmallEndInnerDiameter = original.BaseInnerDiameter,
                SmallEndOuterDiameter = (isCovered && needleInnerDiameter > 0) ? needleInnerDiameter : original.BaseOuterDiameter,
                IsDirectionInnerLChecked = false,
                IsDirectionInnerRChecked = false,
                IsDirectionOuterLChecked = false,
                IsDirectionOuterRChecked = false,
                OriginalIndex = originalIndex
            };

            // 设置偏心参数
            if (isCovered && Math.Abs(yOffset) > 0.001)
            {
                newSection.IsPeccentricshaft = true;
                newSection.EccentricDiameter = yOffset;
                newSection.Phase = 0;  // Phase=0时，cos(0)=1，偏移量直接等于EccentricDiameter
                newSection.SectionType = 2;
            }
            else
            {
                // 保持原始轴段的偏心设置
                newSection.IsPeccentricshaft = original.IsPeccentricshaft;
                newSection.EccentricDiameter = original.EccentricDiameter;
                newSection.Phase = original.Phase;
                newSection.SectionType = original.SectionType;
            }

            return newSection;
        }
    }
    public class DrawAxisDimension : DrawBase
    {
        public bool IsInnerTapered;
        public bool IsOuterTapered;
        public double SmallEndInnerDiameter;
        public double SmallEndOuterDiameter;
        public bool IsDirectionInnerLChecked;
        public bool IsDirectionInnerRChecked;
        public bool IsDirectionOuterLChecked;
        public bool IsDirectionOuterRChecked;
        public bool IsPeccentricshaft;
        public double EccentricDiameter;
        public double Phase;
        public int SectionType;
        public double CurrentLength { get; set; } // 存储轴段当前长度
        public int OriginalIndex { get; set; } = -1; // 新增：记录原始轴段索引，用于删除时定位
        public DrawAxisDimension() { }
        public DrawAxisDimension(object[] data)
        {
            DefineFromArray(data);
        }
        public override void DefineFromArray(object[] data)
        {
            BaseWidth = Convert.ToDouble(data[0]);
            BaseInnerDiameter = Convert.ToDouble(data[1]);
            BaseOuterDiameter = Convert.ToDouble(data[2]);
            IsInnerTapered = Convert.ToBoolean(data[3]);
            IsOuterTapered = Convert.ToBoolean(data[4]);
            SmallEndInnerDiameter = Convert.ToDouble(data[5]);
            SmallEndOuterDiameter = Convert.ToDouble(data[6]);
            IsDirectionInnerLChecked = Convert.ToBoolean(data[7]);
            IsDirectionInnerRChecked = Convert.ToBoolean(data[8]);
            IsDirectionOuterLChecked = Convert.ToBoolean(data[9]);
            IsDirectionOuterRChecked = Convert.ToBoolean(data[10]);
            IsPeccentricshaft = Convert.ToBoolean(data[11]);
            EccentricDiameter = Convert.ToDouble(data[12]);
            Phase = Convert.ToDouble(data[13]);
            SectionType = Convert.ToInt32(data[14]);
        }
        public override void BasicDrawing(Canvas canvas)
        {
            BasicDrawing(canvas, CurrentLength);
        }
        public void BasicDrawing(Canvas canvas, double currentLength)
        {

            CurrentLength = currentLength;
            double x = currentLength;
            double y = canvas.ActualHeight / 2;
            // 绘制外径的多边形
            Polygon outerPolygon = new Polygon
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 0.2,
                Fill = Brushes.White,  // 使用浅色填充外径                
                //Tag = new ComponentKey(PolygonType.Axis, BaseIndex)
            };
            // 使用 OriginalIndex 而不是 BaseIndex 来设置 Tag，这样删除时能正确找到原始轴段
            outerPolygon.Tag = new AxisTagWithParam
            {
                Key = new Shaft.ComponentKey(PolygonType.Axis, this.OriginalIndex >= 0 ? this.OriginalIndex : this.BaseIndex),
                IsPeccentricshaft = this.IsPeccentricshaft,
                EccentricDiameter = this.EccentricDiameter,
                Phase = this.Phase
            };

            // 绘制内径的多边形
            Polygon innerPolygon = new Polygon
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 0.2,
                Fill = Brushes.White, // 使用白色填充内径
                StrokeDashArray = new DoubleCollection { 2, 2 }, // 设置虚线
                Tag = outerPolygon.Tag
            };

            double radians = Phase * Math.PI / 180;
            double eccentricOffset = EccentricDiameter * Math.Cos(radians);
            // 判断外径锥度的方向并绘制
            if (IsOuterTapered && IsDirectionOuterLChecked == true)
            {

                outerPolygon.Points = new PointCollection
                {
                    new Point(x,             y - BaseOuterDiameter / 2 - eccentricOffset),                     // 左上角（大端外径）
                    new Point(x + BaseWidth, y - SmallEndOuterDiameter / 2 - eccentricOffset),                 // 右上角（小端外径）
                    new Point(x + BaseWidth, y + SmallEndOuterDiameter / 2 - eccentricOffset),                 // 右下角（小端外径）
                    new Point(x,             y + BaseOuterDiameter / 2 - eccentricOffset)                      // 左下角（大端外径）
                };
            }
            else if (IsOuterTapered && IsDirectionOuterRChecked == true)
            {
                outerPolygon.Points = new PointCollection
                {
                    new Point(x,             y - SmallEndOuterDiameter / 2 - eccentricOffset),              // 左上角（小端外径）
                    new Point(x + BaseWidth, y - BaseOuterDiameter / 2 - eccentricOffset),                  // 右上角（大端外径）
                    new Point(x + BaseWidth, y + BaseOuterDiameter / 2 - eccentricOffset),                  // 右下角（大端外径）
                    new Point(x,             y + SmallEndOuterDiameter / 2 - eccentricOffset)               // 左下角（小端外径）
                };
            }
            else
            {
                outerPolygon.Points = new PointCollection
                {
                    new Point(x,             y - BaseOuterDiameter / 2 - eccentricOffset),
                    new Point(x + BaseWidth, y - BaseOuterDiameter / 2 - eccentricOffset),
                    new Point(x + BaseWidth, y + BaseOuterDiameter / 2 - eccentricOffset),
                    new Point(x,             y + BaseOuterDiameter / 2 - eccentricOffset)
                };
            }
            // 判断内径锥度的方向并绘制
            if (IsInnerTapered && IsDirectionInnerLChecked == true)
            {
                innerPolygon.Points = new PointCollection
                {
                    new Point(x,             y - BaseInnerDiameter / 2 - eccentricOffset),                     // 左上角（大端内径）
                    new Point(x + BaseWidth, y - SmallEndInnerDiameter / 2 - eccentricOffset),                  // 右上角（小端内径）
                    new Point(x + BaseWidth, y + SmallEndInnerDiameter / 2 - eccentricOffset),                  // 右下角（小端内径）
                    new Point(x,             y + BaseInnerDiameter / 2 - eccentricOffset)                      // 左下角（大端内径）
                };
            }
            else if (IsInnerTapered && IsDirectionInnerRChecked == true)
            {
                innerPolygon.Points = new PointCollection
                {
                    new Point(x,             y - SmallEndInnerDiameter / 2 - eccentricOffset),              // 左上角（小端内径）
                    new Point(x + BaseWidth, y - BaseInnerDiameter / 2 - eccentricOffset),                  // 右上角（大端内径）
                    new Point(x + BaseWidth, y + BaseInnerDiameter / 2 - eccentricOffset),                  // 右下角（大端内径）
                    new Point(x,             y + SmallEndInnerDiameter / 2 - eccentricOffset)               // 左下角（小端内径）
                };
            }
            else
            {
                innerPolygon.Points = new PointCollection
                {
                    new Point(x,             y - BaseInnerDiameter / 2 - eccentricOffset),
                    new Point(x + BaseWidth, y - BaseInnerDiameter / 2 - eccentricOffset),
                    new Point(x + BaseWidth, y + BaseInnerDiameter / 2 - eccentricOffset),
                    new Point(x,             y + BaseInnerDiameter / 2 - eccentricOffset)
                };
            }
            canvas.Children.Add(outerPolygon);// 添加外多边形到画布
            canvas.Children.Add(innerPolygon); // 添加内多边形到画布

        }
        public void DrawDimension(Canvas annotationCanvas, Canvas canvas, double currentLength)
        {
            annotationCanvas.Children.Clear();
            double minY = FindMinYOfAllAxisPolygons(canvas);
            GraphicAnnotation.ShowTotalDimensions(annotationCanvas, canvas, currentLength, minY);
        }
        double FindMinYOfAllAxisPolygons(Canvas canvas)
        {
            double minY = double.MaxValue;

            foreach (var polygon in canvas.Children.OfType<Polygon>())
            {
                if (polygon.Tag is AxisTagWithParam tag)
                {
                    foreach (var point in polygon.Points)
                    {
                        if (point.Y < minY)
                            minY = point.Y;
                    }
                }
            }
            return minY;
        }

        public int GetSectionType()
        {
            return SectionType;
        }
    }
    interface IDraw
    {
        //void DrawBeam(double x, double y, double length, double innerDiameter, double outerDiameter, bool isInnerTapered, bool isOuterTapered, double smallEndInnerDiameter, double smallEndOuterDiameter,
        //                        bool isPdirectioninnerL, bool isPdirectioninnerR, bool isPdirectionouterL, bool isPdirectionouterR,
        //                        bool isPeccentricshaft, double eccentricDiameter, double phase, int index);//x,y为绘制轴段图形的起点
        //void DrawBearing(double bearinDistance, double innerDiameter, double outerDiameter, double bearingWidth, bool isTaper, string baseFace, string orientation, int index); // 定义轴承接口，但轴承暂时未设置偏心！
        //void DrawGear(double gearDistance, double gearWidth, double gearHeight, string baseFace, int index);//定义齿轮绘制的接口
        //void DrawNeedleRoller(double distanceFromStart, double d, double D, double B, string baseFace, int index); // 定义滚针接口
    }
    public static class GraphicAnnotation
    {

        //绘制箭头
        public static void DrawArrow(Canvas canvas, double x, double y, double size, bool isLeft)
        {
            Line arrowLine1 = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = isLeft ? x + size : x - size,
                Y2 = y - size,
                Stroke = Brushes.Blue,
                StrokeThickness = 0.2
            };
            Line arrowLine2 = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = isLeft ? x + size : x - size,
                Y2 = y + size,
                Stroke = Brushes.Blue,
                StrokeThickness = 0.2
            };
            canvas.Children.Add(arrowLine1);
            canvas.Children.Add(arrowLine2);
        }
        public static void DrawLine(Canvas annotationCanvas, Canvas canvas)
        {
            double startX = canvas.ActualWidth / 2; // 初始X坐标
            // 创建中心线
            Line centerLine = new Line
            {
                X1 = 0,                // 起点X坐标
                Y1 = canvas.ActualHeight / 2, // 起点Y坐标 (Canvas垂直中心)
                X2 = canvas.ActualWidth,      // 终点X坐标
                Y2 = canvas.ActualHeight / 2, // 终点Y坐标 (Canvas垂直中心)
                Stroke = Brushes.Blue,  // 线条颜色
                StrokeThickness = 0.2,    // 线条粗细
                StrokeDashArray = new DoubleCollection() { 1, 2 } // 设置为点划线，1是点的长度，2是间隔
            };
            // 创建垂直中心线
            Line verticalCenterLine = new Line
            {
                X1 = startX,  // 起点X坐标 (Canvas水平中心)
                Y1 = 0,                // 起点Y坐标
                X2 = startX,  // 终点X坐标 (Canvas水平中心)
                Y2 = canvas.ActualHeight,     // 终点Y坐标
                Stroke = Brushes.Blue, // 线条颜色
                StrokeThickness = 0.2    // 线条粗细
            };

            // 将垂直中心线添加到Canvas
            annotationCanvas.Children.Add(verticalCenterLine);
            // 将中心线添加到Canvas
            annotationCanvas.Children.Add(centerLine);
            Panel.SetZIndex(verticalCenterLine, int.MaxValue);
            Panel.SetZIndex(centerLine, int.MaxValue); // 确保它显示在最上层
        }
        // 显示整根轴的总尺寸
        public static void ShowTotalDimensions(Canvas annotationCanvas, Canvas canvas, double totalLength, double outerDiameter)
        {
            double startX = canvas.ActualWidth / 2;
            DrawLine(annotationCanvas, canvas);
            double offset = 20; // 总尺寸线的偏移量
            double arrowSize = 5; // 箭头大小
            // 绘制总尺寸线
            Line totalLine = new Line
            {
                X1 = startX,
                Y1 = outerDiameter - offset,
                X2 = totalLength,
                Y2 = outerDiameter - offset,
                Stroke = Brushes.Blue,
                StrokeThickness = 0.2
            };
            annotationCanvas.Children.Add(totalLine);
            // 绘制左侧箭头
            DrawArrow(annotationCanvas, startX, outerDiameter - offset, arrowSize, true);
            // 绘制右侧箭头
            DrawArrow(annotationCanvas, 0 + totalLength, outerDiameter - offset, arrowSize, false);
            // 显示总尺寸文本
            TextBlock totalDimensionText = new TextBlock
            {
                Text = $"总长度: {(totalLength - startX).ToString("F2")}",
                Foreground = Brushes.Blue,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(totalDimensionText, startX / 2 + totalLength / 2 - 20);
            Canvas.SetTop(totalDimensionText, outerDiameter - offset - 20);
            annotationCanvas.Children.Add(totalDimensionText);

            /*            // 添加dimensionCanvas到Pcanvas中
                        if (!canvas.Children.Contains(annotationCanvas))
                        {
                            canvas.Children.Add(annotationCanvas);
                        }*/
        }
    }
    public partial class CTANGirderPage : Page, IDraw
    {
        //当有多根轴时
        private List<Shaft> allShafts = new List<Shaft>(); // 所有轴
        private int shaftIdCounter = 0;   // 自动编号Shaft
        public Shaft currentShaft = null;
        //private bool isCreatingNewShaft = true; //暂时先注释掉，因为推荐完绘制了图形后，再点击插入轴段，就会自动创建新的轴段，导致报错！后续如果需要绘制多根轴，再打开此功能！
        //初始化
        public Canvas annotationCanvas = new Canvas();// 定义一个Canvas用于存放总尺寸标注的元素，便于后面单独刷新！
        //缩放
        private ScaleTransform canvasScaleTransform = new ScaleTransform(1.0, 1.0);
        private const double MinScale = 0.1; // 最小缩放比例
        private const double MaxScale = 10.0; // 最大缩放比例
                                              // 类字段
        private bool isCentering = false;
        private DispatcherTimer centerDebounceTimer;
        private bool pendingResetScale = false;
        // 用户交互标志（用于在拖拽时暂停自动居中）
        private bool isUserPanning = false;

        // 鼠标拖拽时记录点（使用窗口坐标，和原逻辑兼容）
        private Point lastMousePositionOnScreen;

        //鼠标左键拖动
        private bool isDragging = false;
        private Point lastMousePosition;
        private TranslateTransform canvasTranslateTransform = new TranslateTransform();
        private TransformGroup canvasTransformGroup = new TransformGroup();

        //高亮显示
        private Polygon highlightedPolygon = null; // 用于存储当前高亮的多边形
        private Canvas highlightedCanvas = null;// 用于存储当前高亮的画布
        private Shaft.ComponentKey? selectedComponentKey = null;
        //private double beamCurrentLength { get; set; }// 轴段的当前长度
        //private bool stopAdd = false;
        //private VassalData selectedAxisData;//临时存储鼠标选中的轴段数据

        // 添加父级引用属性
        public CTANCrankShaft ParentCrankShaft { get; set; }

        public CTANGirderPage()
        {
            InitializeComponent();
            //拖动+缩放
            canvasTransformGroup.Children.Add(canvasScaleTransform);
            canvasTransformGroup.Children.Add(canvasTranslateTransform);
            Pcanvas.RenderTransform = canvasTransformGroup;

            annotationCanvas.IsHitTestVisible = false; //禁用 dimensionCanvas 上的所有鼠标事件（例如点击、鼠标悬停等）
            Pcanvas.Children.Add(annotationCanvas);
            // 初始化居中去抖逻辑
            InitCentering();
        }
        //private void Psurebutton_Click(object sender, RoutedEventArgs e)//测试角接触球轴承用，后续删除
        //{
        //    DrawBearing bearing1 = new DrawBearing();
        //    bearing1.DefineFromArray(new object[] { "角接触球轴承", 1, 200, 100, 200, 50, 15});
        //    bearing1.BasicDrawing(Pcanvas);
        //}
        //插入轴段
        private void Psurebutton_Click(object sender, RoutedEventArgs e)
        {
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
                if (currentShaft == null)
                {
                    currentShaft = new Shaft(shaftIdCounter++);
                }
                //if (isCreatingNewShaft || currentShaft == null)
                //{
                //    currentShaft = new Shaft(shaftIdCounter++);
                //    isCreatingNewShaft = false;
                //}
                // 添加轴段到当前 Shaft
                // 创建轴段数据数组
                object[] axisData = new object[]
                {
                    axisLength,           // 0:  BaseWidth
                    innerDiameter,        // 1: BaseInnerDiameter
                    outerDiameter,        // 2: BaseOuterDiameter
                    isInnerTapered,       // 3: IsInnerTapered
                    isOuterTapered,       // 4: IsOuterTapered
                    smallEndInnerDiameter,// 5: SmallEndInnerDiameter
                    smallEndOuterDiameter,// 6: SmallEndOuterDiameter
                    isPdirectioninnerL,   // 7: IsDirectionInnerLChecked
                    isPdirectioninnerR,   // 8: IsDirectionInnerRChecked
                    isPdirectionouterL,   // 9: IsDirectionOuterLChecked
                    isPdirectionouterR,   // 10: IsDirectionOuterRChecked
                    isPeccentricshaft,    // 11: IsPeccentricshaft
                    eccentricDiameter,    // 12: EccentricDiameter
                    phase,                // 13: Phase
                    sectionType           // 14: SectionType
                };

                // 确保有当前轴
                if (currentShaft == null)
                {
                    currentShaft = new Shaft(shaftIdCounter++);
                    allShafts.Add(currentShaft);
                }

                // 创建轴段并添加到原始轴段列表
                DrawAxisDimension newSection = new DrawAxisDimension();
                newSection.DefineFromArray(axisData);
                currentShaft.AddOriginalAxisSection(newSection);

                // 重建组件字典
                currentShaft.RebuildComponentDict();

                // 重新绘制
                currentShaft.DrawShaftWithComponents(annotationCanvas, Pcanvas);
                ScheduleCenter(resetScale: false);
            }
            else
            {
                MessageBox.Show("请输入有效的尺寸！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Draw3D()
        {
            if (currentShaft?.AxisSections != null)
            {
                ParentCrankShaft.m3DTestPage.AxisDimensions = currentShaft.AxisSections;
            }
            ParentCrankShaft.m3DTestPage.InitializeVtk(); //先初始化，可避免未点击“3D模型”界面出现“渲染未完成等错误”
            ParentCrankShaft.m3DTestPage.UpdateModelButton_Click();
        }
        //完成轴绘制
        private void Pfinish_Click(object sender, RoutedEventArgs e)
        {
            if (currentShaft != null)
            {
                allShafts.Add(currentShaft);
                currentShaft = null;
                //isCreatingNewShaft = true;
            }
        }
        //画布的右键触发事件
        private void Pcanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            Point clickPosition = e.GetPosition(Pcanvas);

            UIElement targetShape = null;
            Shaft.ComponentKey? selectedKey = null;

            // 遍历子元素，寻找鼠标命中的多边形（支持 AxisTagWithParam）
            foreach (var child in Pcanvas.Children)
            {
                if (child is Polygon polygon)
                {
                    Shaft.ComponentKey? key = polygon.Tag switch
                    {
                        Shaft.ComponentKey ck => ck,
                        AxisTagWithParam param => param.Key,
                        _ => (Shaft.ComponentKey?)null
                    };


                    if (key != null)
                    {
                        if (IsPointInsidePolygon(polygon, clickPosition))
                        {
                            selectedKey = key;
                            targetShape = polygon;
                            break;
                        }
                        if (highlightedPolygon?.Tag is AxisTagWithParam hParam)
                        {
                            selectedKey = hParam.Key;
                            targetShape = polygon;
                            break;
                        }
                        else if (highlightedPolygon?.Tag is Shaft.ComponentKey hKey)
                        {
                            selectedKey = hKey;
                            targetShape = polygon;
                            break;
                        }
                    }
                }
                else if (child is Canvas mcanvas && mcanvas.Tag is Shaft.ComponentKey tagKey)
                {
                    Shaft.ComponentKey? key = null;
                    key = tagKey;
                    if (IsPointInsideCanvas(mcanvas, clickPosition))
                    {
                        selectedKey = key;
                        targetShape = mcanvas;
                        break;
                    }
                }
            }
            // 如果没找到，默认使用最后一个可识别多边形
            if (selectedKey == null)
            {
                foreach (var child in Pcanvas.Children)
                {
                    if (child is Polygon polygon)
                    {
                        Shaft.ComponentKey? key = polygon.Tag switch
                        {
                            Shaft.ComponentKey ck => ck,
                            AxisTagWithParam param => param.Key,
                            _ => (Shaft.ComponentKey?)null
                        };
                        if (key != null)
                        {
                            selectedKey = key;
                            targetShape = polygon;
                        }
                    }
                }
            }

            // 根据找到的多边形构建右键菜单
            if (targetShape != null && selectedKey != null)
            {
                var key = selectedKey.Value;

                //if (key.Type == PolygonType.Axis)
                //{
                //    var editMenu = new MenuItem { Header = "修改轴段" };
                //    editMenu.Click += (s, e) => EditBeam(key.Index);
                //    contextMenu.Items.Add(editMenu);
                //}
                MenuItem addGearMenu = new MenuItem { Header = "添加齿轮" };
                addGearMenu.Click += (s, e) => AddGearBeam();
                contextMenu.Items.Add(addGearMenu);

                MenuItem addBearingMenu = new MenuItem { Header = "添加轴承" };
                addBearingMenu.Click += (s, e) => AddBearing();
                contextMenu.Items.Add(addBearingMenu);

                //MenuItem addCycloidWheel = new MenuItem { Header = "添加摆线轮" };
                //addCycloidWheel.Click += (s, e) => AddCycloidWheel();
                //contextMenu.Items.Add(addCycloidWheel);

                MenuItem deleteMenu = new MenuItem { Header = "删除" };
                deleteMenu.Click += (s, e) => DeleteComponent();
                contextMenu.Items.Add(deleteMenu);

                //MenuItem testMenu = new MenuItem { Header = "测试" };
                //testMenu.Click += (s, e) => Test();
                //contextMenu.Items.Add(testMenu);

                //MenuItem temAssembly = new MenuItem { Header = "临时装配" };
                //temAssembly.Click += (s, e) => TemAssembly();
                //contextMenu.Items.Add(temAssembly);
            }
            else
            {
                MenuItem defaultMenu = new MenuItem { Header = "不可操作" };
                contextMenu.Items.Add(defaultMenu);
            }

            contextMenu.IsOpen = true;
        }
        //画布的左键点击事件
        // 鼠标按下：进入用户平移模式（保持原点选中逻辑），保留原有选择逻辑
        private void Pcanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 开始用户平移（以屏幕/窗口坐标作为基准）
            isUserPanning = true;
            isDragging = true;
            lastMousePositionOnScreen = e.GetPosition(this);
            Pcanvas.CaptureMouse();

            // 保留原来的点击选中逻辑（使用逆变换判断）
            Point point = e.GetPosition(Pcanvas);
            bool found = false;
            foreach (var child in Pcanvas.Children)
            {
                if (child is Polygon polygon)
                {
                    Shaft.ComponentKey? key = null;
                    if (polygon.Tag is Shaft.ComponentKey tagKey) key = tagKey;
                    else if (polygon.Tag is AxisTagWithParam param) key = param.Key;

                    if (key != null && IsPointInsidePolygon(polygon, point))
                    {
                        ClearHighlight();
                        polygon.Stroke = Brushes.Red;
                        polygon.StrokeThickness = 0.2;
                        highlightedPolygon = polygon;
                        selectedComponentKey = key.Value;
                        found = true;
                        break;
                    }
                }
                else if (child is Canvas mcanvas && mcanvas.Tag is Shaft.ComponentKey tagKey)
                {
                    Shaft.ComponentKey? key = null;
                    key = tagKey;
                    if (IsPointInsideCanvas(mcanvas, point))
                    {
                        ClearHighlight();
                        var highlightLayer = mcanvas.Children
                            .OfType<Rectangle>()
                            .FirstOrDefault(r => r.Name == "HighlightLayer");
                        if (highlightLayer != null)
                            highlightLayer.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                        highlightedCanvas = mcanvas;
                        selectedComponentKey = key.Value;
                        found = true;
                        break;
                    }
                }
            }
            if (!found) ClearHighlight();
        }
        //画布的滚轮缩放事件
        private void Pcanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scaleFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;

            double newScale = canvasScaleTransform.ScaleX * scaleFactor;

            if (newScale < MinScale || newScale > MaxScale)
                return;
            canvasScaleTransform.ScaleX *= scaleFactor;
            canvasScaleTransform.ScaleY *= scaleFactor;
        }
        //private void Pcanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    double scaleFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;
        //    double currentScale = canvasScaleTransform.ScaleX;
        //    double newScale = currentScale * scaleFactor;
        //    if (newScale < MinScale || newScale > MaxScale) return;

        //    // 获取鼠标在 Pcanvas 的位置（视图坐标），并转换到内容坐标
        //    Point mouseOnPcanvas = e.GetPosition(Pcanvas);
        //    Point contentMouse = TransformPointToContent(mouseOnPcanvas);

        //    // 先更新 scale
        //    canvasScaleTransform.ScaleX = newScale;
        //    canvasScaleTransform.ScaleY = newScale;

        //    // 为了缩放以鼠标为中心，调整 translate 使 contentMouse 仍映射到相同屏幕位置
        //    // 新的 translate = mouseScreenPos - newScale * contentMouse
        //    // mouseScreenPos 在 Pcanvas 坐标系就是 mouseOnPcanvas
        //    canvasTranslateTransform.X = mouseOnPcanvas.X - newScale * contentMouse.X;
        //    canvasTranslateTransform.Y = mouseOnPcanvas.Y - newScale * contentMouse.Y;

        //    // 缩放时不立即自动居中（避免覆盖用户意图），如果需要可 ScheduleCenter(false) 由用户触发或外部事件处理
        //}
        //鼠标拖动事件
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging && !isUserPanning) return;

            // 如果在用户拖拽期间，使用屏幕坐标进行 delta 计算并更新 Translate（与原先逻辑一致）
            if (isUserPanning || isDragging)
            {
                Point current = e.GetPosition(this);
                Vector delta = current - lastMousePositionOnScreen;

                // 应用平移（累加）
                canvasTranslateTransform.X += delta.X;
                canvasTranslateTransform.Y += delta.Y;

                lastMousePositionOnScreen = current;
            }
        }
        // 鼠标抬起：结束拖拽，不再主动发起居中（去掉原来在这里调用 ScheduleCenter 的行为）
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 结束用户拖拽交互
            isDragging = false;

            if (isUserPanning)
            {
                isUserPanning = false;
                Pcanvas.ReleaseMouseCapture();
                // 注意：不再在这里调用 ScheduleCenter(...)，以保持用户拖拽结束后的当前位置
                // 若需要在拖拽完后仍自动居中（不推荐），可以手动调用： ScheduleCenter(false);
                if (centerDebounceTimer != null && centerDebounceTimer.IsEnabled)
                    centerDebounceTimer.Stop();
                pendingResetScale = false;
            }
        }
        //重置画布视图
        public void InitCentering()
        {
            if (centerDebounceTimer == null)
            {
                centerDebounceTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(120)
                };
                centerDebounceTimer.Tick += (s, e) =>
                {
                    // 如果用户正在交互（拖拽），不要重试并直接停止定时器，避免在用户放手后仍然触发居中
                    if (isUserPanning)
                    {
                        centerDebounceTimer.Stop();
                        // 不重启，也不立即居中，等待下一次显式请求或 SizeChanged
                        return;
                    }

                    centerDebounceTimer.Stop();
                    Dispatcher.BeginInvoke(new Action(() => CenterContent(pendingResetScale)), DispatcherPriority.Loaded);
                };

                Pcanvas.SizeChanged += Pcanvas_SizeChanged;
            }
            else
            {
                // 已初始化：把这次调用当作“请求居中”（不重置缩放）
                ScheduleCenter(resetScale: false);
            }
        }
        private void Pcanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!isUserPanning)
            {
                ScheduleCenter(resetScale: false);
            }
        }

        private void ScheduleCenter(bool resetScale)
        {
            if (resetScale) pendingResetScale = true;
            centerDebounceTimer.Stop();
            centerDebounceTimer.Start();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // 异步去抖执行：重置缩放并居中
            ScheduleCenter(resetScale: true);
        }

        // 通用居中函数：resetScale=true 表示先把缩放复位为 1 再居中；false 表示保持当前缩放，仅居中
        public void CenterContent(bool resetScale = false)
        {
            // 如果用户正在交互，则不自动居中
            if (isUserPanning) return;

            if (isCentering) return;
            isCentering = true;

            try
            {
                // 等待布局完成（如果尺寸还没准备好，延后执行）
                if (Pcanvas.ActualWidth <= 0 || Pcanvas.ActualHeight <= 0)
                {
                    Dispatcher.BeginInvoke((Action)(() => CenterContent(resetScale)), DispatcherPriority.Loaded);
                    return;
                }

                // 根据请求复位缩放
                if (resetScale)
                {
                    // 先将 scale 复位为 1
                    canvasScaleTransform.ScaleX = 1.0;
                    canvasScaleTransform.ScaleY = 1.0;
                }

                double s = canvasScaleTransform.ScaleX;
                if (s <= 0) s = 1.0;

                // 确保布局刷新（避免 bounds 读取不完整）
                try { Pcanvas.UpdateLayout(); } catch { }

                // 使用 VisualTreeHelper 读取 Pcanvas 子元素的内容边界（内容坐标系）
                Rect contentBounds = VisualTreeHelper.GetDescendantBounds(Pcanvas);
                if (contentBounds.IsEmpty)
                {
                    // 没有内容则复位平移
                    canvasTranslateTransform.X = 0;
                    canvasTranslateTransform.Y = 0;
                    return;
                }

                // 使用“内容中心映射到视口中心”的公式：viewportCenter = s * contentCenter + translate
                double contentCenterX = contentBounds.X + contentBounds.Width / 2.0;
                double contentCenterY = contentBounds.Y + contentBounds.Height / 2.0;

                double viewportCenterX = Pcanvas.ActualWidth / 2.0;
                double viewportCenterY = Pcanvas.ActualHeight / 2.0;

                double targetTranslateX = viewportCenterX - s * contentCenterX;
                double targetTranslateY = viewportCenterY - s * contentCenterY;

                // 只有当用户不在拖拽时才覆盖 translate（额外保险）
                if (!isUserPanning)
                {
                    canvasTranslateTransform.X = targetTranslateX;
                    canvasTranslateTransform.Y = targetTranslateY;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CenterContent 出错: {ex.Message}");
            }
            finally
            {
                isCentering = false;
                pendingResetScale = false;
            }
        }
        //修改功能（待实现）
        private void EditBeam(int index)
        {
            //// 检查索引是否合法
            //if (index < 0 || index >= axisDimensions.Count)
            //{
            //    MessageBox.Show("无效的索引，无法编辑！");
            //    return;
            //}
            //// 获取一个对象
            //var dimensions = axisDimensions[index];
            //List<AxisDimensions> listdimensions = new List<AxisDimensions> { dimensions };
            //EditWindow editWindow = new EditWindow(listdimensions);
            //// 显示编辑窗口并处理更新
            //if (editWindow.ShowDialog() == true)
            //{
            //    // 从 editWindow 中获取更新后的 AxisDimensions 对象并更新 
            //    axisDimensions[index] = editWindow.UpdatedDimensions[0];
            //    stopAdd = true;//禁用Psurebutton_Click中部分代码
            //    Psurebutton_Click(null, null); // 重新绘制
            //    stopAdd = false;//恢复默认状态
            //}
        }
        //右键菜单删除功能实现
        private void DeleteComponent()
        {
            if (selectedComponentKey == null || currentShaft == null) return;

            var key = selectedComponentKey.Value;

            string componentName = GetComponentTypeName(key.Type);
            // 显示确认对话框
            MessageBoxResult result = MessageBox.Show(
                $"确定要删除 {componentName} {key.Index + 1} 吗？\n\n此操作不可撤销。",
                "确认删除",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            // 如果用户选择"否"，则取消删除
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            var sys = ParentCrankShaft.ParentCrankShaftSystem;

            switch (key.Type)
            {
                case PolygonType.Axis:
                    // 获取拆分轴段对应的原始轴段索引
                    int originalIndex = currentShaft.GetOriginalIndexFromSplitIndex(key.Index);
                    if (originalIndex >= 0)
                    {
                        currentShaft.RemoveOriginalAxisSection(originalIndex);
                    }
                    break;

                case PolygonType.Bearing:
                    if (key.Index >= 0 && key.Index < currentShaft.Bearings.Count)
                    {                
                        var selectedDrawBearing = currentShaft.Bearings[key.Index];//获取即将被删除的前端图形对象

                        currentShaft.Bearings.RemoveAt(key.Index);//从前端数据中移除

                        if (ParentCrankShaft?.ParentCrankShaftSystem != null)
                        {
                            

                            // 根据绑定关系，找到对应的后端逻辑轴承
                            var backendBearing = sys.mBearingList.FirstOrDefault(b => b.CanvasGraphic == selectedDrawBearing);
                            if (backendBearing != null)
                            {
                                // 从左侧树状视图(TreeView)中移除
                                sys.mTreeViewItem.Items.Remove(backendBearing.mTreeViewItem);

                                // 从后端列表中彻底移除
                                sys.mBearingList.Remove(backendBearing);
                                sys.mBearingCount--;

                                // 从全局页面字典中移除，防止脏数据残留和内存泄漏
                                MainWindow.sPageIDDict.Remove(backendBearing.mID);
                            }
                        }
                        // 重新编号
                        for (int i = 0; i < currentShaft.Bearings.Count; i++)
                        {
                            currentShaft.Bearings[i].BaseIndex = i;
                        }
                    }
                    break;

                case PolygonType.Gear:
                    if (key.Index >= 0 && key.Index < currentShaft.Gears.Count)
                    {
                        var gearToDelete = currentShaft.Gears[key.Index];
                        currentShaft.Gears.RemoveAt(key.Index);

                        // 删除齿轮的后端逻辑节点
                        var logicGear = sys.mCrankShaftSystemGearList.FirstOrDefault(g => g.CanvasGraphic == gearToDelete);
                        if (logicGear != null)
                        {
                            sys.mTreeViewItem.Items.Remove(logicGear.mTreeViewItem);
                            sys.mCrankShaftSystemGearList.Remove(logicGear);
                            MainWindow.sPageIDDict.Remove(logicGear.mID);
                        }
                        // 重新编号
                        for (int i = 0; i < currentShaft.Gears.Count; i++)
                        {
                            currentShaft.Gears[i].BaseIndex = i;
                        }
                    }
                    break;

                case PolygonType.CycloidWheel:
                    if (key.Index >= 0 && key.Index < currentShaft.CycloidWheels.Count)
                    {
                        var cycToDelete = currentShaft.CycloidWheels[key.Index];
                        currentShaft.CycloidWheels.RemoveAt(key.Index);

                        // 删除摆线轮的后端逻辑节点
                        var logicCyc = sys.mCrankShaftSystemCycloidPinionList.FirstOrDefault(c => c.CanvasGraphic == cycToDelete);
                        if (logicCyc != null)
                        {
                            sys.mTreeViewItem.Items.Remove(logicCyc.mTreeViewItem);
                            sys.mCrankShaftSystemCycloidPinionList.Remove(logicCyc);
                            MainWindow.sPageIDDict.Remove(logicCyc.mID);

                            // 同时移除该摆线轮附带的滚针轴承逻辑节点
                            string brgName = logicCyc.mTreeViewItem.Header.ToString() + "滚针轴承";
                            var logicBrg = sys.mCrankShaftSystemSlewingBearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == brgName);
                            if (logicBrg != null)
                            {
                                var brgGraphic = currentShaft.Bearings.FirstOrDefault(b =>
                                    b.BearingType == "滚针轴承" &&
                                    Math.Abs(b.YDirectionOffset - cycToDelete.YDirectionOffset) < 0.001); //寻找与当前摆线轮同偏移的滚针轴承

                                if (brgGraphic != null)
                                {
                                    // 从轴承列表中移除图形数据
                                    currentShaft.Bearings.Remove(brgGraphic);

                                    // 从组件字典中移除绑定的键值，防止后续点击报错
                                    var dictEntry = Shaft.componentDict.FirstOrDefault(x => x.Value == brgGraphic);
                                    if (dictEntry.Key != null)
                                    {
                                        Shaft.componentDict.Remove(dictEntry.Key);
                                    }
                                }
                                sys.mCrankShaftSystemSlewingBearingList.Remove(logicBrg);
                                MainWindow.sPageIDDict.Remove(logicBrg.mID);
                            }
                        }

                        // 重新编号
                        for (int i = 0; i < currentShaft.CycloidWheels.Count; i++)
                        {
                            currentShaft.CycloidWheels[i].BaseIndex = i;
                        }
                        // 重新编号
                        for (int i = 0; i < currentShaft.Bearings.Count; i++)
                        {
                            currentShaft.Bearings[i].BaseIndex = i;
                        }
                    }
                    break;
            }

            // 重建组件字典
            currentShaft.RebuildComponentDict();

            // 清除选中状态
            selectedComponentKey = null;
            highlightedPolygon = null;
            highlightedCanvas = null;

            // 重新绘制
            currentShaft.DrawShaftWithComponents(annotationCanvas, Pcanvas);

            //MessageBox.Show($"{componentName}已成功删除。", "删除成功", MessageBoxButton.OK, MessageBoxImage. Information);
        }
        /// <summary>
        /// 根据组件类型获取中文名称
        /// </summary>
        private string GetComponentTypeName(PolygonType type)
        {
            return type switch
            {
                PolygonType.Axis => "轴段",
                PolygonType.Bearing => "轴承",
                PolygonType.Gear => "齿轮",
                PolygonType.CycloidWheel => "摆线轮",
                _ => "组件"
            };
        }
        //绘图更新ALL
        private void UpdateDrawing()
        {
            currentShaft.DrawShaftWithComponents(annotationCanvas, Pcanvas);
            ScheduleCenter(resetScale: false);
        }
        //右键齿轮功能实现
        private void AddGearBeam()
        {
            //设定构件字典
            Dictionary<string, int> TmpDict = new Dictionary<string, int>();
            ParentCrankShaft.mBelongedSet.mInvGearPair.mGears[0].ExportPageToStringIDDict(TmpDict);
            ParentCrankShaft.mBelongedSet.mInvGearPair.mGears[1].ExportPageToStringIDDict(TmpDict);
            ParentCrankShaft.mBelongedSet.mCycloidGearPair.mCycloidPinionList[0].ExportPageToStringIDDict(TmpDict);
            ParentCrankShaft.mBelongedSet.mCycloidGearPair.mCycloidPinionList[1].ExportPageToStringIDDict(TmpDict);

            UITANInsertSymmetryComponent tmpUISymmetryComponentInsert = new UITANInsertSymmetryComponent();
            tmpUISymmetryComponentInsert.SetComponentListBox(TmpDict);
            tmpUISymmetryComponentInsert.AssignedShaftSystem = ParentCrankShaft.ParentCrankShaftSystem;
            tmpUISymmetryComponentInsert.ShowDialog();
            if (tmpUISymmetryComponentInsert.IsAssignComponent)
            {
                var sys = ParentCrankShaft.ParentCrankShaftSystem; // 获取曲轴系引用便于后续添加树视图
                int selectedIdx = tmpUISymmetryComponentInsert.mComponentListComboBox.SelectedIndex;

                if (tmpUISymmetryComponentInsert.mComponentListComboBox.Text == "太阳轮" ||
                    tmpUISymmetryComponentInsert.mComponentListComboBox.Text == "行星轮")
                {
                    DrawGear gear = new DrawGear();
                    gear.DefineFromArray(new object[] {
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnGeometryDiameter(),
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnGeometryWidth(),
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleDistance(),
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnYDirectionOffset(),
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleFaceType()
                    });

                    currentShaft.Gears.Add(gear);
                    gear.BaseIndex = currentShaft.Gears.Count - 1; //确保轴段索引和键对应
                    gear.BasicDrawing(Pcanvas);
                    Shaft.componentDict[new ComponentKey(PolygonType.Gear, gear.BaseIndex)] = gear;// 必须在BasicDrawing之后，否则BaseIndex不正确

                    // ===== 2. 在曲轴系新建逻辑节点并在树视图显示（复制数据） =====
                    string gearName = tmpUISymmetryComponentInsert.mComponentListComboBox.Text == "太阳轮" ? "曲轴太阳轮" : "曲轴行星轮";
                    CTANInvGear logicGear = new CTANInvGear();
                    logicGear.mTreeViewItem.Header = gearName;
                    logicGear.mTreeViewItem.Name = gearName;
                    logicGear.CanvasGraphic = gear; // 绑定前端图形引用

                    // 复制源齿轮的数据（深拷贝，不互相干扰也不破坏原有的树视图）
                    var sourceGear = ParentCrankShaft.mBelongedSet.mInvGearPair.mGears[selectedIdx];
                    foreach (var key in sourceGear.mDataStorageDict.Keys)
                    {
                        if (logicGear.mDataStorageDict.ContainsKey(key))
                            logicGear.mDataStorageDict[key].Value = sourceGear.mDataStorageDict[key].Value;
                        else
                            logicGear.mDataStorageDict.Add(key, new CTANDouble() { Value = sourceGear.mDataStorageDict[key].Value });
                    }

                    MainWindow.sPageIDDict[logicGear.mID] = logicGear;
                    sys.mCrankShaftSystemGearList.Add(logicGear);
                    sys.mTreeViewItem.Items.Add(logicGear.mTreeViewItem);
                   
                }
            
                else //摆线轮（绘制的同时添加滚针轴承！）
                {
                    int inSelectedIndex = tmpUISymmetryComponentInsert.mComponentListComboBox.SelectedIndex - 2;
                    double yOffset = tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnYDirectionOffset();

                    var slewingBearingLogic = ParentCrankShaft.mBelongedSet.mCycloidGearPair.mSlewingBearingList[inSelectedIndex];

                    //更新滚针轴承的定位参数
                    switch (tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleFaceType())
                    {
                        case 1:
                            slewingBearingLogic.mDataStorageDict["轴承定位面"].Value = 1;
                            slewingBearingLogic.mDataStorageDict["轴承定位面距基准面距离"].Value =
                                tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleDistance() +
                                tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnGeometryWidth() / 2 -
                                slewingBearingLogic.mDataStorageDict["轴承宽度Bc"].Value / 2;
                            break;
                        case 0:
                            slewingBearingLogic.mDataStorageDict["轴承定位面"].Value = 0;
                            slewingBearingLogic.mDataStorageDict["轴承定位面距基准面距离"].Value =
                                tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleDistance();
                            break;
                        case -1:
                            slewingBearingLogic.mDataStorageDict["轴承定位面"].Value = -1;
                            slewingBearingLogic.mDataStorageDict["轴承定位面距基准面距离"].Value =
                                tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleDistance() -
                                tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnGeometryWidth() / 2 +
                                slewingBearingLogic.mDataStorageDict["轴承宽度Bc"].Value / 2;
                            break;
                    }

                    //添加滚针轴承
                    DrawBearing bearing = new DrawBearing();
                    bearing.YDirectionOffset = yOffset;  // 设置Y偏移
                    bearing.DefineFromArray(new object[] {
                        "滚针轴承",
                        slewingBearingLogic.mDataStorageDict["轴承定位面"].Value,
                        slewingBearingLogic.mDataStorageDict["轴承定位面距基准面距离"].Value,
                        slewingBearingLogic.mDataStorageDict["轴承内径Fw"].Value,
                        slewingBearingLogic.mDataStorageDict["轴承外径Ew"].Value,
                        slewingBearingLogic.mDataStorageDict["轴承宽度Bc"].Value,
                    });
                    currentShaft.Bearings.Add(bearing);
                    bearing.BaseIndex = currentShaft.Bearings.Count - 1;
                    bearing.BasicDrawing(Pcanvas);
                    Shaft.componentDict[new ComponentKey(PolygonType.Bearing, bearing.BaseIndex)] = bearing;

                    double tmpCycloidWhellOutDiameter = slewingBearingLogic.mDataStorageDict["轴承外径Ew"].Value;
                    if (tmpCycloidWhellOutDiameter == 0) tmpCycloidWhellOutDiameter = ParentCrankShaft.mBelongedSet.mCycloidGearPair.mGear.mDataStorageDict["针轮分度圆半径"].Value;

                    // ===== 3. 绘制摆线轮前端图形 =====
                    DrawCycloidWheel cycloidWheel = new DrawCycloidWheel();
                    cycloidWheel.DefineFromArray(new object[] {
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnGeometryDiameter(),
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnGeometryWidth(),
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleDistance(),
                        yOffset,
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleFaceType(),
                        tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnCycloidGearRf(),
                        tmpCycloidWhellOutDiameter
                    });

                    currentShaft.CycloidWheels.Add(cycloidWheel);
                    cycloidWheel.BaseIndex = currentShaft.CycloidWheels.Count - 1;
                    cycloidWheel.BasicDrawing(Pcanvas);
                    Shaft.componentDict[new ComponentKey(PolygonType.CycloidWheel, cycloidWheel.BaseIndex)] = cycloidWheel;

                    // ===== 4. 在曲轴系新建摆线轮逻辑节点并在树视图显示 =====
                    string cycName = inSelectedIndex == 0 ? "左侧摆线轮" : "右侧摆线轮";
                    CTANCycloidPinion logicCyc = new CTANCycloidPinion();
                    logicCyc.mTreeViewItem.Header = cycName;
                    logicCyc.mTreeViewItem.Name = cycName;
                    logicCyc.CanvasGraphic = cycloidWheel; // 绑定前端图形引用

                    // 复制源摆线轮数据
                    var sourceCyc = ParentCrankShaft.mBelongedSet.mCycloidGearPair.mCycloidPinionList[inSelectedIndex];
                    foreach (var key in sourceCyc.mDataStorageDict.Keys)
                    {
                        if (logicCyc.mDataStorageDict.ContainsKey(key))
                            logicCyc.mDataStorageDict[key].Value = sourceCyc.mDataStorageDict[key].Value;
                        else
                            logicCyc.mDataStorageDict.Add(key, new CTANDouble() { Value = sourceCyc.mDataStorageDict[key].Value });
                    }

                    MainWindow.sPageIDDict[logicCyc.mID] = logicCyc;
                    sys.mCrankShaftSystemCycloidPinionList.Add(logicCyc);
                    sys.mTreeViewItem.Items.Add(logicCyc.mTreeViewItem);

                    // ===== 5. 在曲轴系新建滚针轴承逻辑节点并在树视图显示 =====
                    string brgName = cycName + "滚针轴承";
                    CTANSlewingBearing logicBrg = new CTANSlewingBearing();
                    logicBrg.mTreeViewItem.Header = brgName;
                    logicBrg.mTreeViewItem.Name = brgName;
                    logicBrg.mBelongedSet = ParentCrankShaft.mBelongedSet;
                    logicBrg.CanvasGraphic = bearing; // 绑定前端图形引用

                    // 复制源滚针轴承数据
                    var sourceBrg = ParentCrankShaft.mBelongedSet.mCycloidGearPair.mSlewingBearingList[inSelectedIndex];
                    foreach (var key in sourceBrg.mDataStorageDict.Keys)
                    {
                        if (logicBrg.mDataStorageDict.ContainsKey(key))
                            logicBrg.mDataStorageDict[key].Value = sourceBrg.mDataStorageDict[key].Value;
                        else
                            logicBrg.mDataStorageDict.Add(key, new CTANDouble() { Value = sourceBrg.mDataStorageDict[key].Value });
                    }

                    MainWindow.sPageIDDict[logicBrg.mID] = logicBrg;
                    sys.mCrankShaftSystemSlewingBearingList.Add(logicBrg);

                    // 将滚针轴承作为摆线轮的子节点添加到树视图
                    logicCyc.mTreeViewItem.Items.Add(logicBrg.mTreeViewItem);

                    // 重绘整个轴系统以更新被覆盖轴段的偏移
                    currentShaft.DrawShaftWithComponents(annotationCanvas, Pcanvas);
                }
            }
        }
        private void AddBearing()
        {
            Dictionary<string, int> TmpDict = new Dictionary<string, int>();
            for (int i = 0; i < ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingCount; i++)
            {
                ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingList[i].ExportPageToStringIDDict(TmpDict);
            }
            UITANInsertSymmetryComponent tmpUISymmetryComponentInsert = new UITANInsertSymmetryComponent();
            tmpUISymmetryComponentInsert.SetComponentListBox(TmpDict);
            tmpUISymmetryComponentInsert.AssignedShaftSystem = ParentCrankShaft.ParentCrankShaftSystem;
            tmpUISymmetryComponentInsert.ShowDialog();
            if (tmpUISymmetryComponentInsert.IsAssignComponent)
            {
                DrawBearing newDrawBearing = new DrawBearing();
                CTANBearing newCTANBearing = new CTANBearing();
                newDrawBearing.DefineFromArray(new object[] {tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnBearingType(),
                    tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleFaceType(),
                    tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnAssembleDistance(),
                    tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnBearingInnerDiameter(), //内径
                    tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnGeometryDiameter(),
                    tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnGeometryWidth(),       //||T
                    tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnTaperedBearingB(),     //B
                    tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnTaperedBearingC(),     //C
                    tmpUISymmetryComponentInsert.mAssignComponentPage.ReturnTaperedDirection()});     //轴承朝向

                newCTANBearing.CanvasGraphic = newDrawBearing;
                ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingList.Add(newCTANBearing);

                currentShaft.Bearings.Add(newDrawBearing);
                newDrawBearing.BaseIndex = currentShaft.Bearings.Count - 1; //确保轴段索引和键对应
                newDrawBearing.BasicDrawing(Pcanvas);
                Shaft.componentDict[new ComponentKey(PolygonType.Bearing, newDrawBearing.BaseIndex)] = newDrawBearing;
            }
        }
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
        public void Assembly(double[] InputParams)
        {
            if (InputParams[0] == -1)
            {
                MessageBox.Show("未找到合适的圆锥滚子轴承！");
                return;
            }
            if (InputParams[0] == -2)
            {
                MessageBox.Show("未找到合适的滚针轴承,请增加摆线轮宽度后再尝试！");
                return;
            }

            if (currentShaft == null)
            {
                currentShaft = new Shaft(shaftIdCounter++);
            }
            //每次推荐都先清除之前的图形和列表、字典里面的数据
            annotationCanvas.Children.Clear();
            Pcanvas.Children.Clear();
            Shaft.componentDict.Clear();
            currentShaft.AxisSections.Clear();
            currentShaft.Bearings.Clear();
            currentShaft.CycloidWheels.Clear();
            currentShaft.Gears.Clear();

            currentShaft.OriginalAxisSections.Clear();

            double totalLength = InputParams[50] + InputParams[51] + InputParams[52] + InputParams[53] + InputParams[54];

            // 创建一根通轴（原始轴段）
            var axisSection = new DrawAxisDimension(new object[] {
                totalLength,  // 总长度
                0,            // 内径
                InputParams[0], // 外径
                false, false, 0, 0, false, false, false, false, false, 0, 0, 1  // 非偏心轴
            });
            // 使用 AddOriginalAxisSection 方法添加原始轴段
            currentShaft.AddOriginalAxisSection(axisSection);
            axisSection.BaseIndex = currentShaft.OriginalAxisSections.Count - 1;
 
            //轴段参数38，25；15，50，10，70；15，50，10，110；20，25
            DrawBearing needleRoller1 = new DrawBearing();
            needleRoller1.YDirectionOffset = InputParams[60];  // 使用摆线轮1的Y偏移
            needleRoller1.DefineFromArray(new object[] { "滚针轴承", InputParams[18], InputParams[19], InputParams[20], InputParams[21], InputParams[22] });
            currentShaft.Bearings.Add(needleRoller1);
            needleRoller1.BaseIndex = currentShaft.Bearings.Count - 1; //确保轴段索引和键对应
            Shaft.componentDict[new ComponentKey(PolygonType.Bearing, needleRoller1.BaseIndex)] = needleRoller1;

            DrawBearing needleRoller2 = new DrawBearing();
            needleRoller2.YDirectionOffset = -InputParams[60];  // 使用摆线轮2的Y偏移
            needleRoller2.DefineFromArray(new object[] { "滚针轴承", InputParams[23], InputParams[24], InputParams[25], InputParams[26], InputParams[27] });
            currentShaft.Bearings.Add(needleRoller2);
            needleRoller2.BaseIndex = currentShaft.Bearings.Count - 1; //确保轴段索引和键对应
            Shaft.componentDict[new ComponentKey(PolygonType.Bearing, needleRoller2.BaseIndex)] = needleRoller2;

            DrawBearing bearing1 = new DrawBearing();
            bearing1.DefineFromArray(new object[] { "圆锥滚子轴承", InputParams[2], InputParams[3], InputParams[4], InputParams[5], InputParams[6], InputParams[7], InputParams[8], InputParams[9] });
            currentShaft.Bearings.Add(bearing1);
            bearing1.BaseIndex = currentShaft.Bearings.Count - 1; //确保轴段索引和键对应
            Shaft.componentDict[new ComponentKey(PolygonType.Bearing, bearing1.BaseIndex)] = bearing1;

            DrawBearing bearing2 = new DrawBearing();
            bearing2.DefineFromArray(new object[] { "圆锥滚子轴承", InputParams[10], InputParams[11], InputParams[12], InputParams[13], InputParams[14], InputParams[15], InputParams[16], InputParams[17] });
            currentShaft.Bearings.Add(bearing2);
            bearing2.BaseIndex = currentShaft.Bearings.Count - 1; //确保轴段索引和键对应
            Shaft.componentDict[new ComponentKey(PolygonType.Bearing, bearing2.BaseIndex)] = bearing2;

            DrawCycloidWheel cycloidWheel1 = new DrawCycloidWheel();
            cycloidWheel1.DefineFromArray(new object[] { InputParams[28], InputParams[29], InputParams[30], InputParams[60], InputParams[32], InputParams[90], InputParams[21] });
            currentShaft.CycloidWheels.Add(cycloidWheel1);
            cycloidWheel1.BaseIndex = currentShaft.CycloidWheels.Count - 1; //确保轴段索引和键对应
            Shaft.componentDict[new ComponentKey(PolygonType.CycloidWheel, cycloidWheel1.BaseIndex)] = cycloidWheel1;// 必须在BasicDrawing之后，否则BaseIndex不正确

            DrawCycloidWheel cycloidWheel2 = new DrawCycloidWheel();
            cycloidWheel2.DefineFromArray(new object[] { InputParams[33], InputParams[34], InputParams[35], -InputParams[60], InputParams[37], InputParams[91], InputParams[21] });
            currentShaft.CycloidWheels.Add(cycloidWheel2);
            cycloidWheel2.BaseIndex = currentShaft.CycloidWheels.Count - 1; //确保轴段索引和键对应
            Shaft.componentDict[new ComponentKey(PolygonType.CycloidWheel, cycloidWheel2.BaseIndex)] = cycloidWheel2;

            DrawGear gear1 = new DrawGear();
            gear1.DefineFromArray(new object[] { InputParams[38], InputParams[39], InputParams[40], InputParams[41], InputParams[42] });
            currentShaft.Gears.Add(gear1);
            gear1.BaseIndex = currentShaft.Gears.Count - 1; //确保轴段索引和键对应
            Shaft.componentDict[new ComponentKey(PolygonType.Gear, gear1.BaseIndex)] = gear1;

            if (Pcanvas.ActualWidth == 0)
            {
                RoutedEventHandler handler = null;
                handler = (s, e) =>
                {
                    currentShaft.DrawShaftWithComponents(annotationCanvas, Pcanvas);
                    ScheduleCenter(resetScale: false);
                    Pcanvas.Loaded -= handler; // 只执行一次
                };
                Pcanvas.Loaded += handler;
            }
            else
            {
                currentShaft.DrawShaftWithComponents(annotationCanvas, Pcanvas);
                ScheduleCenter(resetScale: false);
            }


            var treeViewItem = ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mTreeViewItem;
            var bearingList = ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingList;
            // 1. 查找是否已存在“推荐的圆锥滚子轴承”和“推荐的滚针轴承”
            CTANBearing tapered0 = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧圆锥滚子轴承");
            CTANBearing tapered1 = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧圆锥滚子轴承");

            // 2. 如果没有，则设置TreeViewItem
            if (tapered0 == null)
            {
                tapered0 = new CTANBearing();
                tapered0.mTreeViewItem.Header = "左侧圆锥滚子轴承";
                tapered0.mTreeViewItem.Name = "左侧圆锥滚子轴承";
                tapered0.mBelongedSet = ParentCrankShaft.mBelongedSet;
                MainWindow.sPageIDDict[tapered0.mID] = tapered0;
                bearingList.Add(tapered0);
                treeViewItem.Items.Add(tapered0.mTreeViewItem);
                ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingCount++;
            }
            tapered0.CanvasGraphic = bearing1; // 关联图元

            if (tapered1 == null)
            {
                tapered1 = new CTANBearing();
                tapered1.mTreeViewItem.Header = "右侧圆锥滚子轴承";
                tapered1.mTreeViewItem.Name = "右侧圆锥滚子轴承";
                tapered1.mBelongedSet = ParentCrankShaft.mBelongedSet;
                MainWindow.sPageIDDict[tapered1.mID] = tapered1;
                bearingList.Add(tapered1);
                treeViewItem.Items.Add(tapered1.mTreeViewItem);
                ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingCount++;
            }
            tapered1.CanvasGraphic = bearing2;

            // 3. 更新“推荐的圆锥滚子轴承”数据
            tapered0.mDataStorageDict["轴承类型"].Value = CTANBearing.TaperedRollerBearing;
            tapered0.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[70];
            tapered0.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[71];
            tapered0.mDataStorageDict["计算系数e"].Value = InputParams[72];
            tapered0.mDataStorageDict["计算系数Y"].Value = InputParams[73];
            tapered0.mDataStorageDict["计算系数Y0"].Value = InputParams[74];
            tapered0.mDataStorageDict["作用点距离左端面距离a"].Value = InputParams[75];
            tapered0.mDataStorageDict["轴承内径d"].Value = InputParams[4];
            tapered0.mDataStorageDict["轴承外径D"].Value = InputParams[5];
            tapered0.mDataStorageDict["轴承宽度T"].Value = InputParams[6];
            tapered0.mDataStorageDict["轴承宽度B"].Value = InputParams[7];
            tapered0.mDataStorageDict["轴承宽度C"].Value = InputParams[8];

            tapered1.mDataStorageDict["轴承类型"].Value = CTANBearing.TaperedRollerBearing;
            tapered1.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[70];
            tapered1.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[71];
            tapered1.mDataStorageDict["计算系数e"].Value = InputParams[72];
            tapered1.mDataStorageDict["计算系数Y"].Value = InputParams[73];
            tapered1.mDataStorageDict["计算系数Y0"].Value = InputParams[74];
            tapered1.mDataStorageDict["作用点距离左端面距离a"].Value = InputParams[75];
            tapered1.mDataStorageDict["轴承内径d"].Value = InputParams[4];
            tapered1.mDataStorageDict["轴承外径D"].Value = InputParams[5];
            tapered1.mDataStorageDict["轴承宽度T"].Value = InputParams[6];
            tapered1.mDataStorageDict["轴承宽度B"].Value = InputParams[7];
            tapered1.mDataStorageDict["轴承宽度C"].Value = InputParams[8];

            //更新摆线轮下面对应的滚针轴承
            var cycloidWheelBearingsList = ParentCrankShaft.mBelongedSet.mCycloidGearPair.mSlewingBearingList;
            CTANBearing CycloidWheelBearings0 = cycloidWheelBearingsList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "摆线轮0轴承");
            CTANBearing CycloidWheelBearings1 = cycloidWheelBearingsList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "摆线轮1轴承");
            if (CycloidWheelBearings0 != null && CycloidWheelBearings1 != null)
            {
                CycloidWheelBearings0.mDataStorageDict["轴承类型"].Value = CTANBearing.NeedleBearing;
                CycloidWheelBearings0.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[80];
                CycloidWheelBearings0.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[81];
                CycloidWheelBearings0.mDataStorageDict["轴承内径Fw"].Value = InputParams[20];
                CycloidWheelBearings0.mDataStorageDict["轴承外径Ew"].Value = InputParams[21];
                CycloidWheelBearings0.mDataStorageDict["轴承宽度Bc"].Value = InputParams[22];
                CycloidWheelBearings0.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[19];//第一个滚针轴承
                CycloidWheelBearings0.mDataStorageDict["轴承定位面"].Value = InputParams[18];

                CycloidWheelBearings1.mDataStorageDict["轴承类型"].Value = CTANBearing.NeedleBearing;
                CycloidWheelBearings1.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[80];
                CycloidWheelBearings1.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[81];
                CycloidWheelBearings1.mDataStorageDict["轴承内径Fw"].Value = InputParams[20];
                CycloidWheelBearings1.mDataStorageDict["轴承外径Ew"].Value = InputParams[21];
                CycloidWheelBearings1.mDataStorageDict["轴承宽度Bc"].Value = InputParams[22];
                CycloidWheelBearings1.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[24];//第一个滚针轴承
                CycloidWheelBearings1.mDataStorageDict["轴承定位面"].Value = InputParams[23];
            }

            //更新摆线轮和针轮的基本参数
            var cycloidList = ParentCrankShaft.mBelongedSet.mCycloidGearPair;
            cycloidList.mCycloidPinionList[0].mDataStorageDict["设计齿宽"].Value = InputParams[29];
            cycloidList.mCycloidPinionList[1].mDataStorageDict["设计齿宽"].Value = InputParams[34];
            cycloidList.mGear.mDataStorageDict["设计齿宽"].Value = InputParams[29] + InputParams[34];//之前这部分缺失，导致第一次更新参数后，点击轴推荐报错，结果和第一次不一致；原因为未更新该参数，导致计算mPinionList[i].BB出现了-1！！！

            //更新摆线轮和针轮的定位参数
            cycloidList.mGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[30];
            cycloidList.mGear.mDataStorageDict["定位面"].Value = InputParams[32]; //暂时将针轮和摆线轮0的定位参数设置一致√
            cycloidList.mCycloidPinionList[0].mDataStorageDict["定位面距基准面距离"].Value = InputParams[30];
            cycloidList.mCycloidPinionList[0].mDataStorageDict["定位面"].Value = InputParams[32];
            cycloidList.mCycloidPinionList[1].mDataStorageDict["定位面距基准面距离"].Value = InputParams[35];
            cycloidList.mCycloidPinionList[1].mDataStorageDict["定位面"].Value = InputParams[37];

            //更新两个圆锥滚子轴承的定位参数 
            tapered0.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[3];//第一个圆锥滚子轴承
            tapered0.mDataStorageDict["轴承定位面"].Value = InputParams[2];
            tapered0.mDataStorageDict["轴承朝向"].Value = InputParams[9];
            tapered1.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[11];//第二个圆锥滚子轴承
            tapered1.mDataStorageDict["轴承定位面"].Value = InputParams[10];
            tapered1.mDataStorageDict["轴承朝向"].Value = InputParams[17];
            //needle.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[19];//第一个滚针轴承
            //needle.mDataStorageDict["轴承定位面"].Value = InputParams[18];


            //更新行星轮的定位参数
            var invPlanetaryGear = ParentCrankShaft.mBelongedSet.mInvGearPair.mGears[1];
            invPlanetaryGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[40];
            invPlanetaryGear.mDataStorageDict["定位面"].Value = InputParams[42];
            //更新太阳轮数据
            var invSunGear = ParentCrankShaft.mBelongedSet.mInvGearPair.mGears[0];
            if (invPlanetaryGear.mDataStorageDict["定位面"].Value == 1)
            {
                invSunGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[40] + InputParams[39] / 2 - InputParams[46] / 2;
                invSunGear.mDataStorageDict["定位面"].Value = 1; //以太阳轮中点进行定位，且太阳轮中点和行星轮中点重合
            }
            else if (invPlanetaryGear.mDataStorageDict["定位面"].Value == -1)
            {
                invSunGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[40] - InputParams[39] / 2 + InputParams[46] / 2;
                invSunGear.mDataStorageDict["定位面"].Value = -1; //以太阳轮中点进行定位，且太阳轮中点和行星轮中点重合
            }
            else
            {
                invSunGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[40];
                invSunGear.mDataStorageDict["定位面"].Value = 0; //以太阳轮中点进行定位，且太阳轮中点和行星轮中点重合
            }
            //更新渐开线齿轮的直径
            invPlanetaryGear.mDataStorageDict["齿顶圆直径[da]"].Value = InputParams[38];
            invSunGear.mDataStorageDict["齿顶圆直径[da]"].Value = InputParams[45];
            //更新摆线轮直径
            cycloidList.mCycloidPinionList[0].mDataStorageDict["摆线轮顶圆直径"].Value = InputParams[28];
            cycloidList.mCycloidPinionList[1].mDataStorageDict["摆线轮顶圆直径"].Value = InputParams[33];

            // 在这里补充齿宽的更新
            invPlanetaryGear.mDataStorageDict["设计齿宽"].Value = InputParams[39]; // （请将 "齿宽" 替换为你字典中实际的键名，比如 "设计齿宽" 或 "有效齿宽"）
            invSunGear.mDataStorageDict["设计齿宽"].Value = InputParams[46];

            //将其他地方的零部件复制到当前曲轴系下
            var crankShaftSystemCycloidList = ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemCycloidPinionList;
            var crankShaftSystemCycloidWheelBearingsList = ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemSlewingBearingList;
            var crankShaftSystemGearList = ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemGearList;

            // 查找或创建摆线轮的TreeViewItem引用
            CTANCycloidPinion cycloidWheel0Item = crankShaftSystemCycloidList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧摆线轮");
            CTANCycloidPinion cycloidWheel1Item = crankShaftSystemCycloidList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧摆线轮");
            CTANSlewingBearing cycloidWheel0BearingItem = crankShaftSystemCycloidWheelBearingsList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧摆线轮滚针轴承");
            CTANSlewingBearing cycloidWheel1BearingItem = crankShaftSystemCycloidWheelBearingsList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧摆线轮滚针轴承");


            if (cycloidWheel0Item == null)
            {
                cycloidWheel0Item = new CTANCycloidPinion();
                cycloidWheel0Item.mTreeViewItem.Header = "左侧摆线轮";
                cycloidWheel0Item.mTreeViewItem.Name = "左侧摆线轮";
                cycloidWheel0Item.mInputPageGrid = cycloidList.mCycloidPinionList[0].mInputPageGrid;
                cycloidWheel0Item.mDataStorageDict = cycloidList.mCycloidPinionList[0].mDataStorageDict;
                crankShaftSystemCycloidList.Add(cycloidWheel0Item);
                treeViewItem.Items.Add(cycloidWheel0Item.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemCycloidPinionList.Add(cycloidWheel0Item);

                //还有其下的滚针轴承
                cycloidWheel0BearingItem = new CTANSlewingBearing();
                cycloidWheel0BearingItem.mTreeViewItem.Header = "左侧摆线轮滚针轴承";
                cycloidWheel0BearingItem.mTreeViewItem.Name = "左侧摆线轮滚针轴承";
                cycloidWheel0BearingItem.mInputPageGrid = cycloidWheelBearingsList[0].mInputPageGrid;
                cycloidWheel0BearingItem.mDataStorageDict = cycloidWheelBearingsList[0].mDataStorageDict;
                crankShaftSystemCycloidWheelBearingsList.Add(cycloidWheel0BearingItem);
                cycloidWheel0Item.mTreeViewItem.Items.Add(cycloidWheel0BearingItem.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemSlewingBearingList.Add(cycloidWheel0BearingItem);
            }
            if (cycloidWheel1Item == null)
            {
                cycloidWheel1Item = new CTANCycloidPinion();
                cycloidWheel1Item.mTreeViewItem.Header = "右侧摆线轮";
                cycloidWheel1Item.mTreeViewItem.Name = "右侧摆线轮";
                cycloidWheel1Item.mInputPageGrid = cycloidList.mCycloidPinionList[1].mInputPageGrid;
                cycloidWheel1Item.mDataStorageDict = cycloidList.mCycloidPinionList[1].mDataStorageDict;
                crankShaftSystemCycloidList.Add(cycloidWheel1Item);
                treeViewItem.Items.Add(cycloidWheel1Item.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemCycloidPinionList.Add(cycloidWheel1Item);

                cycloidWheel1BearingItem = new CTANSlewingBearing();
                cycloidWheel1BearingItem.mTreeViewItem.Header = "右侧摆线轮滚针轴承";
                cycloidWheel1BearingItem.mTreeViewItem.Name = "右侧摆线轮滚针轴承";
                cycloidWheel1BearingItem.mInputPageGrid = cycloidWheelBearingsList[1].mInputPageGrid;
                cycloidWheel1BearingItem.mDataStorageDict = cycloidWheelBearingsList[1].mDataStorageDict;
                crankShaftSystemCycloidWheelBearingsList.Add(cycloidWheel1BearingItem);
                cycloidWheel1Item.mTreeViewItem.Items.Add(cycloidWheel1BearingItem.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemSlewingBearingList.Add(cycloidWheel1BearingItem);
            }

            CTANInvGear gearItem = crankShaftSystemGearList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "曲轴行星轮");
            if (gearItem == null)
            {
                gearItem = new CTANInvGear();
                gearItem.mTreeViewItem.Header = "曲轴行星轮";
                gearItem.mTreeViewItem.Name = "曲轴行星轮";
                gearItem.mInputPageGrid = invPlanetaryGear.mInputPageGrid;
                gearItem.mDataStorageDict = invPlanetaryGear.mDataStorageDict;
                crankShaftSystemGearList.Add(gearItem);
                treeViewItem.Items.Add(gearItem.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemGearList.Add(gearItem);
            }

            //无论是否存在，都要更新参数
            cycloidWheel0Item.mInputPageGrid = cycloidList.mCycloidPinionList[0].mInputPageGrid;
            cycloidWheel1Item.mInputPageGrid = cycloidList.mCycloidPinionList[1].mInputPageGrid;
            cycloidWheel0BearingItem.mInputPageGrid = cycloidWheelBearingsList[0].mInputPageGrid;
            cycloidWheel1BearingItem.mInputPageGrid = cycloidWheelBearingsList[1].mInputPageGrid;
            gearItem.mInputPageGrid = invPlanetaryGear.mInputPageGrid;

            cycloidWheel0Item.mDataStorageDict = cycloidList.mCycloidPinionList[0].mDataStorageDict;
            cycloidWheel0BearingItem.mDataStorageDict = cycloidWheelBearingsList[0].mDataStorageDict;
            cycloidWheel1Item.mDataStorageDict = cycloidList.mCycloidPinionList[1].mDataStorageDict;
            cycloidWheel1BearingItem.mDataStorageDict = cycloidWheelBearingsList[1].mDataStorageDict;
            gearItem.mDataStorageDict = invPlanetaryGear.mDataStorageDict;

            // ====== 新增：绑定图形实体供后续高亮和删除使用 ======
            cycloidWheel0Item.CanvasGraphic = cycloidWheel1;
            cycloidWheel1Item.CanvasGraphic = cycloidWheel2;
            cycloidWheel0BearingItem.CanvasGraphic = needleRoller1;
            cycloidWheel1BearingItem.CanvasGraphic = needleRoller2;
            gearItem.CanvasGraphic = gear1;
            //OutputRecommendBearingPara(InputParams);//输出推荐轴承到树形列表下

        }
        private void DrawAllGraphics(DrawBearing needleRoller1, DrawBearing needleRoller2, DrawBearing bearing1, DrawBearing bearing2, DrawCycloidWheel cycloidWheel1, DrawCycloidWheel cycloidWheel2, DrawGear gear1)
        {
            needleRoller1.BasicDrawing(Pcanvas);
            needleRoller2.BasicDrawing(Pcanvas);
            bearing1.BasicDrawing(Pcanvas);
            bearing2.BasicDrawing(Pcanvas);
            cycloidWheel1.BasicDrawing(Pcanvas);
            cycloidWheel2.BasicDrawing(Pcanvas);
            gear1.BasicDrawing(Pcanvas);
            currentShaft.DrawShaftWithComponents(annotationCanvas, Pcanvas);
        }
        private void OutputRecommendBearingPara(double[] InputParams)
        {
            var treeViewItem = ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mTreeViewItem;

            var bearingList = ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingList;
            // 1. 查找是否已存在“推荐的圆锥滚子轴承”和“推荐的滚针轴承”
            CTANBearing tapered0 = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧圆锥滚子轴承");
            CTANBearing tapered1 = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧圆锥滚子轴承");

            // 2. 如果没有，则添加并设置TreeViewItem
            if (tapered0 == null)
            {
                tapered0 = new CTANBearing();
                tapered0.mTreeViewItem.Header = "左侧圆锥滚子轴承";
                tapered0.mTreeViewItem.Name = "左侧圆锥滚子轴承";
                bearingList.Add(tapered0);
                treeViewItem.Items.Add(tapered0.mTreeViewItem);
                ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingCount++;
            }
            if (tapered1 == null)
            {
                tapered1 = new CTANBearing();
                tapered1.mTreeViewItem.Header = "右侧圆锥滚子轴承";
                tapered1.mTreeViewItem.Name = "右侧圆锥滚子轴承";
                bearingList.Add(tapered1);
                treeViewItem.Items.Add(tapered1.mTreeViewItem);
                ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mBearingCount++;
            }

            // 3. 更新“推荐的圆锥滚子轴承”数据
            tapered0.mDataStorageDict["轴承类型"].Value = CTANBearing.TaperedRollerBearing;
            tapered0.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[70];
            tapered0.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[71];
            tapered0.mDataStorageDict["计算系数e"].Value = InputParams[72];
            tapered0.mDataStorageDict["计算系数Y"].Value = InputParams[73];
            tapered0.mDataStorageDict["计算系数Y0"].Value = InputParams[74];
            tapered0.mDataStorageDict["作用点距离左端面距离a"].Value = InputParams[75];
            tapered0.mDataStorageDict["轴承内径d"].Value = InputParams[4];
            tapered0.mDataStorageDict["轴承外径D"].Value = InputParams[5];
            tapered0.mDataStorageDict["轴承宽度T"].Value = InputParams[6];
            tapered0.mDataStorageDict["轴承宽度B"].Value = InputParams[7];
            tapered0.mDataStorageDict["轴承宽度C"].Value = InputParams[8];

            tapered1.mDataStorageDict["轴承类型"].Value = CTANBearing.TaperedRollerBearing;
            tapered1.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[70];
            tapered1.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[71];
            tapered1.mDataStorageDict["计算系数e"].Value = InputParams[72];
            tapered1.mDataStorageDict["计算系数Y"].Value = InputParams[73];
            tapered1.mDataStorageDict["计算系数Y0"].Value = InputParams[74];
            tapered1.mDataStorageDict["作用点距离左端面距离a"].Value = InputParams[75];
            tapered1.mDataStorageDict["轴承内径d"].Value = InputParams[4];
            tapered1.mDataStorageDict["轴承外径D"].Value = InputParams[5];
            tapered1.mDataStorageDict["轴承宽度T"].Value = InputParams[6];
            tapered1.mDataStorageDict["轴承宽度B"].Value = InputParams[7];
            tapered1.mDataStorageDict["轴承宽度C"].Value = InputParams[8];

            //更新摆线轮下面对应的滚针轴承
            var cycloidWheelBearingsList = ParentCrankShaft.mBelongedSet.mCycloidGearPair.mSlewingBearingList;
            CTANBearing CycloidWheelBearings0 = cycloidWheelBearingsList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "摆线轮0轴承");
            CTANBearing CycloidWheelBearings1 = cycloidWheelBearingsList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "摆线轮1轴承");
            if (CycloidWheelBearings0 != null && CycloidWheelBearings1 != null)
            {
                CycloidWheelBearings0.mDataStorageDict["轴承类型"].Value = CTANBearing.NeedleBearing;
                CycloidWheelBearings0.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[80];
                CycloidWheelBearings0.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[81];
                CycloidWheelBearings0.mDataStorageDict["轴承内径Fw"].Value = InputParams[20];
                CycloidWheelBearings0.mDataStorageDict["轴承外径Ew"].Value = InputParams[21];
                CycloidWheelBearings0.mDataStorageDict["轴承宽度Bc"].Value = InputParams[22];
                CycloidWheelBearings0.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[19];//第一个滚针轴承
                CycloidWheelBearings0.mDataStorageDict["轴承定位面"].Value = InputParams[18];

                CycloidWheelBearings1.mDataStorageDict["轴承类型"].Value = CTANBearing.NeedleBearing;
                CycloidWheelBearings1.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[80];
                CycloidWheelBearings1.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[81];
                CycloidWheelBearings1.mDataStorageDict["轴承内径Fw"].Value = InputParams[20];
                CycloidWheelBearings1.mDataStorageDict["轴承外径Ew"].Value = InputParams[21];
                CycloidWheelBearings1.mDataStorageDict["轴承宽度Bc"].Value = InputParams[22];
                CycloidWheelBearings1.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[24];//第一个滚针轴承
                CycloidWheelBearings1.mDataStorageDict["轴承定位面"].Value = InputParams[23];
            }

            //更新摆线轮和针轮的基本参数
            var cycloidList = ParentCrankShaft.mBelongedSet.mCycloidGearPair;
            cycloidList.mCycloidPinionList[0].mDataStorageDict["设计齿宽"].Value = InputParams[29];
            cycloidList.mCycloidPinionList[1].mDataStorageDict["设计齿宽"].Value = InputParams[34];
            cycloidList.mGear.mDataStorageDict["设计齿宽"].Value = InputParams[29] + InputParams[34];//之前这部分缺失，导致第一次更新参数后，点击轴推荐报错，结果和第一次不一致；原因为未更新该参数，导致计算mPinionList[i].BB出现了-1！！！

            //更新摆线轮和针轮的定位参数
            cycloidList.mGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[30];
            cycloidList.mGear.mDataStorageDict["定位面"].Value = InputParams[32]; //暂时将针轮和摆线轮0的定位参数设置一致√
            cycloidList.mCycloidPinionList[0].mDataStorageDict["定位面距基准面距离"].Value = InputParams[30];
            cycloidList.mCycloidPinionList[0].mDataStorageDict["定位面"].Value = InputParams[32];
            cycloidList.mCycloidPinionList[1].mDataStorageDict["定位面距基准面距离"].Value = InputParams[35];
            cycloidList.mCycloidPinionList[1].mDataStorageDict["定位面"].Value = InputParams[37];

            //更新两个圆锥滚子轴承的定位参数 
            tapered0.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[3];//第一个圆锥滚子轴承
            tapered0.mDataStorageDict["轴承定位面"].Value = InputParams[2];
            tapered0.mDataStorageDict["轴承朝向"].Value = InputParams[9];
            tapered1.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[11];//第二个圆锥滚子轴承
            tapered1.mDataStorageDict["轴承定位面"].Value = InputParams[10];
            tapered1.mDataStorageDict["轴承朝向"].Value = InputParams[17];
            //needle.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[19];//第一个滚针轴承
            //needle.mDataStorageDict["轴承定位面"].Value = InputParams[18];


            //更新行星轮的定位参数
            var invPlanetaryGear = ParentCrankShaft.mBelongedSet.mInvGearPair.mGears[1];
            invPlanetaryGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[40];
            invPlanetaryGear.mDataStorageDict["定位面"].Value = InputParams[42];
            //更新太阳轮数据
            var invSunGear = ParentCrankShaft.mBelongedSet.mInvGearPair.mGears[0];
            if (invPlanetaryGear.mDataStorageDict["定位面"].Value == 1)
            {
                invSunGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[40] + InputParams[39] / 2 - InputParams[46] / 2;
                invSunGear.mDataStorageDict["定位面"].Value = 1; //以太阳轮中点进行定位，且太阳轮中点和行星轮中点重合
            }
            else if (invPlanetaryGear.mDataStorageDict["定位面"].Value == -1)
            {
                invSunGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[40] - InputParams[39] / 2 + InputParams[46] / 2;
                invSunGear.mDataStorageDict["定位面"].Value = -1; //以太阳轮中点进行定位，且太阳轮中点和行星轮中点重合
            }
            else
            {
                invSunGear.mDataStorageDict["定位面距基准面距离"].Value = InputParams[40];
                invSunGear.mDataStorageDict["定位面"].Value = 0; //以太阳轮中点进行定位，且太阳轮中点和行星轮中点重合
            }
            //更新渐开线齿轮的直径
            invPlanetaryGear.mDataStorageDict["齿顶圆直径[da]"].Value = InputParams[38];
            invSunGear.mDataStorageDict["齿顶圆直径[da]"].Value = InputParams[45];
            //更新摆线轮直径
            cycloidList.mCycloidPinionList[0].mDataStorageDict["摆线轮顶圆直径"].Value = InputParams[28];
            cycloidList.mCycloidPinionList[1].mDataStorageDict["摆线轮顶圆直径"].Value = InputParams[33];


            //将其他地方的零部件复制到当前曲轴系下
            var crankShaftSystemCycloidList = ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemCycloidPinionList;
            var crankShaftSystemCycloidWheelBearingsList = ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemSlewingBearingList;
            var crankShaftSystemGearList = ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemGearList;

            // 查找或创建摆线轮的TreeViewItem引用
            CTANCycloidPinion cycloidWheel0Item = crankShaftSystemCycloidList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧摆线轮");
            CTANCycloidPinion cycloidWheel1Item = crankShaftSystemCycloidList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧摆线轮");
            CTANSlewingBearing cycloidWheel0BearingItem = crankShaftSystemCycloidWheelBearingsList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧摆线轮滚针轴承");
            CTANSlewingBearing cycloidWheel1BearingItem = crankShaftSystemCycloidWheelBearingsList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧摆线轮滚针轴承");


            if (cycloidWheel0Item == null)
            {
                cycloidWheel0Item = new CTANCycloidPinion();
                cycloidWheel0Item.mTreeViewItem.Header = "左侧摆线轮";
                cycloidWheel0Item.mTreeViewItem.Name = "左侧摆线轮";
                cycloidWheel0Item.mInputPageGrid = cycloidList.mCycloidPinionList[0].mInputPageGrid;
                cycloidWheel0Item.mDataStorageDict = cycloidList.mCycloidPinionList[0].mDataStorageDict;
                crankShaftSystemCycloidList.Add(cycloidWheel0Item);
                treeViewItem.Items.Add(cycloidWheel0Item.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemCycloidPinionList.Add(cycloidWheel0Item);

                //还有其下的滚针轴承
                cycloidWheel0BearingItem = new CTANSlewingBearing();
                cycloidWheel0BearingItem.mTreeViewItem.Header = "左侧摆线轮滚针轴承";
                cycloidWheel0BearingItem.mTreeViewItem.Name = "左侧摆线轮滚针轴承";
                cycloidWheel0BearingItem.mInputPageGrid = cycloidWheelBearingsList[0].mInputPageGrid;
                cycloidWheel0BearingItem.mDataStorageDict = cycloidWheelBearingsList[0].mDataStorageDict;
                crankShaftSystemCycloidWheelBearingsList.Add(cycloidWheel0BearingItem);
                cycloidWheel0Item.mTreeViewItem.Items.Add(cycloidWheel0BearingItem.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemSlewingBearingList.Add(cycloidWheel0BearingItem);
            }
            if (cycloidWheel1Item == null)
            {
                cycloidWheel1Item = new CTANCycloidPinion();
                cycloidWheel1Item.mTreeViewItem.Header = "右侧摆线轮";
                cycloidWheel1Item.mTreeViewItem.Name = "右侧摆线轮";
                cycloidWheel1Item.mInputPageGrid = cycloidList.mCycloidPinionList[1].mInputPageGrid;
                cycloidWheel1Item.mDataStorageDict = cycloidList.mCycloidPinionList[1].mDataStorageDict;
                crankShaftSystemCycloidList.Add(cycloidWheel1Item);
                treeViewItem.Items.Add(cycloidWheel1Item.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemCycloidPinionList.Add(cycloidWheel1Item);

                cycloidWheel1BearingItem = new CTANSlewingBearing();
                cycloidWheel1BearingItem.mTreeViewItem.Header = "右侧摆线轮滚针轴承"; 
                cycloidWheel1BearingItem.mTreeViewItem.Name = "右侧摆线轮滚针轴承";
                cycloidWheel1BearingItem.mInputPageGrid = cycloidWheelBearingsList[1].mInputPageGrid;
                cycloidWheel1BearingItem.mDataStorageDict = cycloidWheelBearingsList[1].mDataStorageDict;
                crankShaftSystemCycloidWheelBearingsList.Add(cycloidWheel1BearingItem);
                cycloidWheel1Item.mTreeViewItem.Items.Add(cycloidWheel1BearingItem.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemSlewingBearingList.Add(cycloidWheel1BearingItem);
            }

            CTANInvGear gearItem = crankShaftSystemGearList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "曲轴行星轮");
            if (gearItem == null)
            {
                gearItem = new CTANInvGear();
                gearItem.mTreeViewItem.Header = "曲轴行星轮";
                gearItem.mTreeViewItem.Name = "曲轴行星轮";
                gearItem.mInputPageGrid = invPlanetaryGear.mInputPageGrid;
                gearItem.mDataStorageDict = invPlanetaryGear.mDataStorageDict;
                crankShaftSystemGearList.Add(gearItem);
                treeViewItem.Items.Add(gearItem.mTreeViewItem);
                ParentCrankShaft.ParentCrankShaftSystem.mCrankShaftSystemGearList.Add(gearItem);
            }

            //无论是否存在，都要更新参数
            cycloidWheel0Item.mInputPageGrid = cycloidList.mCycloidPinionList[0].mInputPageGrid;
            cycloidWheel1Item.mInputPageGrid = cycloidList.mCycloidPinionList[1].mInputPageGrid;
            cycloidWheel0BearingItem.mInputPageGrid = cycloidWheelBearingsList[0].mInputPageGrid;
            cycloidWheel1BearingItem.mInputPageGrid = cycloidWheelBearingsList[1].mInputPageGrid;
            gearItem.mInputPageGrid = invPlanetaryGear.mInputPageGrid;

            cycloidWheel0Item.mDataStorageDict = cycloidList.mCycloidPinionList[0].mDataStorageDict;
            cycloidWheel0BearingItem.mDataStorageDict = cycloidWheelBearingsList[0].mDataStorageDict;
            cycloidWheel1Item.mDataStorageDict = cycloidList.mCycloidPinionList[1].mDataStorageDict;
            cycloidWheel1BearingItem.mDataStorageDict = cycloidWheelBearingsList[1].mDataStorageDict;
            gearItem.mDataStorageDict = invPlanetaryGear.mDataStorageDict;
        }
        private void UpdateCrankShaft()
        {
            ParentCrankShaft.mBelongedSet.mCrankShaftSystem.mCrankShaft.mDataStorageDict["偏心量"].Value = currentShaft.AxisSections[1].EccentricDiameter;
        }
        // 使用视觉边界检测
        private bool IsPointInsideCanvas(Canvas canvas, Point point)
        {
            try
            {
                // 获取视觉边界
                Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
                // 转换为父坐标系
                GeneralTransform transform = canvas.TransformToAncestor(Pcanvas);
                Rect rect = transform.TransformBounds(bounds);

                return rect.Contains(point);
            }
            catch
            {
                MessageBox.Show("画布内部视觉边界检测错误，请联系软件作者。");
                return false;
            }
        }
        // 判断点是否在多边形内的方法
        private bool IsPointInsidePolygon(Polygon polygon, Point point)
        {
            return polygon.RenderedGeometry.FillContains(point);
        }
        //private Point TransformPointToContent(Point pointRelativeToPcanvas)
        //{
        //    try
        //    {
        //        // canvasTransformGroup.Value 是 Pcanvas 的 RenderTransform 矩阵（Scale + Translate）
        //        var m = canvasTransformGroup.Value;
        //        if (m.HasInverse)
        //        {
        //            m.Invert();
        //            return m.Transform(pointRelativeToPcanvas);
        //        }
        //        return pointRelativeToPcanvas;
        //    }
        //    catch
        //    {
        //        return pointRelativeToPcanvas;
        //    }
        //}

        //private bool IsPointInsidePolygon(Polygon polygon, Point pointRelativeToPcanvas)
        //{
        //    try
        //    {
        //        Point contentPoint = TransformPointToContent(pointRelativeToPcanvas);
        //        return polygon.RenderedGeometry.FillContains(contentPoint);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        // 清除高亮的方法
        private void ClearHighlight()
        {
            // 清除多边形高亮
            if (highlightedPolygon != null)
            {
                highlightedPolygon.Stroke = Brushes.Gray;
                highlightedPolygon.StrokeThickness = 0.2;
                highlightedPolygon = null;
            }

            // 清除画布高亮
            if (highlightedCanvas != null)
            {
                var highlightLayer = highlightedCanvas.Children
                    .OfType<Rectangle>()
                    .FirstOrDefault(r => r.Name == "HighlightLayer");

                if (highlightLayer != null)
                    highlightLayer.Fill = Brushes.Transparent;

                highlightedCanvas = null;
            }

            selectedComponentKey = null;
        }
        #region 锥形控制事件
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
        #endregion
    }
    //获取其他图元位于的轴段的相关信息（也是轴段的Tag）
    public class AxisTagWithParam
    {
        public Shaft.ComponentKey Key { get; set; }
        public bool IsPeccentricshaft { get; set; }
        public double EccentricDiameter { get; set; }
        public double Phase { get; set; }
    }
    //根据X偏移量获取轴段的Y轴偏移量
    public static class CanvasHelper
    {
        public static double GetAxisOffsetY(Canvas canvas, double offsetX)
        {
            foreach (var polygon in canvas.Children.OfType<Polygon>())
            {
                if (polygon.Tag is AxisTagWithParam tag)
                {
                    double left = polygon.Points.Min(p => p.X);
                    double right = polygon.Points.Max(p => p.X);
                    if (offsetX + canvas.ActualWidth / 2 >= left && offsetX + canvas.ActualWidth / 2 <= right && tag.IsPeccentricshaft)
                    {
                        double radians = tag.Phase * Math.PI / 180;
                        double tmp = tag.EccentricDiameter * Math.Cos(radians);
                        return tmp;
                    }
                }
            }
            return 0;
        }
        public static int GetAxisIndex(Canvas canvas, double offsetX)
        {
            foreach (var polygon in canvas.Children.OfType<Polygon>())
            {
                if (polygon.Tag is AxisTagWithParam tag)
                {
                    double left = polygon.Points.Min(p => p.X);
                    double right = polygon.Points.Max(p => p.X);
                    if (offsetX >= left && offsetX <= right)
                    {
                        return tag.Key.Index;
                    }
                }
            }
            return 0;
        }
    }
}
