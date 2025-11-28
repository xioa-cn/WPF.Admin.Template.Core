namespace WPF.Admin.Themes.Helper
{
    public static class ColorHelper
    {
        /// <summary>
        /// 将 WPF Color 转换为 #RRGGBB 格式字符串（如 #1890ff）
        /// </summary>
        public static string ToHexString(this System.Windows.Media.Color color)
        {
            // 提取 R、G、B 分量（各占两位十六进制），忽略 Alpha 通道
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// 含 Alpha 通道的格式（#AARRGGBB，如 #FF1890ff），按需选择
        /// </summary>
        public static string ToHexStringWithAlpha(this System.Windows.Media.Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}