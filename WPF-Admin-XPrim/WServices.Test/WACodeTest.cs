namespace WServices.Test
{
    public class WACodeTest
    {
        private string MonthGetString(int month)
        {
            switch (month)
            {
                case 1: return "1";
                case 2: return "2";
                case 3: return "3";
                case 4: return "4";
                case 5: return "5";
                case 6: return "6";
                case 7: return "7";
                case 8: return "8";
                case 9: return "9";
                case 10: return "A";
                case 11: return "B";
                case 12: return "C";
            }

            return "1";
        }

        private string _serialCode = "0001";
        private int serialIndex = 1;

        private void SerialAdd()
        {
            serialIndex++;
            if (serialIndex == 10000)
            {
                return;
            }

            string temp = serialIndex.ToString();
            while (temp.Length < 4)
            {
                temp = "0" + temp;
            }

            _serialCode = temp;
        }

        [Fact]
        public void Test()
        {
            var time = DateTime.Now;

            var yearCode = time.ToString("yy");

            var monthCode = MonthGetString(time.Month);

            var dayCode = time.Day.ToString("00");
        }
    }
}