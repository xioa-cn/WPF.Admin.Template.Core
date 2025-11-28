using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using External.GrpcServices.Services;

namespace External.GrpcServices.Start {
    /// <summary>
    /// 简化版的gRPC启动器，专门用于调试和WPF集成
    /// </summary>
    public class SimpleGrpcStartup {
        private static WebApplication? _app;
        private static CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// 简化的启动方法
        /// </summary>
        public static async Task<WebApplication> StartAsync(int port = 8999) {
            try {
                Console.WriteLine($"[SimpleGrpcStartup] 开始启动gRPC服务，端口: {port}");
                
                var builder = WebApplication.CreateBuilder();
                
                // 设置环境和日志
                builder.Environment.EnvironmentName = "Development";
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.SetMinimumLevel(LogLevel.Debug);
                
                // 配置gRPC
                builder.Services.AddGrpc(options => {
                    options.EnableDetailedErrors = true;
                });
                
                // 明确配置Kestrel和监听地址
                Console.WriteLine($"[SimpleGrpcStartup] 配置Kestrel监听 http://localhost:{port}");
                builder.WebHost.UseUrls($"http://localhost:{port}");
                builder.WebHost.ConfigureKestrel(options => {
                    options.ListenLocalhost(port, listenOptions => {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                });
                
                _app = builder.Build();
                
                // 注册gRPC服务
                Console.WriteLine("[SimpleGrpcStartup] 注册gRPC服务...");
                _app.MapGrpcService<NormalTestGrpcServerService>();
                
                // 添加根路径处理
                _app.MapGet("/", () => "gRPC服务正在运行");
                
                // 启动服务
                Console.WriteLine("[SimpleGrpcStartup] 在后台线程启动服务...");
                _cancellationTokenSource = new CancellationTokenSource();
                
                var startTask = Task.Run(async () => {
                    try {
                        Console.WriteLine($"[SimpleGrpcStartup] 服务开始运行在 http://localhost:{port}");
                        await _app.RunAsync(_cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException) {
                        Console.WriteLine("[SimpleGrpcStartup] 服务正常停止");
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"[SimpleGrpcStartup] 服务运行异常: {ex.Message}");
                        Console.WriteLine($"[SimpleGrpcStartup] 异常堆栈: {ex}");
                    }
                });
                
                // 等待服务启动
                await Task.Delay(3000);
                
                // 验证服务状态
                Console.WriteLine("[SimpleGrpcStartup] 验证服务状态...");
                if (_app != null && !startTask.IsCompleted) {
                    Console.WriteLine($"[SimpleGrpcStartup] ✅ gRPC服务启动成功！监听地址: http://localhost:{port}");
                } else {
                    Console.WriteLine("[SimpleGrpcStartup] ❌ gRPC服务启动失败");
                }
                
                return _app;
            }
            catch (Exception ex) {
                Console.WriteLine($"[SimpleGrpcStartup] 启动过程异常: {ex.Message}");
                Console.WriteLine($"[SimpleGrpcStartup] 详细异常: {ex}");
                throw;
            }
        }
        
        /// <summary>
        /// 停止服务
        /// </summary>
        public static async Task StopAsync() {
            try {
                Console.WriteLine("[SimpleGrpcStartup] 正在停止gRPC服务...");
                
                _cancellationTokenSource?.Cancel();
                
                if (_app != null) {
                    await _app.StopAsync(TimeSpan.FromSeconds(5));
                    await _app.DisposeAsync();
                    _app = null;
                }
                
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                
                Console.WriteLine("[SimpleGrpcStartup] gRPC服务已停止");
            }
            catch (Exception ex) {
                Console.WriteLine($"[SimpleGrpcStartup] 停止服务时发生异常: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 检查服务是否运行
        /// </summary>
        public static bool IsRunning => _app != null && _cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested;
    }
}