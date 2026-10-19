using LibVLCSharp.Shared;
using RVReducerDesignApp;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows;

public partial class UITANInsertSymmetryComponent : Window
{
    public bool IsAssignComponent = false;
    public int AssignPageID;
    public CTANBasicContentPageObj mAssignComponentPage;
    public CTANComboBox mComponentListComboBox = new CTANComboBox();
    CTANTextBox mAssembleDistanceBox = new CTANTextBox();
    CTANComboBox mAssembleFaceTypeBox = new CTANComboBox();
    CTANTextBox mYOffsetBox = new CTANTextBox();
    CTANComboBox mOrientationBox = new CTANComboBox();
    public CTANCrankShaftSystem AssignedShaftSystem;
    private Dictionary<string, int> refComponentListDict;

    private TextBlock tb3; //便于后续根据条件调整文本及可见性！

    public UITANInsertSymmetryComponent()
    {
        SetInput();
    }

    private void SetInput()
    {
        Title = "添加图元";
        Height = 200;
        Width = 300;
        Grid inputGrid = new Grid { Name = "InputGrid" };
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition());
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition());

        for (int i = 0; i < 5; i++)
        {
            inputGrid.RowDefinitions.Add(new RowDefinition());
        }

        TextBlock tb0 = new TextBlock { Text = "选择零部件:", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(tb0, 0);
        inputGrid.Children.Add(tb0);

        TextBlock tb1 = new TextBlock { Text = "距左基准距离（毫米）:", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(tb1, 1);
        inputGrid.Children.Add(tb1);

        TextBlock tb2 = new TextBlock { Text = "零部件定位面:", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(tb2, 2);
        inputGrid.Children.Add(tb2);

        tb3 = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(tb3, 3);
        inputGrid.Children.Add(tb3);

        CTANGrid.AssignControlToGrid(3, 1, mYOffsetBox, inputGrid);
        mYOffsetBox.VerticalAlignment = VerticalAlignment.Center;

        mOrientationBox.SetComboBoxWithKeyValue(
            new[] { CTANBearing.BearingExForceToRightNotation, CTANBearing.BearingExForceToLeftNotation },
            new[] { "正向", "反向" });
        CTANGrid.AssignControlToGrid(3, 1, mOrientationBox, inputGrid);
        mOrientationBox.VerticalAlignment = VerticalAlignment.Center;

        Button confirmButton = new Button { Content = "确定", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        confirmButton.Click += Button_Click;
        Grid.SetRow(confirmButton, 4);
        Grid.SetColumnSpan(confirmButton, 2);
        inputGrid.Children.Add(confirmButton);

        CTANGrid.AssignControlToGrid(0, 1, mComponentListComboBox, inputGrid);
        mComponentListComboBox.VerticalAlignment = VerticalAlignment.Center;
        mComponentListComboBox.SelectionChanged += ComponentListComboBox_SelectionChanged;

        CTANGrid.AssignControlToGrid(1, 1, mAssembleDistanceBox, inputGrid);
        mAssembleDistanceBox.VerticalAlignment = VerticalAlignment.Center;

        mAssembleFaceTypeBox.SetComboBoxWithKeyValue(
            new[] { CTANRVSet.AssembleLeftBaseNotation, CTANRVSet.AssembleMiddleBaseNotation, CTANRVSet.AssembleRightBaseNotation },
            new[] { "左端面", "中间点", "右端面" });
        CTANGrid.AssignControlToGrid(2, 1, mAssembleFaceTypeBox, inputGrid);
        mAssembleFaceTypeBox.VerticalAlignment = VerticalAlignment.Center;

        tb3.Visibility = Visibility.Collapsed;
        mYOffsetBox.Visibility = Visibility.Collapsed;
        mOrientationBox.Visibility = Visibility.Collapsed;

        Content = inputGrid;
    }

    private void ComponentListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (mComponentListComboBox.SelectedIndex < 0 || AssignedShaftSystem == null) return;

        bool hasSunGear = refComponentListDict != null && refComponentListDict.ContainsKey("太阳轮");   

        if (hasSunGear)
        {
            tb3.Text = "Y方向偏移:";
            tb3.Visibility = Visibility.Visible;
            mYOffsetBox.Visibility = Visibility.Visible;
            mOrientationBox.Visibility = Visibility.Collapsed;
        }
        else
        {          
            tb3.Text = "轴承朝向:";            
            mYOffsetBox.Visibility = Visibility.Collapsed;
            mOrientationBox.Visibility = Visibility.Visible;
            // 根据轴承类型判断是否显示
            var selectedBearing = AssignedShaftSystem.mBearingList[mComponentListComboBox.SelectedIndex];
            if (selectedBearing.mDataStorageDict["轴承类型"].Value == CTANBearing.TaperedRollerBearing)
            {
                tb3.Visibility = Visibility.Visible;
                mOrientationBox.Visibility = Visibility.Visible;
            }                
            else
            {
                tb3.Visibility = Visibility.Collapsed;
                mOrientationBox.Visibility = Visibility.Collapsed;
            }              
        }
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (mComponentListComboBox.SelectedItem != null)
        {
            IsAssignComponent = true;
            mAssignComponentPage = MainWindow.sPageIDDict[(int)mComponentListComboBox.SelectedValue];
            mAssignComponentPage.AssignToShaft(AssignedShaftSystem);

            bool hasSunGear = refComponentListDict != null && refComponentListDict.ContainsKey("太阳轮");
            if (hasSunGear)
            {
                mAssignComponentPage.mDataStorageDict["定位面"].Value = Convert.ToDouble(mAssembleFaceTypeBox.SelectedValue);
                mAssignComponentPage.mDataStorageDict["定位面距基准面距离"].Value = Convert.ToDouble(mAssembleDistanceBox.Text);
                // 检查 mYOffsetBox.Text 是否为空或无效，如果是则设为 0
                double yOffset = double.TryParse(mYOffsetBox.Text, out double parsedYOffset) ? parsedYOffset : 0;
                mAssignComponentPage.mDataStorageDict["Y方向偏移"].Value = yOffset;
                //mAssignComponentPage.mDataStorageDict["Y方向偏移"].Value = Convert.ToDouble(mYOffsetBox.Text);
            }
            else
            {
                mAssignComponentPage.mDataStorageDict["轴承定位面"].Value = Convert.ToDouble(mAssembleFaceTypeBox.SelectedValue);
                mAssignComponentPage.mDataStorageDict["轴承定位面距基准面距离"].Value = Convert.ToDouble(mAssembleDistanceBox.Text);
                mAssignComponentPage.mDataStorageDict["轴承朝向"].Value = Convert.ToDouble(mOrientationBox.SelectedValue);
            }
            Close();
        }
    }

    public void SetComponentListBox(int[] InComponentIDList, string[] InComponentName)
    {
        mComponentListComboBox.SetComboBoxWithKeyValue(InComponentIDList, InComponentName);
    }

    public void SetComponentListBox(Dictionary<string, int> refedDict)
    {
        refComponentListDict = refedDict;
        mComponentListComboBox.ItemsSource = refComponentListDict;
        mComponentListComboBox.SelectedValuePath = "Value";
        mComponentListComboBox.DisplayMemberPath = "Key";
    }
}
