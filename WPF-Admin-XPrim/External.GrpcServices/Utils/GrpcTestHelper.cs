using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace External.GrpcServices.Utils {
    /// <summary>
    /// gRPC测试辅助工具
    /// </summary>
    public static class GrpcTestHelper {
        /// <summary>
        /// 检查端口是否被占用
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>如果端口被占用返回true</returns>
        public static bool IsPortInUse(int port) {
            try {
                var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                var tcpListeners = ipGlobalProperties.GetActiveTcpListeners();
                
                return tcpListeners.Any(x => x.Port == port);
            }
            catch (Exception ex) {
                Console.WriteLine($"检查端口时发生错误: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 获取可用的端口
        /// </summary>
        /// <param name="startPort">起始端口</param>
        /// <returns>可用端口号</returns>
        public static int GetAvailablePort(int startPort = 8999) {
            for (int port = startPort; port < startPort + 100; port++) {
                if (!IsPortInUse(port)) {
                    return port;
                }
            }
            return startPort; // 如果都被占用，返回起始端口
        }
        
        /// <summary>
        /// 测试TCP连接
        /// </summary>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns>连接是否成功</returns>
        public static async Task<bool> TestTcpConnection(string host = "localhost", int port = 8999, int timeout = 3000) {
            try {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var completedTask = await Task.WhenAny(connectTask, Task.Delay(timeout));
                
                if (completedTask == connectTask && client.Connected) {
                    Console.WriteLine($"TCP连接测试成功: {host}:{port}");
                    return true;
                }
                else {
                    Console.WriteLine($"TCP连接测试失败: {host}:{port} (超时或连接失败)");
                    return false;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"TCP连接测试异常: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 打印当前系统信息
        /// </summary>
        public static void PrintSystemInfo() {
            Console.WriteLine("=== 系统信息 ===");
            Console.WriteLine($"操作系统: {Environment.OSVersion}");
            Console.WriteLine($".NET版本: {Environment.Version}");
            Console.WriteLine($"机器名: {Environment.MachineName}");
            Console.WriteLine($"当前目录: {Environment.CurrentDirectory}");
            Console.WriteLine("================");
        }
    }
}