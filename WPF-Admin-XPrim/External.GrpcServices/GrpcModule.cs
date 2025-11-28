using External.GrpcServices.Start;
using External.GrpcServices.Utils;

namespace External.GrpcServices {
    /// <summary>
    /// gRPC模块，提供在WPF项目中启动gRPC服务的方法
    /// </summary>
    public class GrpcModule {
        /// <summary>
        /// 在WPF项目中启动gRPC服务的示例方法
        /// 在WPF的App.xaml.cs或MainWindow的构造函数中调用
        /// </summary>
        public static async Task StartGrpcServiceAsync() {
            try {
                Console.WriteLine("开始启动gRPC服务...");
                
                // 只使用简化版本，避免重复注册
                var port = int.Parse(Environment.GetEnvironmentVariable("GRPC_PORT") ?? "8999");
                Console.WriteLine($"使用简化启动器，端口: {port}");
                
                var app = await SimpleGrpcStartup.StartAsync(port);
                Console.WriteLine("gRPC服务启动成功！");
            }
            catch (Exception ex) {
                Console.WriteLine($"gRPC服务启动失败：{ex.Message}");
                Console.WriteLine($"异常详情：{ex}");
                throw;
            }
        }

        /// <summary>
        /// 停止gRPC服务
        /// 在WPF应用退出时调用
        /// </summary>
        public static async Task StopGrpcServiceAsync() {
            try {
                // 先尝试停止简化版本
                await SimpleGrpcStartup.StopAsync();
                // 再停止原版本
                await StartupGrpc.StopAsync();
                Console.WriteLine("gRPC服务已停止");
            }
            catch (Exception ex) {
                Console.WriteLine($"gRPC服务停止失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 检查gRPC服务是否正在运行
        /// </summary>
        public static bool IsGrpcServiceRunning => StartupGrpc.IsRunning || SimpleGrpcStartup.IsRunning;
        
        /// <summary>
        /// 测试gRPC连接
        /// </summary>
        private static async Task TestGrpcConnection() {
            try {
                var port = Environment.GetEnvironmentVariable("GRPC_PORT") ?? "8999";
                // 这里可以添加简单的HTTP检查，但gRPC服务不支持HTTP/1.1
                Console.WriteLine($"请确认gRPC服务在 http://localhost:{port} 上运行");
                Console.WriteLine("您可以使用gRPC客户端测试连接。");
            }
            catch (Exception ex) {
                Console.WriteLine($"测试连接失败: {ex.Message}");
            }
        }
    }
}