using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Service.Utils;
using WPF.Admin.Themes.Controls;
using WPF.Admin.Themes.Helper;

namespace WPFAdmin.NavigationModules.Utils
{
    public partial class ApplicationGlobalIcon : ObservableObject
    {
        private readonly string _iconPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon.svg");

        private static ApplicationGlobalIcon? _applicationGlobalIcon;

        public static ApplicationGlobalIcon Instance => _applicationGlobalIcon ??= new ApplicationGlobalIcon();

        [ObservableProperty] private BitmapSource? _icon;

        private string _logoSvgPath;

        private string _normalColor = "#163986";

        public ApplicationGlobalIcon()
        {
            if (System.IO.File.Exists(_iconPath))
            {
                _logoSvgPath = System.IO.File.ReadAllText(_iconPath);
            }
            else
            {
                try
                {
                    var resourceInfo =
                        ApplicationUtils.FindApplicationResourceStream("WPFAdmin.NavigationModules",
                            "Assets/Img/logo.svg");


                    // 从流中读取 SVG 内容为字符串
                    using (var reader = new StreamReader(resourceInfo))
                    {
                        _logoSvgPath = reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                }
            }


            IconColor();

            WeakReferenceMessenger.Default.Register<ChangeThemeEvent>(this, ChangeIconColor);
        }

        private void ChangeIconColor(object recipient, ChangeThemeEvent message)
        {
            IconColor();
        }

        private void IconColor()
        {
            if (Application.Current.FindResource("Primary.Color") is System.Windows.Media.Color primaryColor)
            {
                var color = primaryColor.ToHexString();

                _logoSvgPath = _logoSvgPath.Replace(_normalColor, color);
                _normalColor = color;
            }

            Icon = SvgHelper.ConvertSvgToBitmap(_logoSvgPath, 30, 30).ToBitmapSource();
        }
    }
}