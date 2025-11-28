namespace CMS.Plcs.Test
{
    public class DirTest
    {
        [Fact]
        public void C()
        {
            var find = IsExistCode("code1", 3, "D:\\Data");
        }


        public static bool IsExistCode(string code, int day = 3, string baseDir = null)
        {
            // 1. 输入参数校验
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("代码标识不能为空", nameof(code));
            if (day <= 0)
                return false; // 天数为非正数时，直接返回不存在
            baseDir ??= Directory.GetCurrentDirectory(); // 默认当前工作目录，更合理

            // 2. 遍历指定天数（从当天开始，向前推移）
            for (int i = 0; i < day; i++)
            {
                // 计算当前迭代的日期（当天、前一天、前两天...）
                DateTime currentDate = DateTime.Now.AddDays(-i);
                string dateStr = currentDate.ToString("yyyy-MM-dd");
                string dirPath = Path.Combine(baseDir, dateStr);

                // 3. 检查目录是否存在，存在则遍历文件
                if (Directory.Exists(dirPath))
                {
                    // 只遍历文件名（而非全路径），减少字符串匹配范围（根据业务需求调整）
                    foreach (string file in Directory.EnumerateFiles(dirPath))
                    {
                        // 若业务逻辑是“文件名包含code”，用Path.GetFileName更精准
                        if (Path.GetFileName(file).Contains(code))
                        {
                            return true; // 找到后立即返回，无需继续遍历
                        }
                    }
                }
            }

            return false;
        }
    }
}