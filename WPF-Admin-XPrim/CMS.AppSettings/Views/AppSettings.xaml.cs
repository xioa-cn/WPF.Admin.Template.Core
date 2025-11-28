using System.Windows.Controls;

namespace CMS.AppSettings.Views
{
    public partial class AppSettings : Page
    {
        public AppSettings()
        {
            HandyControl.Controls.Dialog.Register(
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogSettingsAuthToken, this);
            InitializeComponent();
        }
    }
}