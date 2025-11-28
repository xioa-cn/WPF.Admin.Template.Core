# WPF.Admin.Template.Core

基于 .NET 8.0 的现代化 WPF 企业级后台管理系统模板，集成了 XPrism（Prism框架轻量实现）、多语言国际化、主题系统等核心功能。

## 🚀 特性

### 核心技术栈
- **.NET 8.0** - 最新稳定版本
- **WPF** - Windows Presentation Foundation
- **XPrism** - 类似Prism但更轻量
- **MVVM模式** - 完整的MVVM模式实现

### 核心功能
- **模块化架构** - 基于区域的模块化设计
- **多语言支持** - 完整的I18n国际化系统
- **主题系统** - 动态主题切换（深色/浅色）
- **权限管理** - 基于角色的权限控制系统
- **导航系统** - 强大的区域导航管理
- **通知系统** - 系统托盘和消息通知

### UI组件
- **HandyControl** - 现代化UI组件库
- **自定义控件** - 丰富的自定义控件库
- **动画效果** - 流畅的界面动画
- **响应式设计** - 适配不同屏幕尺寸

## 📁 项目结构

```
WPF-Admin-XPrim/
├── WPFAdmin/                    # 主应用程序
├── WPF.Admin.Themes/           # 主题系统
├── WPF.Admin.Models/           # 数据模型
├── WPF.Admin.Service/          # 服务层
├── WPFAdmin.NavigationModules/ # 导航模块
├── WPFAdmin.LoginModules/      # 登录模块
├── WPFAdmin.DialogModules/     # 对话框模块
├── CMS.AppRouters/             # 应用路由
├── CMS.AppSettings/            # 应用设置
├── XPrism.Core/                # XPrism核心框架
└── 其他功能模块...
```

## 🛠️ 快速开始

### 环境要求
- Visual Studio 2022 或更高版本
- .NET 8.0 SDK
- Windows 10/11

### 构建和运行

1. **克隆项目**
   ```bash
   git clone https://github.com/xioa-cn/WPF.Admin.Template.Core
   ```

2. **打开解决方案**
   ```bash
   cd WPF-Admin-XPrim
   start WPF-Admin-XPrism.sln
   ```

3. **设置启动项目**
   - 设置 `WPFAdmin` 或 `WPFAdmin.AuthApplication` 为启动项目

4. **构建并运行**
   - 按 F5 或点击运行按钮启动应用

## 🎯 核心功能详解

### XPrism 模块化框架

XPrism 是一个轻量级的模块化框架，提供：
- **区域管理** - 动态内容区域管理
- **模块发现** - 自动模块加载机制
- **依赖注入** - 简化的服务注册

### 国际化 (I18n)

项目支持中英文双语切换：
- 语言资源文件位于 `Langs/` 目录
- 支持动态语言切换
- XAML 中直接使用 `{i18n:Localize Key}` 绑定

### 主题系统

基于 HandyControl 的主题系统：
- 深色/浅色主题切换
- 自定义主题色彩配置
- 动态资源绑定支持

### 🔐 授权系统 (WPFAdmin.AuthApplication)

WPFAdmin.AuthApplication 是项目的核心授权管理模块，提供完整的软件授权解决方案：

#### 授权模式
- **ListenAuthorization模式** - 需要授权码验证
- **NoAuthorization模式** - 无需授权码，但需要 `NoAuthorizationRequired.dll` 授权模块

#### 授权码管理
- **动态授权码生成** - 基于版本号和时间戳生成授权码
- **授权码加密** - 使用 `TextCodeHelper` 进行加密解密
- **授权码验证** - 自动验证授权码有效性

#### 授权工具功能
- **生成密钥** - 根据版本号生成永久授权码
- **时限密钥** - 基于时间戳生成有时效性的授权码
- **授权码复制** - 一键复制生成的授权码

#### 权限管理
支持三级权限体系：
- **管理员 (Admin)** - 完全权限，可访问所有功能
- **工程师 (Engineer)** - 技术权限，可访问技术相关功能
- **员工 (Employee)** - 基础权限，仅限基础操作

#### 授权文件
- `authcode.txt` - 授权码存储文件
- `authcode.dll` - 授权模块文件
- `WPFAdmin.auc.bat` - 授权码写入批处理工具

## 🔧 配置说明

### 应用配置
- `appSettings.json` - 应用基础配置
- `router.json` - 路由配置
- `modules.config` - 模块配置文件

### 语言配置
- `Langs/zh.json` - 中文语言包
- `Langs/en.json` - 英文语言包

## 📖 开发指南

### 添加新模块

1. 创建新的模块项目
2. 实现模块接口
3. 在 `modules.config` 中注册模块
4. 配置路由信息

### 自定义主题

1. 在 `WPF.Admin.Themes` 项目中添加主题资源
2. 更新主题配置文件
3. 在主应用中选择主题

### 添加新语言

XPrism框架提供了智能的语言文件自动识别机制，添加新语言只需简单几步：

#### 1. 创建语言文件（自动识别）
- 在 `Langs/` 目录下创建新的语言文件，格式为 `{lang-code}.json`
- **自动识别机制**：系统会自动扫描 `Langs/` 目录下的所有 `.json` 文件
- 文件名即为语言代码（如：`fr.json`、`ja.json`、`es.json`）

```json
// 示例：Langs/fr.json
{
  "Welcome": "Bienvenue",
  "Login": "Connexion",
  "Settings": "Paramètres",
  "User": {
    "Profile": "Profil",
    "Logout": "Déconnexion"
  }
}
```

#### 2. 自动注册语言选项
- **无需手动注册**：系统启动时自动检测所有语言文件
- 通过 `I18nManager` 的 `Initialize` 方法自动加载
- 语言选择器会自动显示所有可用的语言选项
- 支持本地文件目录加载和程序资源加载

#### 3. 智能语言切换
- **动态切换**：语言切换时自动重新加载对应语言文件
- **嵌套结构支持**：支持JSON嵌套结构，自动扁平化为单层字典
- **错误处理**：如果目标语言文件不存在，自动回退到默认语言

**优势**：
- ✅ **零配置**：创建文件即自动识别
- ✅ **动态更新**：修改语言文件无需重启应用
- ✅ **结构灵活**：支持复杂的JSON嵌套结构
- ✅ **错误恢复**：文件缺失时自动回退到默认语言

## 🌐 对外GRPC服务

项目提供了完整的对外GRPC服务支持，可以轻松构建微服务架构和跨语言通信。

### 架构特点

#### 服务端架构 (`External.GrpcServices`)
- **基于ASP.NET Core** - 使用最新的gRPC.AspNetCore框架
- **HTTP/2协议** - 高性能二进制通信协议
- **自动服务发现** - 通过反射自动注册GRPC服务
- **端口动态管理** - 智能端口检测和分配
- **WPF集成** - 与WPF应用无缝集成，支持后台运行

#### 核心组件
- `StartupGrpc` - GRPC服务启动器，支持异步/同步启动
- `MapGrpcServicesHelper` - 自动服务发现和注册工具
- `GrpcServiceAttribute` - 服务标记属性，用于自动发现
- `ApplicationGrpc` - 应用层GRPC服务管理

### 快速开始

#### 1. 启动GRPC服务
```csharp
// 在WPF应用启动时自动启动GRPC服务
Task.Run(() => ApplicationGrpc.AppGrpcServices());

// 或者手动启动
await StartupGrpc.StartupAsync();
```

#### 2. 创建GRPC服务
```csharp
[GrpcService]
public class MyService : MyService.MyServiceBase
{
    public override Task<MyResponse> MyMethod(MyRequest request, ServerCallContext context)
    {
        // 业务逻辑处理
        return Task.FromResult(new MyResponse { Result = "Success" });
    }
}
```

#### 3. 定义Proto文件
```protobuf
syntax = "proto3";
option csharp_namespace = "GrpcServices";

package mypackage;

service MyService {
    rpc MyMethod(MyRequest) returns (MyResponse);
}

message MyRequest {
    string data = 1;
}

message MyResponse {
    string result = 1;
}
```

## 🌐 Garnet Server

#### 应用配置
在 `appConfigSettings.json` 中配置：
```json
{
  "OpenExternalGrpc": true,
  "GrpcPort": 8999
}
```

#### 环境变量
- `GRPC_PORT` - 指定GRPC服务端口（默认：8999）

#### 其他语言客户端
支持所有主流编程语言：
- **Java**：通过protobuf-java生成客户端
- **Python**：使用grpcio库
- **Go**：使用google.golang.org/grpc
- **Node.js**：使用@grpc/grpc-js


#### 端口检测工具
- `GrpcTestHelper` - 端口占用检测和可用端口查找
- 自动端口分配 - 避免端口冲突

#### 服务监控
- 实时日志输出
- 连接状态监控
- 性能指标收集

## 📡 SharedMemoryPubSub 内存发布订阅

项目集成了高效的Windows内存发布订阅系统，为进程间通信提供高性能解决方案。

### 架构特点

#### 进程间通信架构
- **共享内存** - 基于Windows共享内存的高性能通信
- **发布订阅模式** - 支持多对多的消息传递
- **事件驱动** - 异步事件处理机制
- **零拷贝** - 内存直接读写，避免数据复制

#### 核心组件
- `SharedMemoryPubSubManager` - 共享内存发布订阅管理器
- `ISharedMemoryPublisher` - 发布者接口
- `ISharedMemorySubscriber` - 订阅者接口
- `SharedMemoryEvent` - 共享内存事件定义

### 配置选项

#### 内存配置
- **缓冲区大小** - 可配置的共享内存缓冲区大小
- **消息队列长度** - 支持消息队列长度限制
- **超时设置** - 读写操作的超时时间配置

#### 性能优化
- **批量发送** - 支持消息批量发送
- **异步处理** - 全异步操作，不阻塞UI线程
- **内存池** - 使用内存池优化内存分配

#### WPF应用内部通信

#### 进程间通信


### 优势特性

#### 性能优势
- **零拷贝传输** - 数据直接在内存中传递
- **低延迟** - 微秒级的消息传递延迟
- **高吞吐量** - 支持大量并发消息
- **资源友好** - 内存占用小，CPU使用率低

#### 开发优势
- **简单易用** - 简洁的API设计
- **类型安全** - 强类型消息支持
- **线程安全** - 多线程环境安全使用
- **可扩展** - 支持自定义消息格式和序列化

## 🧪 测试

项目包含多个测试项目：
- `HttpRequest.Test` - HTTP请求测试
- `GrpcServices.Test` - gRPC服务测试
- `CMS.Plcs.Test` - PLC通信测试
- `WPFAdmin.AuthApplication` - 授权系统测试

运行测试：
```bash
cd WPF-Admin-XPrim
dotnet test
```

### 🔐 授权系统使用指南

#### 启动授权工具
1. 设置 `WPFAdmin.AuthApplication` 为启动项目
2. 按 F5 运行授权工具

#### 生成授权码
- **永久授权码**：在版本号输入框中输入软件版本号，点击"生成密钥"
- **时限授权码**：在版本号输入框中输入日期（如：2025-12-31），点击"时限密钥"

#### 应用授权码
1. 使用生成的授权码
2. 运行 `WPFAdmin.auc.bat` 批处理文件
3. 输入授权码，自动写入 `authcode.txt`

#### 授权模式配置
- 在应用配置文件中设置授权模式
- `ListenAuthorization`：需要授权码验证
- `NoAuthorization`：无需授权码但需要授权模块

### 发布应用
```bash
cd WPF-Admin-XPrim/WPFAdmin
dotnet publish -c Release -o ./publish
```

### 安装程序
项目包含安装程序项目 `Xioa.Setup`，可生成安装包。

### 🔐 授权部署说明

#### 生产环境授权配置
1. **授权模块准备**
   - 确保 `authcode.dll` 授权模块存在
   - 配置 `NoAuthorizationRequired.dll`（如果使用NoAuthorization模式）

2. **授权码部署**
   - 使用 `WPFAdmin.AuthApplication` 生成授权码
   - 将授权码部署到目标环境
   - 确保授权文件权限正确

3. **授权验证**
   - 应用启动时自动验证授权码
   - 授权失败时显示相应提示
   - 支持授权码动态更新

#### 授权文件说明
- `authcode.txt` - 存储授权码的文本文件
- `authcode.dll` - 授权验证核心模块
- `NoAuthorizationRequired.dll` - 无授权模式所需模块
- `WPFAdmin.auc.bat` - 授权码写入工具

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 📞 联系方式

如有问题或建议，请通过以下方式联系：
- 提交 Issue
---

**注意**: 这是一个企业级WPF应用模板，适用于需要模块化、多语言、主题切换等高级功能的后台管理系统开发。
