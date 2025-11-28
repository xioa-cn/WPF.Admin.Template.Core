using System.Management;
using System.Runtime.InteropServices;

namespace WPF.Admin.Service.Utils
{
    public class HardwareClockHelper
    {
        // Windows API：读取系统时间（但优先读硬件时钟）
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetSystemTime(ref SYSTEMTIME lpSystemTime);

        // Windows API：设置系统时间（此处仅用于读取，无需设置）
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool SetSystemTime(ref SYSTEMTIME lpSystemTime);

        // 系统时间结构体（对应CMOS硬件时钟格式）
        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort wYear;       // 年（4位，如2025）
            public ushort wMonth;      // 月（1-12）
            public ushort wDayOfWeek;  // 星期（0-6，0=周日）
            public ushort wDay;        // 日（1-31）
            public ushort wHour;       // 时（0-23，UTC时间）
            public ushort wMinute;     // 分（0-59）
            public ushort wSecond;     // 秒（0-59）
            public ushort wMilliseconds;// 毫秒（0-999）
        }
        
        
        /// <summary>
        /// 读取Windows主板CMOS硬件时钟（不受系统时间篡改影响）
        /// </summary>
        /// <returns>硬件时钟的本地时间</returns>
        public static DateTime GetHardwareLocalTime()
        {
            SYSTEMTIME sysTime = new SYSTEMTIME();
            // 调用API读取硬件时钟（返回UTC时间）
            bool success = GetSystemTime(ref sysTime);
            if (!success)
            {
                throw new InvalidOperationException("读取硬件时钟失败，可能缺少权限或硬件不支持");
            }

            // 转换为UTC时间，再转本地时间
            DateTime utcTime = new DateTime(
                sysTime.wYear, sysTime.wMonth, sysTime.wDay,
                sysTime.wHour, sysTime.wMinute, sysTime.wSecond, sysTime.wMilliseconds,
                DateTimeKind.Utc
            );
            return utcTime.ToLocalTime(); // 转为本地时间（与系统时区一致）
        }
    }
}