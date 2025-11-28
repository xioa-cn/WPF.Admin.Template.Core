
namespace WPF.Admin.Service.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 数据拐点检测器（支持XY轴数据对，适配任意X轴类型）
    /// 支持：单调性拐点（趋势反转）、凹凸性拐点（速率变化）
    /// </summary>
    public class InflectionPointDetector
    {
        /// <summary>
        /// 移动平均窗口大小（用于数据平滑，默认3，窗口越大平滑效果越强）
        /// </summary>
        public int SmoothWindowSize { get; set; } = 3;

        /// <summary>
        /// 最小X差值阈值（避免Δx过小导致差分溢出，默认1e-6）
        /// </summary>
        public double MinXDiffThreshold { get; set; } = 1e-6;

        /// <summary>
        /// 相对阈值系数（基于数据标准差的自适应阈值，默认0.1）
        /// </summary>
        public double RelativeThresholdFactor { get; set; } = 0.1;

        /// <summary>
        /// 检测单调性拐点（趋势反转：增→减 或 减→增）
        /// </summary>
        /// <param name="data">XY轴数据对集合（需按X轴升序排列）</param>
        /// <returns>拐点列表（包含原始索引、X/Y值、拐点类型）</returns>
        public List<InflectionPoint> DetectMonotonicInflectionPoints(List<(double X, double Y)> data)
        {
            ValidateInputData(data, minCount: 3);

            // 1. 数据平滑（仅平滑Y值，X值保持不变）
            var smoothedData = SmoothData(data, SmoothWindowSize);

            // 2. 计算归一化一阶差分（Δy/Δx，反映单位X变化对应的Y变化率）
            var firstDifferences = CalculateNormalizedFirstDifferences(smoothedData);

            // 3. 计算自适应阈值
            double adaptiveThreshold = CalculateAdaptiveThreshold(data);

            // 4. 检测差分符号变化，识别拐点
            var inflectionPoints = new List<InflectionPoint>();
            for (int i = 0; i < firstDifferences.Count - 1; i++)
            {
                double prevDiff = firstDifferences[i].DiffValue;
                double currDiff = firstDifferences[i + 1].DiffValue;

                // 符号变化（排除零值和微小波动，避免无效拐点）
                if (prevDiff * currDiff < 0 && 
                    Math.Abs(prevDiff) > adaptiveThreshold && 
                    Math.Abs(currDiff) > adaptiveThreshold)
                {
                    // 修正：拐点应该映射到 smoothedData[i] 和 smoothedData[i+1] 之间的点
                    // 选择变化更明显的点作为拐点
                    var targetIndex = Math.Abs(prevDiff) > Math.Abs(currDiff) ? i : i + 1;
                    var targetData = smoothedData[targetIndex];
                    
                    inflectionPoints.Add(new InflectionPoint
                    {
                        OriginalIndex = targetData.OriginalIndex,
                        X = targetData.X,
                        Y = targetData.Y,
                        Type = prevDiff > 0 ? InflectionPointType.Peak : InflectionPointType.Valley
                    });
                }
            }

            return inflectionPoints;
        }

        /// <summary>
        /// 检测凹凸性拐点（速率变化：凹→凸 或 凸→凹）
        /// </summary>
        /// <param name="data">XY轴数据对集合（需按X轴升序排列）</param>
        /// <returns>拐点列表（包含原始索引、X/Y值、拐点类型）</returns>
        public List<InflectionPoint> DetectConcavityInflectionPoints(List<(double X, double Y)> data)
        {
            ValidateInputData(data, minCount: 4);

            // 1. 数据平滑（仅平滑Y值，X值保持不变）
            var smoothedData = SmoothData(data, SmoothWindowSize);

            // 2. 计算归一化一阶差分（Δy/Δx）
            var firstDifferences = CalculateNormalizedFirstDifferences(smoothedData);

            // 3. 计算二阶差分（基于一阶差分的变化率，Δ(Δy/Δx)/Δx_mid，Δx_mid为两个相邻X间隔的平均值）
            var secondDifferences = CalculateNormalizedSecondDifferences(firstDifferences);

            // 4. 计算自适应阈值
            double adaptiveThreshold = CalculateAdaptiveThreshold(data);

            // 5. 检测二阶差分符号变化，识别拐点
            var inflectionPoints = new List<InflectionPoint>();
            for (int i = 0; i < secondDifferences.Count - 1; i++)
            {
                double prevDiff2 = secondDifferences[i].DiffValue;
                double currDiff2 = secondDifferences[i + 1].DiffValue;

                // 符号变化（排除零值和微小波动）
                if (prevDiff2 * currDiff2 < 0 && 
                    Math.Abs(prevDiff2) > adaptiveThreshold && 
                    Math.Abs(currDiff2) > adaptiveThreshold)
                {
                    // 修正：二阶差分符号变化对应 smoothedData[i+1]
                    // 因为二阶差分是相邻一阶差分之间的变化，对应原始数据中的中间点
                    var targetData = smoothedData[i + 1];
                    inflectionPoints.Add(new InflectionPoint
                    {
                        OriginalIndex = targetData.OriginalIndex,
                        X = targetData.X,
                        Y = targetData.Y,
                        Type = prevDiff2 > 0 
                            ? InflectionPointType.ConcaveToConvex 
                            : InflectionPointType.ConvexToConcave
                    });
                }
            }

            return inflectionPoints;
        }

        /// <summary>
        /// 检测所有类型的拐点
        /// </summary>
        public List<InflectionPoint> DetectAllInflectionPoints(List<(double X, double Y)> data)
        {
            var monotonicPoints = DetectMonotonicInflectionPoints(data);
            var concavityPoints = DetectConcavityInflectionPoints(data);
            
            return monotonicPoints.Concat(concavityPoints)
                .OrderBy(p => p.OriginalIndex)
                .ToList();
        }

        #region 私有辅助方法
        /// <summary>
        /// 验证输入数据合法性
        /// </summary>
        private void ValidateInputData(List<(double X, double Y)> data, int minCount)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "数据集合不能为空");
            if (data.Count < minCount)
                throw new ArgumentException($"数据长度至少为{minCount}才能检测拐点", nameof(data));
            if (data.DistinctBy(d => d.X).Count() != data.Count)
                throw new ArgumentException("X轴数据不能重复", nameof(data));
            // 验证X轴是否升序（差分计算依赖X轴顺序）
            for (int i = 0; i < data.Count - 1; i++)
            {
                if (data[i + 1].X - data[i].X <= MinXDiffThreshold)
                    throw new ArgumentException($"X轴数据需按升序排列，且相邻X差值不能小于{MinXDiffThreshold}", nameof(data));
            }
        }

        /// <summary>
        /// 计算自适应阈值
        /// </summary>
        private double CalculateAdaptiveThreshold(List<(double X, double Y)> data)
        {
            if (data.Count < 2) return MinXDiffThreshold;
            
            var yValues = data.Select(d => d.Y).ToList();
            double mean = yValues.Average();
            double stdDev = Math.Sqrt(yValues.Sum(y => Math.Pow(y - mean, 2)) / yValues.Count);
            
            return Math.Max(MinXDiffThreshold, stdDev * RelativeThresholdFactor);
        }

        /// <summary>
        /// 移动平均平滑数据（改进版边界处理）
        /// </summary>
        private List<(int OriginalIndex, double X, double Y)> SmoothData(List<(double X, double Y)> data, int windowSize)
        {
            // 预处理窗口大小
            if (windowSize < 1) windowSize = 1;
            if (windowSize > data.Count) windowSize = data.Count;
            
            int halfWindow = windowSize / 2;
            var smoothed = new List<(int OriginalIndex, double X, double Y)>(data.Count);

            // 为原始数据添加索引
            var dataWithIndex = data.Select((d, idx) => (OriginalIndex: idx, d.X, d.Y)).ToList();

            // 改进的平滑算法：统一的边界处理
            for (int i = 0; i < dataWithIndex.Count; i++)
            {
                int start = Math.Max(0, i - halfWindow);
                int end = Math.Min(dataWithIndex.Count - 1, i + halfWindow);
                
                var windowData = dataWithIndex.Skip(start).Take(end - start + 1).ToList();
                double smoothedY = windowData.Average(d => d.Y);
                
                smoothed.Add((dataWithIndex[i].OriginalIndex, dataWithIndex[i].X, smoothedY));
            }

            return smoothed;
        }

        /// <summary>
        /// 计算归一化一阶差分（Δy/Δx = (Y[i+1]-Y[i])/(X[i+1]-X[i])）
        /// </summary>
        private List<(double MidX, double DiffValue)> CalculateNormalizedFirstDifferences(List<(int OriginalIndex, double X, double Y)> data)
        {
            var differences = new List<(double MidX, double DiffValue)>();
            for (int i = 0; i < data.Count - 1; i++)
            {
                double xDiff = data[i + 1].X - data[i].X;
                if (xDiff <= MinXDiffThreshold)
                    continue; // 跳过过小的X差值，避免溢出

                double yDiff = data[i + 1].Y - data[i].Y;
                double normalizedDiff = yDiff / xDiff; // 归一化差分（单位X变化的Y变化率）
                double midX = (data[i].X + data[i + 1].X) / 2; // 差分对应的中间X值

                differences.Add((midX, normalizedDiff));
            }
            return differences;
        }

        /// <summary>
        /// 计算归一化二阶差分（反映一阶差分的变化率：Δ(Δy/Δx)/Δx_mid）
        /// </summary>
        private List<(double MidX, double DiffValue)> CalculateNormalizedSecondDifferences(List<(double MidX, double DiffValue)> firstDifferences)
        {
            var secondDifferences = new List<(double MidX, double DiffValue)>();
            for (int i = 0; i < firstDifferences.Count - 1; i++)
            {
                double xDiff = firstDifferences[i + 1].MidX - firstDifferences[i].MidX;
                if (xDiff <= MinXDiffThreshold)
                    continue;

                double diffOfFirstDiff = firstDifferences[i + 1].DiffValue - firstDifferences[i].DiffValue;
                double normalizedSecondDiff = diffOfFirstDiff / xDiff;
                double midX = (firstDifferences[i].MidX + firstDifferences[i + 1].MidX) / 2;

                secondDifferences.Add((midX, normalizedSecondDiff));
            }
            return secondDifferences;
        }
        #endregion
    }

    /// <summary>
    /// 拐点实体类（包含原始索引，方便对应原始数据）
    /// </summary>
    public class InflectionPoint
    {
        /// <summary>
        /// 拐点在原始数据集合中的索引
        /// </summary>
        public int OriginalIndex { get; set; }

        /// <summary>
        /// 拐点的X值（原始X轴数据）
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 拐点的Y值（平滑后的Y轴数据）
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 拐点类型
        /// </summary>
        public InflectionPointType Type { get; set; }

        public override string ToString()
        {
            return $"原始索引：{OriginalIndex}, X：{X:F2}, Y：{Y:F2}, 类型：{Type}";
        }
    }

    /// <summary>
    /// 拐点类型枚举
    /// </summary>
    public enum InflectionPointType
    {
        /// <summary>
        /// 峰值（增→减）
        /// </summary>
        Peak,

        /// <summary>
        /// 谷值（减→增）
        /// </summary>
        Valley,

        /// <summary>
        /// 凹转凸（增长速率放缓/下降速率加快）
        /// </summary>
        ConcaveToConvex,

        /// <summary>
        /// 凸转凹（增长速率加快/下降速率放缓）
        /// </summary>
        ConvexToConcave
    }
}