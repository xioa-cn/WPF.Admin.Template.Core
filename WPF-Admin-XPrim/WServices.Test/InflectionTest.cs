using WPF.Admin.Service.Utils;

namespace WServices.Test
{
    public class InflectionTest
    {
        
        [Fact]
        public void Test()
        {
            // 测试数据：非等间隔X轴，模拟趋势：上升→峰值→下降→谷值→上升（含凹凸性变化）
                var testData = new List<(double X, double Y)>
                {
                    (1.0, 1.2),  // 0
                    (2.3, 2.1),  // 1
                    (3.5, 3.5),  // 2
                    (4.2, 5.3),  // 3（凸转凹拐点）
                    (5.7, 7.8),  // 4
                    (6.1, 9.2),  // 5（峰值拐点）
                    (7.4, 8.5),  // 6
                    (8.2, 6.1),  // 7（凹转凸拐点）
                    (9.5, 4.2),  // 8
                    (10.1, 3.8), // 9（谷值拐点）
                    (11.3, 20.1), // 10
                    (12.7, 7.3), // 11（凸转凹拐点）
                    (13.2, 9.5), // 12
                    (14.5, 8.9), // 13
                    (15.1, 6.7)  // 14
                };

                Console.WriteLine("原始XY数据：");
                foreach (var (idx, data) in testData.Select((d, i) => (i, d)))
                {
                    Console.WriteLine($"索引{idx}：X={data.X:F2}, Y={data.Y:F2}");
                }
                Console.WriteLine("--------------------------------------------------");

                // 初始化检测器
                var detector = new InflectionPointDetector
                {
                    SmoothWindowSize = 3,
                    MinXDiffThreshold = 1e-6 // 可根据数据精度调整
                };

                // 1. 检测单调性拐点（峰值/谷值）
                var monotonicPoints = detector.DetectMonotonicInflectionPoints(testData);
                Console.WriteLine("单调性拐点（峰值/谷值）：");
                foreach (var point in monotonicPoints)
                {
                    Console.WriteLine(point);
                }
                Console.WriteLine("--------------------------------------------------");

                // 2. 检测凹凸性拐点（速率变化）
                var concavityPoints = detector.DetectConcavityInflectionPoints(testData);
                Console.WriteLine("凹凸性拐点（速率变化）：");
                foreach (var point in concavityPoints)
                {
                    Console.WriteLine(point);
                }
        }
    }
}