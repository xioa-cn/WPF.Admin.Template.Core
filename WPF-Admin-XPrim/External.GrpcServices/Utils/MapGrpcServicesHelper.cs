﻿using System.Reflection;
using External.GrpcServices.Services;
using Microsoft.AspNetCore.Builder;
using WPF.Admin.Service.Services.Grpcs;

namespace External.GrpcServices.Utils {
    public static class MapGrpcServicesHelper {
        // 移除无参数的重载方法，避免重复注册
        // public static WebApplication MapGrpcServices(this WebApplication app) {
        //     app.MapGrpcService<NormalTestGrpcServerService>();
        //     return app;
        // }

        public static WebApplication MapGrpcServices(this WebApplication app, Assembly assembly) {
            Console.WriteLine($"[MapGrpcServicesHelper] 开始自动发现程集 {assembly.FullName} 中的gRPC服务...");
            
            var mapGrpcServiceMethod = typeof(GrpcEndpointRouteBuilderExtensions)
                .GetMethods()
                .First(m => 
                    m.Name == "MapGrpcService" && 
                    m.IsGenericMethodDefinition && 
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(IEndpointRouteBuilder));

            var registeredCount = 0;
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.GetCustomAttribute<GrpcServiceAttribute>() != null)
                {
                    Console.WriteLine($"[MapGrpcServicesHelper] 发现gRPC服务: {type.FullName}");
                    var genericMethod = mapGrpcServiceMethod.MakeGenericMethod(type);
                    try
                    {
                        genericMethod.Invoke(null, new object[] { app });
                        registeredCount++;
                        Console.WriteLine($"[MapGrpcServicesHelper] ✅ 成功注册: {type.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[MapGrpcServicesHelper] ❌ 注册失败 {type.FullName}: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }
            }
            
            Console.WriteLine($"[MapGrpcServicesHelper] 共注册了 {registeredCount} 个gRPC服务");
            return app;
        }
    }
}