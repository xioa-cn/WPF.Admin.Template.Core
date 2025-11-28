using System.Windows.Controls;
using KeyenceCollectionModule.ViewModels;
using WPF.Admin.Models.Attributes;
using WPF.Admin.Models.Models;
using XPrism.Core.Attribute;

namespace KeyenceCollectionModule.Views
{
    /// <summary>
    /// HomeView.xaml 的交互逻辑
    /// </summary>
    [AutoRegisterView(nameof(RegionName.HomeRegion), "Home")]
    public partial class HomeView : Page
    {
        public HomeView()
        {
            InitializeComponent();
        }
    }
}