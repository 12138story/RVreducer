using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// UILIneChart.xaml 的交互逻辑
    /// </summary>  
    public partial class UILIneChart : Page
    {
        private readonly List<DrawDataSet> _allDataSets = new List<DrawDataSet>();
        private int _loadCallCount = 1;
        public int TotalDataSetsCount => _allDataSets.Count;
        private bool _isInitialParamsSet = false;
        private double _initialGlobalMaxY;

        // 基础常量
        private const double YExtensionRatio = 0;
        private const double MinLeftMargin = 100;
        private const double MaxLeftMargin = 200;
        private const double AxisEndPadding = 5;
        private const int MinXAxisTicks = 4;
        private const int MaxXAxisTicks = 10;
        private const double TopBlank = 40;
        private const double MinBottomMargin = 60;
        private const double TitleToYAxisRatio = 0.03;
        private const int TitleMinSize = 10;
        private const int TitleMaxSize = 24;
        private const double FontScaleRatio = 0.016; // 刻度字体基准
        private const int MinDynamicFontSize = 8;
        private const int MaxDynamicFontSize = 14;
        private const double YTitleToTicksSpacing = 25;

        // 图例参数
        private const double LegendBaseScale = 0.0012;
        private const double LegendMaxWidthRatio = 0.3;
        private const double LegendMaxHeightRatio = 0.4;
        private const double LegendIconToWidthRatio = 0.12;
        private const double LegendItemSpacingRatio = 0.1;
        private const double LegendPaddingRatio = 0.05;
        private const double FontToIconRatio = 0.6; // 图例字体相对图块比例


        public UILIneChart()
        {
            InitializeComponent();
            Loaded += Page1_Loaded;
            drawCanvas.SizeChanged += (s, e) =>
            {
                if (_loadCallCount > 0)
                {
                    CalculateDataRange();
                    CalculateMargins();
                    Re(_loadCallCount);
                }
            };
        }

        private void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            Frame parentFrame = GetParentFrame(this);
            if (parentFrame != null)
            {
                parentFrame.SizeChanged += (s, args) =>
                {
                    if (_loadCallCount > 0)
                    {
                        CalculateDataRange();
                        CalculateMargins();
                        Re(_loadCallCount);
                    }
                };
            }
        }

        public void Load(double[][] x, double[][] y, string[][] code)
        {
            _isInitialParamsSet = false;
            InitDrawData(x, y, _loadCallCount, code);
            CalculateDataRange();
            CalculateMargins();
            Re(_loadCallCount);
        }

        private void CalculateMargins()
        {
            if (_allDataSets.Count == 0) return;

            var dataSet = _allDataSets[0];
            string yTitle = dataSet.YAxisTitle;
            string xTitle = dataSet.XAxisTitle;

            TextBlock titleMeasure = new TextBlock { Text = yTitle, FontSize = TitleMaxSize };
            titleMeasure.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double maxTitleWidth = titleMeasure.DesiredSize.Width;
            double leftMargin = Math.Clamp(maxTitleWidth + YTitleToTicksSpacing + 70, MinLeftMargin, MaxLeftMargin);

            titleMeasure.Text = xTitle;
            titleMeasure.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double bottomMargin = Math.Max(titleMeasure.DesiredSize.Height + 30, MinBottomMargin);

            double baseLegendWidth = drawCanvas.ActualWidth * LegendBaseScale * 100;
            double legendMin = 100;
            double legendMax = drawCanvas.ActualWidth * LegendMaxWidthRatio;
            double actualMin = Math.Min(legendMin, legendMax);
            double actualMax = Math.Max(legendMin, legendMax);
            dataSet.LegendBaseWidth = Math.Clamp(baseLegendWidth, actualMin, actualMax);
            foreach (var ds in _allDataSets)
            {
                ds.LeftMargin = leftMargin;
                ds.BottomMargin = bottomMargin;
            }
        }

        private void InitDrawData(double[][] x, double[][] y, int loadCount, string[][] code)
        {
            if (loadCount == 1)
                _allDataSets.Clear();

            int codeIndex = loadCount - 1;

            if (code == null || codeIndex >= code.Length)
            {
                InitWithDefaultTitles(x, y);
                return;
            }

            string[] currentCode = code[codeIndex];
            string xAxisTitle = currentCode.Length > 0 ? currentCode[0] : "X轴";
            string yAxisTitle = currentCode.Length > 1 ? currentCode[1] : "Y轴";
            string chartTitle = currentCode.Length > 2 ? currentCode[2] : "折线图";

            List<string> seriesNames = new List<string>();
            for (int i = 3; i < currentCode.Length; i++)
            {
                seriesNames.Add(currentCode[i]);
            }

            if (x == null || y == null || x.Length != y.Length)
            {
                MessageBox.Show("x和y数组长度不一致或为空");
                return;
            }

            var newDataSet = new DrawDataSet
            {
                Names = seriesNames.ToArray(),
                DataQuantity = x.Length,
                XAxisTitle = xAxisTitle,
                YAxisTitle = yAxisTitle,
                ChartTitle = chartTitle,
                LeftMargin = MinLeftMargin,
                LegendBaseWidth = 100
            };
            _allDataSets.Add(newDataSet);

            var (x1, y1) = ProcessSeriesWithInterpolation(x, y, 0, newDataSet);
            var (x2, y2) = ProcessSeriesWithInterpolation(x, y, 1, newDataSet);
            var (x3, y3) = ProcessSeriesWithInterpolation(x, y, 2, newDataSet);

            newDataSet.DataX1 = x1;
            newDataSet.DataY1 = y1;
            newDataSet.DataX2 = x2;
            newDataSet.DataY2 = y2;
            newDataSet.DataX3 = x3;
            newDataSet.DataY3 = y3;
        }

        private (List<double> x, List<double> y) ProcessSeriesWithInterpolation(double[][] x, double[][] y, int index, DrawDataSet dataSet)
        {
            if (index < 0 || index >= x.Length || index >= y.Length)
                return (new List<double>(), new List<double>());

            double[] rawX = x[index]?.Select(val => Math.Abs(val)).ToArray() ?? Array.Empty<double>();
            double[] rawY = y[index]?.Select(val => Math.Abs(val)).ToArray() ?? Array.Empty<double>();

            switch (index)
            {
                case 0:
                    dataSet.OriginalX1 = rawX.ToList();
                    dataSet.OriginalY1 = rawY.ToList();
                    break;
                case 1:
                    dataSet.OriginalX2 = rawX.ToList();
                    dataSet.OriginalY2 = rawY.ToList();
                    break;
                case 2:
                    dataSet.OriginalX3 = rawX.ToList();
                    dataSet.OriginalY3 = rawY.ToList();
                    break;
            }

            int minLength = Math.Min(rawX.Length, rawY.Length);
            var resultX = rawX.Take(minLength).ToList();
            var resultY = rawY.Take(minLength).ToList();

            return (resultX, resultY);
        }

        private void InitWithDefaultTitles(double[][] x, double[][] y)
        {
            var newDataSet = new DrawDataSet
            {
                Names = new[] { "系列1", "系列2", "系列3" },
                DataQuantity = x.Length,
                XAxisTitle = "X轴",
                YAxisTitle = "Y轴",
                ChartTitle = "折线图",
                LeftMargin = MinLeftMargin,
                LegendBaseWidth = 100
            };
            _allDataSets.Add(newDataSet);

            var (x1, y1) = ProcessSeriesWithInterpolation(x, y, 0, newDataSet);
            var (x2, y2) = ProcessSeriesWithInterpolation(x, y, 1, newDataSet);
            var (x3, y3) = ProcessSeriesWithInterpolation(x, y, 2, newDataSet);

            newDataSet.DataX1 = x1;
            newDataSet.DataY1 = y1;
            newDataSet.DataX2 = x2;
            newDataSet.DataY2 = y2;
            newDataSet.DataX3 = x3;
            newDataSet.DataY3 = y3;
        }

        private DrawDataSet GetDataSetByIndex(int index)
        {
            if (index >= 0 && index < _allDataSets.Count) return _allDataSets[index];
            throw new ArgumentOutOfRangeException(nameof(index), "数据集索引超出范围");
        }

        private void Re(int count)
        {
            if (_allDataSets.Count == 0) return;

            Frame parentFrame = GetParentFrame(this);
            if (parentFrame == null) return;
            double frameWidth = parentFrame.ActualWidth;
            double frameHeight = parentFrame.ActualHeight;

            var dataSet = _allDataSets[0];
            CalculateMargins();
            double leftMargin = dataSet.LeftMargin;
            double rightMargin = dataSet.DataQuantity > 1 ? dataSet.LegendBaseWidth : 80;
            double topMargin = TopBlank;
            double bottomMargin = dataSet.BottomMargin;

            double xAvailableWidth = frameWidth - leftMargin - rightMargin;
            xAvailableWidth = Math.Max(xAvailableWidth, 200);

            double xMin = _allDataSets.Min(ds => ds.DataMin[0]);
            double xMax = _allDataSets.Max(ds => ds.DataMax[0]);
            double yMin = _allDataSets.Min(ds => ds.DataMin[1]);
            double yMax = _allDataSets.Max(ds => ds.DataMax[1]);

            double targetYLength = xAvailableWidth;
            double yAvailableHeight = Math.Min(frameHeight - topMargin - bottomMargin, targetYLength);
            yAvailableHeight = Math.Max(yAvailableHeight, 200);

            double xInterval = CalculateOptimalInterval(xMax, MinXAxisTicks, MaxXAxisTicks);
            double yInterval = CalculateOptimalInterval(yMax, MinXAxisTicks, MaxXAxisTicks);

            double xMaxTick = Math.Ceiling(xMax / xInterval) * xInterval;
            double yMaxTick = Math.Ceiling(yMax / yInterval) * yInterval;

            xMaxTick = Math.Max(xMaxTick, xMax);
            yMaxTick = Math.Max(yMaxTick, yMax);

            int xTickCount = (int)Math.Ceiling(xMaxTick / xInterval);
            int yTickCount = (int)Math.Ceiling(yMaxTick / yInterval);

            if (xTickCount < MinXAxisTicks)
            {
                xInterval = CalculateOptimalInterval(xMaxTick, MinXAxisTicks, MaxXAxisTicks);
                xMaxTick = Math.Ceiling(xMax / xInterval) * xInterval;
            }

            if (yTickCount < MinXAxisTicks)
            {
                yInterval = CalculateOptimalInterval(yMaxTick, MinXAxisTicks, MaxXAxisTicks);
                yMaxTick = Math.Ceiling(yMax / yInterval) * yInterval;
            }

            double scaleX = xAvailableWidth / xMaxTick;
            double scaleY = yAvailableHeight / yMaxTick;

            dataSet.ScaleX = scaleX;
            dataSet.ScaleY = scaleY;
            dataSet.OffsetX = leftMargin;
            dataSet.OffsetY = topMargin + yAvailableHeight;
            dataSet.AvailableWidth = xAvailableWidth;
            dataSet.AvailableHeight = yAvailableHeight;
            dataSet.XTotalExtend = xMaxTick;
            dataSet.LastTickX = xMaxTick;
            dataSet.YAxisMaxTick = yMaxTick;
            dataSet.XInterval = xInterval;
            dataSet.YInterval = yInterval;

            drawCanvas.Children.Clear();
            DrawAllDataSets(_allDataSets.Select(ds => new string[] {
                ds.XAxisTitle, ds.YAxisTitle, ds.ChartTitle
            }).ToArray(), xMax);
        }

        private double CalculateOptimalInterval(double maxValue, int minTicks, int maxTicks)
        {
            if (maxValue <= 0) return 1.0;

            double magnitude = Math.Pow(10, Math.Floor(Math.Log10(maxValue)));
            double[] candidates = new[] { 0.1, 0.2, 0.5, 1.0, 2.0, 5.0, 10.0 };

            foreach (double candidate in candidates)
            {
                double interval = candidate * magnitude;
                if (interval <= 0) continue;

                int tickCount = (int)Math.Ceiling(maxValue / interval);

                if (tickCount >= minTicks && tickCount <= maxTicks)
                {
                    return interval;
                }
            }

            int targetTicks = (minTicks + maxTicks) / 2;
            double bestInterval = 0;
            int bestDiff = int.MaxValue;

            foreach (double candidate in candidates)
            {
                double interval = candidate * magnitude;
                if (interval <= 0) continue;

                int tickCount = (int)Math.Ceiling(maxValue / interval);
                int diff = Math.Abs(tickCount - targetTicks);

                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    bestInterval = interval;
                }
            }

            return bestInterval > 0 ? bestInterval : magnitude;
        }

        private Frame GetParentFrame(DependencyObject element)
        {
            while (element != null && !(element is Frame))
            {
                element = VisualTreeHelper.GetParent(element);
            }
            return element as Frame;
        }

        private void CalculateDataRange()
        {
            double globalMaxY = 0;
            double globalMaxX = 0;

            foreach (var dataSet in _allDataSets)
            {
                var allXData = new List<double>();
                var allYData = new List<double>();
                AddDataToList(dataSet.DataX1, allXData);
                AddDataToList(dataSet.DataX2, allXData);
                AddDataToList(dataSet.DataX3, allXData);
                AddDataToList(dataSet.DataY1, allYData);
                AddDataToList(dataSet.DataY2, allYData);
                AddDataToList(dataSet.DataY3, allYData);

                dataSet.LastDataPointX = GetLastDataPointX(dataSet);

                double maxX = allXData.Any() ? allXData.Max() : 0;
                double maxY = allYData.Any() ? allYData.Max() : 0;
                dataSet.DataMax = new[] { maxX, maxY };
                dataSet.DataMin = new[] { 0.0, 0.0 };

                if (maxY > globalMaxY) globalMaxY = maxY;
                if (maxX > globalMaxX) globalMaxX = maxX;
            }

            _initialGlobalMaxY = globalMaxY;
        }

        private double GetLastDataPointX(DrawDataSet dataSet)
        {
            double lastX = 0;
            if (dataSet.DataX1.Any()) lastX = Math.Max(lastX, dataSet.DataX1.Last());
            if (dataSet.DataX2.Any()) lastX = Math.Max(lastX, dataSet.DataX2.Last());
            if (dataSet.DataX3.Any()) lastX = Math.Max(lastX, dataSet.DataX3.Last());
            return lastX;
        }

        private void AddDataToList(List<double> source, List<double> target)
        {
            if (source != null && source.Any()) target.AddRange(source);
        }

        private void DrawAllDataSets(string[][] allCode, double dataMaxX)
        {
            if (_allDataSets.Count == 0) return;

            var dataSet = GetDataSetByIndex(0);
            string[] currentCode = new string[] {
                dataSet.XAxisTitle,
                dataSet.YAxisTitle,
                dataSet.ChartTitle
            };

            var drawBase = new DrawBase(
                dataSet.OffsetX,
                dataSet.OffsetY,
                dataSet.DataX1,
                dataSet.DataY1,
                dataSet.DataX2,
                dataSet.DataY2,
                dataSet.DataX3,
                dataSet.DataY3,
                dataSet.Names,
                dataSet.DataQuantity,
                dataSet.ScaleX,
                dataSet.ScaleY,
                dataSet.AvailableWidth,
                dataSet.AvailableHeight,
                YExtensionRatio,
                0,
                dataSet.LegendBaseWidth,
                dataSet.XTotalExtend,
                dataSet.LastDataPointX,
                dataMaxX,
                dataSet.LastTickX,
                MinXAxisTicks,
                MaxXAxisTicks,
                _initialGlobalMaxY,
                dataSet.YAxisMaxTick,
                dataSet,
                dataSet.XInterval,
                dataSet.YInterval,
                TitleToYAxisRatio,
                TitleMinSize, TitleMaxSize
            );

            drawBase.DrawZXT(drawCanvas, 0, currentCode);
        }

        public static List<Tuple<double, double>> CubicSplineInterpolate(double[] x, double[] y, int interpolateCount)
        {
            if (x == null || y == null)
                throw new ArgumentNullException("x或y不能为null");
            if (x.Length != y.Length)
                throw new ArgumentException("x和y长度必须相等");
            if (x.Length < 2)
                throw new ArgumentException("至少需要2个原始数据点");
            if (interpolateCount <= 0)
                throw new ArgumentException("插值个数必须是正整数");

            var sortedPoints = x.Zip(y, (xi, yi) => Tuple.Create(xi, yi))
                                .OrderBy(p => p.Item1)
                                .ToList();
            double[] sortedX = sortedPoints.Select(p => p.Item1).ToArray();
            double[] sortedY = sortedPoints.Select(p => p.Item2).ToArray();
            int n = sortedX.Length;

            double minX = sortedX[0];
            double maxX = sortedX.Last();
            if (minX >= maxX)
                throw new ArgumentException("x不能全为相同值或降序排列");

            List<double> targetXList = new List<double>();
            double step = (maxX - minX) / (interpolateCount + 1);
            for (int i = 1; i <= interpolateCount; i++)
            {
                double targetX = minX + step * i;
                if (!sortedX.Any(xi => Math.Abs(targetX - xi) < 1e-9))
                {
                    targetXList.Add(targetX);
                }
            }
            while (targetXList.Count < interpolateCount)
            {
                double lastX = targetXList.LastOrDefault(minX);
                double newX = lastX + step + 1e-9;
                if (newX > maxX) newX = maxX - 1e-9;
                targetXList.Add(newX);
            }

            double[] h = new double[n - 1];
            double[] alpha = new double[n];
            double[] l = new double[n];
            double[] mu = new double[n];
            double[] z = new double[n];
            double[] c = new double[n];
            double[] b = new double[n - 1];
            double[] d = new double[n - 1];

            for (int i = 0; i < n - 1; i++)
                h[i] = sortedX[i + 1] - sortedX[i];
            for (int i = 1; i < n - 1; i++)
                alpha[i] = 3 * ((sortedY[i + 1] - sortedY[i]) / h[i] - (sortedY[i] - sortedY[i - 1]) / h[i - 1]);

            l[0] = 1;
            mu[0] = 0;
            z[0] = 0;
            for (int i = 1; i < n - 1; i++)
            {
                l[i] = 2 * (sortedX[i + 1] - sortedX[i - 1]) - h[i - 1] * mu[i - 1];
                mu[i] = h[i] / l[i];
                z[i] = (alpha[i] - h[i - 1] * z[i - 1]) / l[i];
            }
            l[n - 1] = 1;
            z[n - 1] = 0;
            c[n - 1] = 0;

            for (int j = n - 2; j >= 0; j--)
            {
                c[j] = z[j] - mu[j] * c[j + 1];
                b[j] = (sortedY[j + 1] - sortedY[j]) / h[j] - h[j] * (c[j + 1] + 2 * c[j]) / 3;
                d[j] = (c[j + 1] - c[j]) / (3 * h[j]);
            }

            List<Tuple<double, double>> interpolatedPoints = new List<Tuple<double, double>>();
            foreach (double tx in targetXList)
            {
                int k = 0;
                while (k < n - 1 && tx > sortedX[k + 1])
                    k++;
                double dx = tx - sortedX[k];
                double ty = sortedY[k] + b[k] * dx + c[k] * dx * dx + d[k] * dx * dx * dx;
                interpolatedPoints.Add(Tuple.Create(tx, ty));
            }

            return sortedPoints.Concat(interpolatedPoints)
                               .OrderBy(p => p.Item1)
                               .ToList();
        }

        public class DrawDataSet
        {
            public List<double> DataX1 { get; set; } = new List<double>();
            public List<double> DataY1 { get; set; } = new List<double>();
            public List<double> DataX2 { get; set; } = new List<double>();
            public List<double> DataY2 { get; set; } = new List<double>();
            public List<double> DataX3 { get; set; } = new List<double>();
            public List<double> DataY3 { get; set; } = new List<double>();
            public List<double> OriginalX1 { get; set; } = new List<double>();
            public List<double> OriginalY1 { get; set; } = new List<double>();
            public List<double> OriginalX2 { get; set; } = new List<double>();
            public List<double> OriginalY2 { get; set; } = new List<double>();
            public List<double> OriginalX3 { get; set; } = new List<double>();
            public List<double> OriginalY3 { get; set; } = new List<double>();

            public double[] DataMax { get; set; } = new double[2];
            public double[] DataMin { get; set; } = new double[2];
            public string[] Names { get; set; } = Array.Empty<string>();
            public int DataQuantity { get; set; }
            public string XAxisTitle { get; set; } = "X轴";
            public string YAxisTitle { get; set; } = "Y轴";
            public string ChartTitle { get; set; } = "折线图";
            public double ScaleX { get; set; }
            public double ScaleY { get; set; }
            public double OffsetX { get; set; }
            public double OffsetY { get; set; }
            public double AvailableWidth { get; set; }
            public double AvailableHeight { get; set; }
            public double LeftMargin { get; set; }
            public double LegendBaseWidth { get; set; }
            public double XTotalExtend { get; set; }
            public double LastDataPointX { get; set; }
            public double LastTickX { get; set; }
            public double DataMaxX { get; set; }
            public double YAxisMaxTick { get; set; }
            public double XInterval { get; set; }
            public double YInterval { get; set; }
            public double BottomMargin { get; set; } = MinBottomMargin;
        }

        public class DrawBase
        {
            private readonly double _offsetX;
            private readonly double _offsetY;
            private readonly List<double> _x1;
            private readonly List<double> _y1;
            private readonly List<double> _x2;
            private readonly List<double> _y2;
            private readonly List<double> _x3;
            private readonly List<double> _y3;
            private readonly string[] _dataNames;
            private readonly int _dataQuantity;
            private readonly double _scaleX;
            private readonly double _scaleY;
            private readonly double _xAvailableWidth;
            private readonly double _yAvailableHeight;
            private readonly double _yExtensionRatio;
            private readonly double _yAxisIntervalBase;
            private readonly double _legendBaseWidth;
            private readonly double _axisEndPadding = AxisEndPadding;
            private readonly double _xTotalExtend;
            private readonly double _lastDataPointX;
            private readonly double _dataMaxX;
            private readonly double _lastTickX;
            private readonly int _minXAxisTicks;
            private readonly int _maxXAxisTicks;
            private readonly double _tickLength = 5;
            private readonly double _initialGlobalMaxY;
            private readonly double _yAxisMaxTick;
            private readonly DrawDataSet _dataSet;
            private readonly double _xInterval;
            private readonly double _yInterval;
            private readonly double _titleToYAxisRatio;
            private readonly int _titleMinSize;
            private readonly int _titleMaxSize;
            private List<Rect> _occupiedAreas;
            private List<LineSegment> _lineSegments;
            private double _dynamicFontSize; // 刻度字体基准
            private readonly double _fontScaleRatio = FontScaleRatio;
            private readonly int _minDynamicFontSize = MinDynamicFontSize;
            private readonly int _maxDynamicFontSize = MaxDynamicFontSize;
            private readonly double _yTitleToTicksSpacing = YTitleToTicksSpacing;

            // 图例参数
            private readonly double _legendBaseScale = LegendBaseScale;
            private readonly double _legendMaxWidthRatio = LegendMaxWidthRatio;
            private readonly double _legendMaxHeightRatio = LegendMaxHeightRatio;
            private readonly double _legendIconToWidthRatio = LegendIconToWidthRatio;
            private readonly double _legendItemSpacingRatio = LegendItemSpacingRatio;
            private readonly double _legendPaddingRatio = LegendPaddingRatio;
            private readonly double _fontToIconRatio = FontToIconRatio;

            // 动态计算的元素尺寸
            private double _legendScaleFactor;
            private double _legendWidth;
            private double _legendHeight;
            private double _legendIconSize;
            private double _legendItemSpacing;
            private double _legendPadding;
            private double _itemHeight;

            // 空白比例区间配置（严格控制）
            private const double MinBlankRatio = 0.1;   // 大图最小空白比例
            private const double MaxBlankRatio = 0.2;   // 小图最大空白比例（强制锁定）
            private const double SmallLegendWidth = 200; // 小图阈值（px）
            private const double LargeLegendWidth = 400; // 大图阈值（px）
            private const double MinWidthBuffer = 20;    // 最小宽度仅比内容宽20px，避免强制放大

            // 新增：图名字体动态计算参数
            private const double ChartTitleScaleRatio = 0.025; // 图名占可用尺寸的比例（可微调）
            private const int MinChartTitleSize = 12;          // 最小图名字体
            private const int MaxChartTitleSize = 24;          // 最大图名字体
            private double _chartTitleFontSize;                // 动态计算的图名字体大小


            public DrawBase(double offsetX, double offsetY, List<double> x1, List<double> y1,
                           List<double> x2, List<double> y2, List<double> x3, List<double> y3,
                           string[] dataNames, int dataQuantity, double scaleX, double scaleY,
                           double xAvailableWidth, double yAvailableHeight,
                           double yExtensionRatio, double yAxisIntervalBase,
                           double legendBaseWidth, double xTotalExtend, double lastDataPointX,
                           double dataMaxX, double lastTickX, int minXAxisTicks, int maxXAxisTicks,
                           double initialGlobalMaxY, double yAxisMaxTick,
                           DrawDataSet dataSet, double xInterval, double yInterval,
                           double titleToYAxisRatio, int titleMinSize, int titleMaxSize)
            {
                _offsetX = offsetX;
                _offsetY = offsetY;
                _x1 = x1;
                _y1 = y1;
                _x2 = x2;
                _y2 = y2;
                _x3 = x3;
                _y3 = y3;
                _dataNames = dataNames;
                _dataQuantity = dataQuantity;
                _scaleX = scaleX;
                _scaleY = scaleY;
                _xAvailableWidth = xAvailableWidth;
                _yAvailableHeight = yAvailableHeight;
                _yExtensionRatio = yExtensionRatio;
                _yAxisIntervalBase = yAxisIntervalBase;
                _legendBaseWidth = legendBaseWidth;
                _xTotalExtend = xTotalExtend;
                _lastDataPointX = lastDataPointX;
                _dataMaxX = dataMaxX;
                _lastTickX = lastTickX;
                _minXAxisTicks = minXAxisTicks;
                _maxXAxisTicks = maxXAxisTicks;
                _initialGlobalMaxY = initialGlobalMaxY;
                _yAxisMaxTick = yAxisMaxTick;
                _dataSet = dataSet;
                _xInterval = xInterval;
                _yInterval = yInterval;
                _titleToYAxisRatio = titleToYAxisRatio;
                _titleMinSize = titleMinSize;
                _titleMaxSize = titleMaxSize;
                _occupiedAreas = new List<Rect>();
                _lineSegments = new List<LineSegment>();

                // 计算刻度字体大小（基准）
                double baseDimension = Math.Min(xAvailableWidth, yAvailableHeight);
                _dynamicFontSize = Math.Round(baseDimension * _fontScaleRatio);
                _dynamicFontSize = Math.Clamp(_dynamicFontSize, _minDynamicFontSize, _maxDynamicFontSize);

                // 新增：计算图名字体大小（基于可用宽高的最小值）
                double titleBaseDimension = Math.Min(_xAvailableWidth, _yAvailableHeight);
                _chartTitleFontSize = Math.Round(titleBaseDimension * ChartTitleScaleRatio);
                _chartTitleFontSize = Math.Clamp(_chartTitleFontSize, MinChartTitleSize, MaxChartTitleSize);
            }

            // 绘制图例（严格控制空白比例）
            private void DrawAllLegends(Canvas canvas)
            {
                _legendScaleFactor = Math.Min(
                    _xAvailableWidth * _legendBaseScale,
                    _yAvailableHeight * _legendBaseScale
                );
                _legendScaleFactor = Math.Max(_legendScaleFactor, 0.5);

                double[] textWidths = MeasureAllLegendTextWidths();
                double maxTextWidth = textWidths.Max();

                CalculateLegendDimensions(maxTextWidth);

                (double legendX, double legendY) = CalculateLegendPosition(canvas);

                var legendBackground = new Rectangle
                {
                    Width = _legendWidth,
                    Height = _legendHeight,
                    Fill = new SolidColorBrush(Color.FromArgb(220, 255, 255, 255)),
                    Stroke = Brushes.LightGray,
                    StrokeThickness = _legendScaleFactor * 2,
                    Margin = new Thickness(legendX, legendY, 0, 0)
                };
                canvas.Children.Add(legendBackground);

                for (int i = 0; i < _dataQuantity; i++)
                {
                    var color = GetLineColor(i);
                    double itemY = legendY + _legendPadding + i * _itemHeight;

                    // 图例图块
                    canvas.Children.Add(new Rectangle
                    {
                        Width = _legendIconSize,
                        Height = _legendIconSize,
                        Fill = color,
                        Margin = new Thickness(legendX + _legendPadding, itemY, 0, 0)
                    });

                    // 图例文本
                    string text = _dataNames.Length > i ? _dataNames[i] : $"系列{i + 1}";
                    int fontSize = (int)Math.Round(_legendIconSize * _fontToIconRatio);
                    fontSize = Math.Min(fontSize, (int)_dynamicFontSize); // 不超过刻度字体
                    fontSize = Math.Clamp(fontSize, 7, 14);

                    TextBlock measureBlock = new TextBlock { Text = text, FontSize = fontSize };
                    measureBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double actualTextWidth = measureBlock.DesiredSize.Width;
                    double textMaxWidth = maxTextWidth * 1.05; // 与测量缓冲一致

                    canvas.Children.Add(new TextBlock
                    {
                        Text = text,
                        FontSize = fontSize,
                        Foreground = Brushes.Black,
                        Margin = new Thickness(
                            legendX + _legendPadding + _legendIconSize + _legendItemSpacing,
                            itemY + (_legendIconSize - fontSize) / 2,
                            0, 0
                        ),
                        MaxWidth = textMaxWidth,
                        TextTrimming = actualTextWidth > textMaxWidth ? TextTrimming.CharacterEllipsis : TextTrimming.None
                    });
                }
            }

            // 收紧文本宽度缓冲，减少小图时的宽度虚高
            private double[] MeasureAllLegendTextWidths()
            {
                List<double> widths = new List<double>();
                double tempIconSize = _legendScaleFactor * 50;
                tempIconSize = Math.Clamp(tempIconSize, 12, 40);
                double tempFontSize = Math.Round(tempIconSize * _fontToIconRatio);
                tempFontSize = Math.Min(tempFontSize, _dynamicFontSize);
                tempFontSize = Math.Clamp(tempFontSize, 7, 14);

                TextBlock measureBlock = new TextBlock { FontSize = tempFontSize };
                for (int i = 0; i < _dataQuantity; i++)
                {
                    string text = _dataNames.Length > i ? _dataNames[i] : $"系列{i + 1}";
                    measureBlock.Text = text;
                    measureBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    // 缓冲从1.1→1.05，更精准
                    double textWidth = measureBlock.DesiredSize.Width * 1.05;
                    widths.Add(textWidth);
                }
                return widths.ToArray();
            }

            // 严格控制小图空白比例，动态调整最小宽度
            private void CalculateLegendDimensions(double maxTextWidth)
            {
                // 内边距：小图适中，大图收紧
                _legendPadding = _legendScaleFactor * 10;
                if (_legendScaleFactor < 0.8) // 小图判断
                    _legendPadding = Math.Clamp(_legendPadding, 6, 15);
                else
                    _legendPadding = Math.Clamp(_legendPadding, 5, 12);

                _legendIconSize = _legendScaleFactor * 50;
                _legendIconSize = Math.Clamp(_legendIconSize, 12, 40);

                // 间距收紧：从0.5→0.4，减少冗余
                _legendItemSpacing = _legendIconSize * 0.4;
                _legendItemSpacing = Math.Clamp(_legendItemSpacing, 4, 15);

                // 1. 精确计算内容宽度（图块+间距+文本）
                double contentWidth = _legendIconSize + _legendItemSpacing + maxTextWidth;
                // 2. 左右内边距总和
                double totalPadding = _legendPadding * 2;
                // 3. 理论最小宽度（无空白时的宽度）
                double theoreticalMinWidth = contentWidth + totalPadding;

                // 4. 动态计算空白比例（严格锁定在10%-20%）
                double rightBlankRatio;
                if (theoreticalMinWidth <= SmallLegendWidth)
                {
                    // 小图：强制20%比例
                    rightBlankRatio = MaxBlankRatio;
                }
                else if (theoreticalMinWidth >= LargeLegendWidth)
                {
                    // 大图：10%比例
                    rightBlankRatio = MinBlankRatio;
                }
                else
                {
                    // 中间大小：线性过渡
                    double ratio = (theoreticalMinWidth - SmallLegendWidth) / (LargeLegendWidth - SmallLegendWidth);
                    rightBlankRatio = MaxBlankRatio - (MaxBlankRatio - MinBlankRatio) * ratio;
                }

                // 5. 按比例计算总宽度
                double calculatedWidth = theoreticalMinWidth / (1 - rightBlankRatio);

                // 6. 动态最小宽度：仅比理论最小宽度宽20px，避免强制放大
                double dynamicMinWidth = theoreticalMinWidth + MinWidthBuffer;
                _legendWidth = Math.Min(calculatedWidth, _xAvailableWidth * _legendMaxWidthRatio);
                _legendWidth = Math.Max(_legendWidth, dynamicMinWidth);

                // 7. 最终校验：确保空白比例不超过20%
                double actualBlankWidth = _legendWidth - theoreticalMinWidth;
                double actualBlankRatio = actualBlankWidth / _legendWidth;
                if (actualBlankRatio > MaxBlankRatio)
                {
                    _legendWidth = theoreticalMinWidth / (1 - MaxBlankRatio);
                }

                // 项高收紧
                _itemHeight = _legendIconSize + _legendPadding;
                _itemHeight = Math.Clamp(_itemHeight, 18, 50);

                // 总高度计算
                _legendHeight = _legendPadding * 2 + _dataQuantity * _itemHeight;
                _legendHeight = Math.Min(_legendHeight, _yAvailableHeight * _legendMaxHeightRatio);
                _legendHeight = Math.Max(_legendHeight, 50);
            }

            private (double x, double y) CalculateLegendPosition(Canvas canvas)
            {
                double yAxisTop = _offsetY - _yAvailableHeight;
                double axisLeft = _offsetX;
                double axisRight = _offsetX + _xAvailableWidth;
                double axisBottom = _offsetY;

                double margin = _legendScaleFactor * 20;
                margin = Math.Clamp(margin, 10, 50);

                double x = axisRight - _legendWidth - margin;
                double y = yAxisTop + margin;

                if (x < 0) x = axisLeft + margin;
                if (y + _legendHeight > axisBottom) y = axisBottom - _legendHeight - margin;

                return (x, y);
            }

            private int CalculateSuitableFontSize(string text, double maxWidth, int baseSize)
            {
                if (maxWidth <= 0) return _minDynamicFontSize;

                for (int size = baseSize; size >= _minDynamicFontSize; size--)
                {
                    TextBlock measureBlock = new TextBlock { Text = text, FontSize = size };
                    measureBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    if (measureBlock.DesiredSize.Width <= maxWidth)
                    {
                        return size;
                    }
                }
                return _minDynamicFontSize;
            }

            public void DrawZXT(Canvas canvas, int index, string[] currentCode)
            {
                if (!ValidateData())
                {
                    MessageBox.Show("数据格式错误，X、Y数据数量不匹配");
                    return;
                }

                var (minX, maxX, minY, maxY) = GetDataRange();
                maxY = _initialGlobalMaxY;
                DrawAxis(canvas, maxX, maxY);
                DrawLineAndPoints(canvas, maxX, maxY);
                DrawAxisLabels(canvas, minX, maxX, minY, maxY);
                AddAxisTitles(canvas, maxX, maxY, currentCode);
            }

            private bool ValidateData()
            {
                return _x1 != null && _y1 != null && _x1.Count == _y1.Count
                       && _x2 != null && _y2 != null && _x2.Count == _y2.Count
                       && _x3 != null && _y3 != null && _x3.Count == _y3.Count;
            }

            private (double minX, double maxX, double minY, double maxY) GetDataRange()
            {
                var minX = 0.0;
                var maxX = double.MinValue;
                var minY = 0.0;
                var maxY = double.MinValue;

                UpdateRange(_x1, _y1, ref minX, ref maxX, ref minY, ref maxY);
                UpdateRange(_x2, _y2, ref minX, ref maxX, ref minY, ref maxY);
                UpdateRange(_x3, _y3, ref minX, ref maxX, ref minY, ref maxY);

                return (minX, maxX, minY, maxY);
            }

            private void UpdateRange(List<double> xList, List<double> yList,
                                    ref double minX, ref double maxX, ref double minY, ref double maxY)
            {
                for (var i = 0; i < xList.Count; i++)
                {
                    maxX = Math.Max(maxX, xList[i]);
                    maxY = Math.Max(maxY, yList[i]);
                }
            }

            private void DrawAxis(Canvas canvas, double maxX, double maxY)
            {
                double yAxisTop = _offsetY - _yAvailableHeight;
                double xAxisRight = _offsetX + _xAvailableWidth;

                canvas.Children.Add(new Polygon
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Points = new PointCollection
                    {
                        new(_offsetX , _offsetY),
                        new(xAxisRight, _offsetY ),
                        new(xAxisRight, yAxisTop),
                        new(_offsetX , yAxisTop),
                    }
                });
            }

            private void DrawLineAndPoints(Canvas canvas, double maxX, double maxY)
            {
                double[][] splineX = new double[][] { _x1.ToArray(), _x2.ToArray(), _x3.ToArray() };
                double[][] splineY = new double[][] { _y1.ToArray(), _y2.ToArray(), _y3.ToArray() };

                double[][] originalX = new double[][] {
                    _dataSet.OriginalX1.ToArray(),
                    _dataSet.OriginalX2.ToArray(),
                    _dataSet.OriginalX3.ToArray()
                };
                double[][] originalY = new double[][] {
                    _dataSet.OriginalY1.ToArray(),
                    _dataSet.OriginalY2.ToArray(),
                    _dataSet.OriginalY3.ToArray()
                };

                _occupiedAreas.Clear();
                _lineSegments.Clear();

                for (var i = 0; i < _dataQuantity; i++)
                {
                    var color = GetLineColor(i);

                    for (var j = 0; j < splineX[i].Length - 1; j++)
                    {
                        double scaledX1 = _offsetX + splineX[i][j] * _scaleX;
                        double scaledY1 = _offsetY - splineY[i][j] * _scaleY;
                        double scaledX2 = _offsetX + splineX[i][j + 1] * _scaleX;
                        double scaledY2 = _offsetY - splineY[i][j + 1] * _scaleY;

                        _lineSegments.Add(new LineSegment
                        {
                            X1 = scaledX1,
                            Y1 = scaledY1,
                            X2 = scaledX2,
                            Y2 = scaledY2
                        });

                        canvas.Children.Add(new Line
                        {
                            X1 = scaledX1,
                            Y1 = scaledY1,
                            X2 = scaledX2,
                            Y2 = scaledY2,
                            Stroke = color,
                            StrokeThickness = 2
                        });
                    }

                    if (originalX[i].Length == 0 || originalY[i].Length == 0)
                        continue;

                    double maxYVal = originalY[i].Max();
                    int maxYIdx = Array.IndexOf(originalY[i], maxYVal);

                    for (var j = 0; j < originalX[i].Length; j++)
                    {
                        double scaledX = _offsetX + originalX[i][j] * _scaleX;
                        double scaledY = _offsetY - originalY[i][j] * _scaleY;

                        canvas.Children.Add(new Ellipse
                        {
                            Width = 6,
                            Height = 6,
                            Fill = color,
                            Margin = new Thickness(scaledX - 3, scaledY - 3, 0, 0)
                        });

                        if (j == maxYIdx)
                        {
                            DrawCoordinateLabels(canvas, scaledX, scaledY, maxYVal,
                                               i, j, color, _scaleX, _scaleY, originalX, originalY);
                        }
                    }
                }

                if (_dataQuantity > 1)
                    DrawAllLegends(canvas);
            }

            private void DrawCoordinateLabels(Canvas canvas, double pointX, double pointY,
                                double yValue, int seriesIndex, int pointIndex,
                                Brush lineColor, double xScale, double yScale,
                                double[][] xData, double[][] yData)
            {
                if (yValue == 0)
                    return;

                string format = GetNumberFormat(yValue, yValue);
                string labelText = yValue.ToString(format);
                TextBlock testLabel = new TextBlock { Text = labelText, FontSize = _dynamicFontSize };
                testLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double labelWidth = testLabel.DesiredSize.Width;
                double labelHeight = testLabel.DesiredSize.Height;

                List<(double x, double y)> offsetDirections = new List<(double x, double y)>
                {
                    (10, -10),
                    (-labelWidth - 10, -10),
                    (10, 5),
                    (-labelWidth - 10, 5)
                };

                (double offsetX, double offsetY) = FindSafeOffset(canvas, pointX, pointY,
                                                       labelWidth, labelHeight,
                                                       seriesIndex, pointIndex,
                                                       xData, yData, xScale, yScale, offsetDirections);

                Rect labelRect = new Rect(pointX + offsetX, pointY + offsetY, labelWidth, labelHeight);
                _occupiedAreas.Add(labelRect);

                canvas.Children.Add(new TextBlock
                {
                    Text = labelText,
                    FontSize = _dynamicFontSize,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(pointX + offsetX, pointY + offsetY, 0, 0)
                });
            }

            private (double x, double y) FindSafeOffset(Canvas canvas, double pointX, double pointY,
                                             double labelWidth, double labelHeight,
                                             int seriesIndex, int pointIndex,
                                             double[][] xData, double[][] yData,
                                             double xScale, double yScale,
                                             List<(double x, double y)> directions)
            {
                foreach (var dir in directions)
                {
                    double offsetX = dir.x;
                    double offsetY = dir.y;
                    Rect labelRect = new Rect(pointX + offsetX, pointY + offsetY, labelWidth, labelHeight);

                    if (IsLabelInCanvas(canvas, labelRect) &&
                        !CollidesWithLines(labelRect, seriesIndex, pointIndex, xData, yData, xScale, yScale))
                    {
                        return (offsetX, offsetY);
                    }
                }
                return (10, -10);
            }

            private bool IsLabelInCanvas(Canvas canvas, Rect labelRect)
            {
                return labelRect.Left >= 10 && labelRect.Top >= 10 &&
                       labelRect.Right <= canvas.ActualWidth - 10 && labelRect.Bottom <= canvas.ActualHeight - 10;
            }

            private bool CollidesWithLines(Rect labelRect, int seriesIndex, int pointIndex,
                                    double[][] xData, double[][] yData,
                                    double xScale, double yScale)
            {
                if (pointIndex > 0)
                {
                    double prevX = _offsetX + xData[seriesIndex][pointIndex - 1] * xScale;
                    double prevY = _offsetY - yData[seriesIndex][pointIndex - 1] * yScale;
                    double currX = _offsetX + xData[seriesIndex][pointIndex] * xScale;
                    double currY = _offsetY - yData[seriesIndex][pointIndex] * yScale;

                    if (IsLineIntersectsRect(labelRect, prevX, prevY, currX, currY))
                        return true;
                }

                if (pointIndex < xData[seriesIndex].Length - 1)
                {
                    double currX = _offsetX + xData[seriesIndex][pointIndex] * xScale;
                    double currY = _offsetY - yData[seriesIndex][pointIndex] * yScale;
                    double nextX = _offsetX + xData[seriesIndex][pointIndex + 1] * xScale;
                    double nextY = _offsetY - yData[seriesIndex][pointIndex + 1] * yScale;

                    if (IsLineIntersectsRect(labelRect, currX, currY, nextX, nextY))
                        return true;
                }

                return false;
            }

            private bool IsLineIntersectsRect(Rect rect, double x1, double y1, double x2, double y2)
            {
                if (rect.Contains(x1, y1) || rect.Contains(x2, y2))
                    return true;

                var edges = new List<(double x1, double y1, double x2, double y2)>
                {
                    (rect.Left, rect.Top, rect.Right, rect.Top),
                    (rect.Right, rect.Top, rect.Right, rect.Bottom),
                    (rect.Left, rect.Bottom, rect.Right, rect.Bottom),
                    (rect.Left, rect.Top, rect.Left, rect.Bottom)
                };

                foreach (var edge in edges)
                {
                    if (SegmentsIntersect(x1, y1, x2, y2, edge.x1, edge.y1, edge.x2, edge.y2))
                        return true;
                }

                return false;
            }

            private bool SegmentsIntersect(double x1, double y1, double x2, double y2,
                                         double x3, double y3, double x4, double y4)
            {
                double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
                if (d == 0) return false;

                double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / d;
                double u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / d;

                return t >= 0 && t <= 1 && u >= 0 && u <= 1;
            }

            private Brush GetLineColor(int index)
            {
                return index switch
                {
                    0 => Brushes.Blue,
                    1 => Brushes.Green,
                    2 => Brushes.SandyBrown,
                    _ => Brushes.RosyBrown
                };
            }

            private void DrawAxisLabels(Canvas canvas, double minX, double maxX, double minY, double maxY)
            {
                double yAxisTop = _offsetY - _yAvailableHeight;
                DrawXAxisLabels(canvas, minX, maxX, yAxisTop);
                DrawYAxisLabels(canvas, minY, maxY, yAxisTop);
            }

            private void DrawXAxisLabels(Canvas canvas, double minX, double maxX, double yAxisTop)
            {
                double xAxisEnd = _offsetX + _xAvailableWidth;

                double xInterval = _xInterval;
                double xMaxTick = _lastTickX;

                for (int i = 0; i <= (int)Math.Ceiling(xMaxTick / xInterval); i++)
                {
                    double xVal = i * xInterval;

                    if (xVal > xMaxTick + 1e-9) break;

                    double canvasX = _offsetX + xVal * _scaleX;

                    canvas.Children.Add(new Line
                    {
                        X1 = canvasX,
                        Y1 = _offsetY,
                        X2 = canvasX,
                        Y2 = _offsetY - _tickLength,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    });

                    var xGridLine = new Line
                    {
                        X1 = canvasX,
                        Y1 = _offsetY - _tickLength,
                        X2 = canvasX,
                        Y2 = yAxisTop,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection { 2, 2 }
                    };
                    Canvas.SetZIndex(xGridLine, -50);
                    canvas.Children.Add(xGridLine);

                    string format = GetNumberFormat(xVal, xMaxTick);
                    canvas.Children.Add(new TextBlock
                    {
                        Text = xVal.ToString(format),
                        FontSize = _dynamicFontSize,
                        Foreground = Brushes.Black,
                        Margin = new Thickness(canvasX - 30, _offsetY + _tickLength + 5, 0, 0),
                        Width = 60,
                        TextAlignment = TextAlignment.Center
                    });

                    canvas.Children.Add(new Line
                    {
                        X1 = canvasX,
                        Y1 = yAxisTop,
                        X2 = canvasX,
                        Y2 = yAxisTop + _tickLength,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    });
                }
            }

            private void DrawYAxisLabels(Canvas canvas, double minY, double maxY, double yAxisTop)
            {
                double yMaxTick = _yAxisMaxTick;
                double yAxisRight = _offsetX + _xAvailableWidth;

                double yInterval = _yInterval;

                for (int i = 1; i <= (int)Math.Ceiling(yMaxTick / yInterval); i++)
                {
                    double yVal = i * yInterval;

                    if (yVal > yMaxTick + 1e-9) break;

                    double canvasY = _offsetY - yVal * _scaleY;

                    if (canvasY < yAxisTop - _axisEndPadding)
                        continue;

                    canvas.Children.Add(new Line
                    {
                        X1 = _offsetX,
                        Y1 = canvasY,
                        X2 = _offsetX + _tickLength,
                        Y2 = canvasY,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    });

                    var yGridLine = new Line
                    {
                        X1 = _offsetX + _tickLength,
                        X2 = yAxisRight,
                        Y1 = canvasY,
                        Y2 = canvasY,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection { 2, 2 }
                    };
                    Canvas.SetZIndex(yGridLine, -50);
                    canvas.Children.Add(yGridLine);

                    string format = GetNumberFormat(yVal, yMaxTick);
                    canvas.Children.Add(new TextBlock
                    {
                        Text = yVal.ToString(format),
                        FontSize = _dynamicFontSize,
                        Foreground = Brushes.Black,
                        Margin = new Thickness(_offsetX - _tickLength - 70, canvasY - 8, 0, 0),
                        Width = 70,
                        TextAlignment = TextAlignment.Right
                    });

                    canvas.Children.Add(new Line
                    {
                        X1 = yAxisRight,
                        Y1 = canvasY,
                        X2 = yAxisRight - _tickLength,
                        Y2 = canvasY,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    });
                }
            }

            private string GetNumberFormat(double value, double axisMax)
            {
                if (axisMax <= 0)
                    return "F0";

                if (axisMax >= 1000)
                    return "F0";
                else if (axisMax >= 100)
                    return "F0";
                else if (axisMax >= 10)
                    return "F0";
                else if (axisMax >= 1)
                    return "F1";
                else if (axisMax >= 0.1)
                    return "F2";
                else
                    return "F3";
            }

            private int CalculateAxisTitleFontSize(string text)
            {
                if (string.IsNullOrEmpty(text) || _yAvailableHeight <= 0)
                    return _titleMinSize;

                double baseSize = _yAvailableHeight * _titleToYAxisRatio;

                int candidateSize = (int)Math.Round(Math.Clamp(baseSize, _titleMinSize, _titleMaxSize));

                for (int size = candidateSize; size >= _titleMinSize; size--)
                {
                    TextBlock measureBlock = new TextBlock { Text = text, FontSize = size };
                    measureBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    if (measureBlock.DesiredSize.Width <= _yAvailableHeight * 0.8)
                    {
                        return size;
                    }
                }

                return _titleMinSize;
            }

            private void AddAxisTitles(Canvas canvas, double maxX, double maxY, string[] currentCode)
            {
                // 图名（图表标题）- 使用动态计算的字体大小
                string chartTitle = currentCode.Length > 2 ? currentCode[2] : "折线图";
                TextBlock chartTitleBlock = new TextBlock
                {
                    Text = chartTitle,
                    FontSize = _chartTitleFontSize,  // 动态字体大小（12-24）
                    FontWeight = FontWeights.Bold
                };
                chartTitleBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double chartTitleWidth = chartTitleBlock.DesiredSize.Width;
                double chartTitleHeight = chartTitleBlock.DesiredSize.Height;

                // 图名位置计算
                double yAxisTop = _offsetY - _yAvailableHeight;
                double chartTitleY = yAxisTop - chartTitleHeight - 10;
                double chartTitleX = _offsetX + _xAvailableWidth / 2 - chartTitleWidth / 2;

                Canvas.SetLeft(chartTitleBlock, chartTitleX);
                Canvas.SetTop(chartTitleBlock, chartTitleY);
                canvas.Children.Add(chartTitleBlock);

                // X轴标题（保持不变）
                string xTitle = currentCode.Length > 0 ? currentCode[0] : "X轴";
                int titleFontSize = CalculateAxisTitleFontSize(xTitle);

                TextBlock xTitleBlock = new TextBlock
                {
                    Text = xTitle,
                    FontSize = titleFontSize,
                    FontWeight = FontWeights.Bold
                };
                xTitleBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double xTitleWidth = xTitleBlock.DesiredSize.Width;
                double xTitleHeight = xTitleBlock.DesiredSize.Height;

                double xTitleY = _offsetY + 12 + 20;
                if (xTitleY + xTitleHeight > canvas.ActualHeight)
                    xTitleY = canvas.ActualHeight - xTitleHeight - 5;
                double xTitleX = _offsetX + _xAvailableWidth / 2 - xTitleWidth / 2;
                Canvas.SetLeft(xTitleBlock, xTitleX);
                Canvas.SetTop(xTitleBlock, xTitleY);
                canvas.Children.Add(xTitleBlock);

                // Y轴标题（保持不变）
                string yTitle = currentCode.Length > 1 ? currentCode[1] : "Y轴";
                TextBlock yTitleBlock = new TextBlock
                {
                    Text = yTitle,
                    FontSize = titleFontSize,
                    FontWeight = FontWeights.Bold,
                    LayoutTransform = new RotateTransform(-90)
                };
                yTitleBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double yTitleRotatedWidth = yTitleBlock.DesiredSize.Height;
                double yTitleRotatedHeight = yTitleBlock.DesiredSize.Width;

                double yTitleX = _offsetX - _yTitleToTicksSpacing - yTitleRotatedWidth / 2;
                double yTitleY = (yAxisTop + _offsetY) / 2 - yTitleRotatedHeight / 2;

                if (yTitleY < 10) yTitleY = 10;
                if (yTitleY + yTitleRotatedHeight > canvas.ActualHeight)
                    yTitleY = canvas.ActualHeight - yTitleRotatedHeight - 10;

                Canvas.SetLeft(yTitleBlock, yTitleX);
                Canvas.SetTop(yTitleBlock, yTitleY);
                canvas.Children.Add(yTitleBlock);
            }
        }

        public struct LineSegment
        {
            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double X2 { get; set; }
            public double Y2 { get; set; }
        }
    }
}
