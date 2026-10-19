using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using VtkBridge;
using System.Windows.Forms;
using RWPF;

namespace RVReducerDesignApp
{
    public partial class Test3DPage : Page
    {
        private VtkWrapper vtkWrapper;
        private DispatcherTimer renderTimer;
        private System.Windows.Forms.Panel vtkPanel;
        private List<DrawAxisDimension> axisDimensions;
        // 添加公共属性
        public List<DrawAxisDimension> AxisDimensions
        {
            get => axisDimensions;
            set => axisDimensions = value ?? throw new System.ArgumentNullException(nameof(value));
        }


        public Test3DPage()
        {
            InitializeComponent();
            InitializeRenderTimer();
            // 绑定 vtkHost 的 Loaded 事件（初始化时就绑定，避免动态订阅导致事件错过）
            vtkHost.Loaded += VtkHost_Loaded;

            this.Loaded += Test3DPage_Loaded;
            //this.Unloaded += Test3DPage_Unloaded;
        }
        private void VtkHost_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("vtkHost.Loaded 事件已触发");
            // 确保在 UI 线程操作
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => VtkHost_Loaded(sender, e));
                return;
            }

            // 初始化 VTK
            InitializeVtk();
        }
        private void InitializeRenderTimer()
        {
            renderTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            renderTimer.Tick += RenderTimer_Tick;
        }

        private void Test3DPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 确保在UI线程操作
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => Test3DPage_Loaded(sender, e));
                    return;
                }
                InitializeVtk();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"VTK初始化失败: {ex.ToString()}",
                    "严重错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        public void InitializeVtk()
        {
            Console.WriteLine("正在初始化 VTK...");
            try
            {
                Console.WriteLine($"当前 vtkWrapper 状态: {vtkWrapper != null}");
                // 检查是否已初始化
                if (vtkWrapper != null) return;

                // 创建 Panel 并强制创建句柄
                if (vtkPanel == null)
                {
                    vtkPanel = new System.Windows.Forms.Panel { Dock = DockStyle.Fill };
                    vtkHost.Child = vtkPanel;
                    // 强制创建句柄（关键步骤）
                    if (!vtkPanel.IsHandleCreated)
                    {
                        var unused = vtkPanel.Handle;
                        Console.WriteLine($"vtkPanel 句柄: {vtkPanel.Handle}");
                    }

                    // 确保句柄已创建
                    if (vtkPanel.Handle == IntPtr.Zero)
                    {
                        throw new InvalidOperationException("无法创建渲染面板句柄");
                    }
                }

                // 初始化 VTK 对象
                vtkWrapper = new VtkWrapper();
                vtkWrapper.Initialize(vtkPanel.Handle);
                vtkWrapper.RequestRender(); // 首次渲染
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"VTK 初始化失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                CleanupVtkResources(); // 清理资源
            }
        }
        private void CleanupVtkResources()
        {
            if (vtkWrapper != null)
            {
                vtkWrapper.Dispose();
                vtkWrapper = null;
            }
            if (vtkPanel != null)
            {
                vtkPanel.Dispose();
                vtkPanel = null;
            }
        }
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            // 在主线程触发渲染
            vtkWrapper?.RequestRender();
        }

        public void UpdateModelButton_Click()
        {
            try
            {
                if (vtkWrapper == null)
                {
                    // 确保在 UI 线程操作
                    if (!Dispatcher.CheckAccess())
                    {
                        Dispatcher.Invoke(() => UpdateModelButton_Click());
                        return;
                    }

                    // 强制更新布局并延迟检查
                    vtkHost.UpdateLayout();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (!vtkHost.IsLoaded)
                        {
                            System.Windows.MessageBox.Show("渲染容器未准备好，请稍后重试", "错误",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // 如果 vtkHost 已加载但未初始化，手动初始化
                        if (vtkWrapper == null)
                        {
                            InitializeVtk();
                        }

                        // 再次检查初始化是否成功
                        if (vtkWrapper == null)
                        {
                            System.Windows.MessageBox.Show("VTK 初始化失败，请检查系统配置", "错误",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // 继续执行模型更新逻辑
                        UpdateModel();
                    }), DispatcherPriority.ContextIdle);
                }
                else
                {
                    // 如果已初始化，直接更新模型
                    UpdateModel();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"模型更新失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateModel()
        {
            // 检查轴段数据
            if (axisDimensions == null || axisDimensions.Count == 0)
            {
                //System.Windows.MessageBox.Show("请先添加有效的轴段数据", "数据错误",
                //    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 预验证所有轴段参数
            foreach (var axis in axisDimensions)
            {
                if (axis.BaseInnerDiameter >= axis.BaseOuterDiameter)
                {
                    System.Windows.MessageBox.Show($"轴段{axis.BaseIndex}内径不能大于等于外径",
                        "参数错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            // 更新模型参数
            foreach (var axis in axisDimensions.OrderBy(a => a.BaseIndex))
            {
                vtkWrapper.UpdateParameters(
                    axis.BaseWidth,
                    axis.BaseInnerDiameter,
                    axis.BaseOuterDiameter,
                    axis.IsInnerTapered,
                    axis.IsOuterTapered,
                    axis.SmallEndInnerDiameter,
                    axis.SmallEndOuterDiameter,
                    axis.IsDirectionInnerLChecked,
                    axis.IsDirectionInnerRChecked,
                    axis.IsDirectionOuterLChecked,
                    axis.IsDirectionOuterRChecked,
                    axis.IsPeccentricshaft,
                    axis.EccentricDiameter,
                    axis.Phase,
                    axis.BaseIndex
                );
            }

            // 首次渲染后启动定时器
            if (!renderTimer.IsEnabled)
            {
                renderTimer.Start();
            }
        }

        private void Test3DPage_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 停止渲染定时器
                renderTimer.Stop();

                // 释放VTK资源
                if (vtkWrapper != null)
                {
                    vtkWrapper.Dispose();
                    vtkWrapper = null;
                }

                // 清空WindowsFormsHost
                vtkHost.Child = null;

                // 释放Panel资源
                if (vtkPanel != null)
                {
                    vtkPanel.Dispose();
                    vtkPanel = null;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"释放 VTK 资源失败!!!!: {ex.Message}");
            }
        }
    }

    public class PerfectShaft : Test3DPage
    {
        public PerfectShaft()
        {
        }
    }
}
