using WPF.Admin.Service.Logger;

namespace WPF.Admin.Service.Utils
{
    public class SystemTimeHelper
    {
        public static bool GetHistoryTime()
        {
            var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GHis.dll");

            if (!System.IO.File.Exists(file))
            {
                System.IO.File.WriteAllText(file, DateTime.Now.ToLocalTime().ToString());
                return true;
            }

            try
            {
                var str = System.IO.File.ReadAllText(file);
                var time = DateTime.Parse(str);

                if (DateTime.Now >= time)
                {
                    System.IO.File.WriteAllText(file, DateTime.Now.ToLocalTime().ToString());
                    return true;
                }
            }
            catch (Exception e)
            {
                XLogGlobal.Logger?.LogError(e.Message, e);
            }
            
            return false;
        }
    }
}