using WPF.Admin.Service.Utils;

namespace WServices.Test
{
    public class TimeTest
    {
        [Fact]
        public void Test()
        {
            var time = HardwareClockHelper.GetHardwareLocalTime();
        }

        [Fact]
        public void Test1()
        {
          var time =  SystemTimeHelper.GetHistoryTime();
        }
    }
}