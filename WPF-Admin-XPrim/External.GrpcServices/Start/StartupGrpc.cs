using System.Net.Http;
using System.Reflection;
using External.GrpcServices.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace External.GrpcServices.Start {
    public class StartupGrpc {
        private static WebApplication? _app;
        private static CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// 启动gRPC服务（非阻塞方式，适用于WPF项目）
        /// </summary>
        /// <returns>返回Task，可以await或在后台运行</returns>
        public static async Task<WebApplication> StartupAsync() {
            // 创建自定义的WebApplication构建器
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
                Args = null,
                EnvironmentName = "Development"
            });

            // 从环境变量或配置获取端口
            var port = Environment.GetEnvironmentVariable("GRPC_PORT") ?? "8999";
            var grpcPort = int.Parse(port);
            
            // 检查端口是否被占用
            if (GrpcTestHelper.IsPortInUse(grpcPort)) {
                Console.WriteLine($"警告：端口 {grpcPort} 已被占用，尝试查找可用端口...");
                grpcPort = GrpcTestHelper.GetAvailablePort(grpcPort);
                Console.WriteLine($"使用端口: {grpcPort}");
            }
            
            Console.WriteLine($"正在启动gRPC服务，端口: {grpcPort}");
            GrpcTestHelper.PrintSystemInfo();

            // 配置日志级别
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            builder.Logging.AddConsole();

            builder.Services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                //options.Interceptors.Add<AuthInterceptor>(); // 权限拦截器
            });
            
            // 关键修复：明确配置Kestrel和URL
            builder.WebHost.UseUrls($"http://localhost:{grpcPort}");
            builder.WebHost.ConfigureKestrel(options =>
            {
                // 清除默认端点配置
                options.ConfigureEndpointDefaults(endpointOptions =>
                {
                    endpointOptions.Protocols = HttpProtocols.Http2;
                });
                
                // 直接指定监听地址和端口
                options.Listen(System.Net.IPAddress.Loopback, grpcPort, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });

            _app = builder.Build();
            
            Console.WriteLine("正在映射gRPC服务...");

            // 只使用自动发现方式，避免重复注册
            try {
                _app.MapGrpcServices(typeof(StartupGrpc).Assembly);
                Console.WriteLine("✅ gRPC服务注册完成");
            }
            catch (Exception ex) {
                Console.WriteLine($"❌ gRPC服务注册失败: {ex.Message}");
                throw new InvalidOperationException("无法注册gRPC服务", ex);
            }
            _app.MapGet("/", () =>
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            // 在后台线程启动服务，避免阻塞WPF主线程
            _cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(async () => {
                try {
                    Console.WriteLine($"gRPC服务正在启动在端口 {grpcPort}...");
                    await _app.RunAsync(_cancellationTokenSource.Token);
                }
                catch (OperationCanceledException) {
                    // 正常关闭，忽略异常
                    Console.WriteLine("gRPC服务已正常关闭");
                }
                catch (Exception ex) {
                    // 记录异常
                    Console.WriteLine($"gRPC服务运行异常: {ex.Message}");
                    Console.WriteLine($"异常详细信息: {ex}");
                }
            }, _cancellationTokenSource.Token);

            // 等待服务启动
            await Task.Delay(2000); // 增加等待时间
            Console.WriteLine($"gRPC服务已在端口 {grpcPort} 上启动完成！");
            
            // 测试TCP连接
            await GrpcTestHelper.TestTcpConnection("localhost", grpcPort);
            
            return _app;
        }

        /// <summary>
        /// 同步启动方法（阻塞方式，适用于控制台应用）
        /// </summary>
        public static void Startup() {
            var task = StartupAsync();
            task.Wait();
        }

        /// <summary>
        /// 停止gRPC服务
        /// </summary>
        public static async Task StopAsync() {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested) {
                _cancellationTokenSource.Cancel();
            }
            
            if (_app != null) {
                await _app.StopAsync();
                await _app.DisposeAsync();
                _app = null;
            }
            
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        /// <summary>
        /// 检查服务是否正在运行
        /// </summary>
        public static bool IsRunning => _app != null && _cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested;
    }
}