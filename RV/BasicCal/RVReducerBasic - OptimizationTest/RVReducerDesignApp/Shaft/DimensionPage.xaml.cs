using RVReducerDesignApp;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RWPF
{
    public partial class DimensionPage : Page
    {
        public DimensionPage()
        {
            InitializeComponent();
        }

        public void UpdateDimensions(IEnumerable<DimensionInfo> dimensions)
        {
            DimensionItemsControl.ItemsSource = dimensions;
        }
        //public void UpdateGearDimensions(IEnumerable<GearDimensionInfo> gearDimensions)
        //{
        //    DimensionItemsControl.ItemsSource = gearDimensions;         
        //}
        //public void UpdateBearingDimensions(IEnumerable<BearingDimensionInfo> bearingDimensions)
        //{
        //    DimensionItemsControl.ItemsSource = bearingDimensions;
        //}
    }

    public class DimensionInfo
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
    //public class GearDimensionInfo
    //{
    //    public string Label { get; set; }
    //    public string Value { get; set; }
    //}
    //public class BearingDimensionInfo
    //{
    //    public string Label { get; set; }
    //    public string Value { get; set; }
    //}
}
