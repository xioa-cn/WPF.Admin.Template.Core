using System.Windows;
using WPF.Admin.Models.Utils;
using WPF.Admin.Themes.Helper;

namespace WPFAdmin.AuthApplication;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CreateAuthCodeClick(object sender, RoutedEventArgs e)
    {
        var str = this.KeyTxt.Text.Replace("\r", "").Replace(" ", "");
        if (string.IsNullOrEmpty(str))
        {
            MessageBox.Show("版本号异常");
            return;
        }
        var authCode = TextCodeHelper.Encrypt(key: str);
        var code = TextCodeHelper.Decrypt(str);
        this.ValueTxt.Text = authCode;

        MessageBox.Show("Success");
    }

    private void CopyAuthCodeClick(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(this.ValueTxt.Text);
        MessageBox.Show("复制成功");
    }

    private void CreateTimeAuthCodeClick(object sender, RoutedEventArgs e)
    {
        var str = this.KeyTxt.Text.Replace("\r", "").Replace(" ", "");
        if (string.IsNullOrEmpty(str))
        {
            MessageBox.Show("版本号异常");
            return;
        }

        if (DateTime.TryParse(str, out var time))
        {
            var t = time.ToString("yyyyMMdd") + ".XA";
            var authCode = TextCodeHelper.Encrypt(t,"DT:----");
            var t1 = TextCodeHelper.Decrypt(authCode, "DT:----");


            this.ValueTxt.Text = authCode;

            MessageBox.Show("Success");
        }
        else
        {
            MessageBox.Show("时间异常异常");
            return;
        }
    }
}