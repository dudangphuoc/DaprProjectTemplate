# DX Dapr Template

Template ASP.NET Core với Dapr cho việc phát triển ứng dụng phân tán sử dụng Clean Architecture.

## 📖 Tổng quan

DX Dapr Template là một template .NET hiện đại được thiết kế để tạo ra các ứng dụng phân tán với Dapr. Template này tuân theo các nguyên tắc Clean Architecture và Domain-Driven Design, cung cấp một nền tảng vững chắc cho việc phát triển các microservices.

### 🎯 Đặc điểm chính

- **Clean Architecture** với Domain-Driven Design
- **CQRS** với MediatR pattern
- **Repository Pattern** với Unit of Work
- **AutoMapper** cho object mapping
- **Dapr** integration cho distributed computing
- **Multi-framework support** (.NET 8, 9, 10)
- **Flexible database providers** (PostgreSQL, SQL Server, InMemory)
- **Feature toggles** để tùy chỉnh template
- **Production-ready** với monitoring và security
- **Secure build system** với API key management

## 🔧 Cài đặt Template

### Từ thư mục local
```bash
# Cài đặt template từ thư mục root của project
dotnet new install src/templates

# Kiểm tra template đã được cài đặt
dotnet new list | grep dxdapr
```

### Từ NuGet package (sau khi publish)
```bash
dotnet new install DX.Dapr.Template
```

### Gỡ cài đặt template
```bash
dotnet new uninstall src/templates
# hoặc
dotnet new uninstall DX.Dapr.Template
```

## 📚 Sử dụng Template

### Tạo project cơ bản
```bash
dotnet new dxdapr -n MyDaprApp
```

### Tùy chỉnh đầy đủ
```bash
dotnet new dxdapr -n MyECommerce \
  --ProjectName "MyECommerce" \
  --RootNamespace "MyCompany.ECommerce" \
  --CompanyName "My Company Ltd" \
  --ProjectDescription "E-commerce platform built with Dapr" \
  --ProjectVersion "1.0.0" \
  --Framework "net10.0" \
  --DatabaseProvider "PostgreSQL" \
  --HttpPort 5000 \
  --HttpsPort 7000 \
  --DaprAppId "my-ecommerce" \
  --DaprHttpPort 3500 \
  --DaprGrpcPort 50001 \
  --EnableSwagger true \
  --EnableRedis true \
  --EnableAuthentication true \
  --EnableHealthChecks true \
  --EnableOpenTelemetry true
```

### Setup tối giản (Testing)
```bash
dotnet new dxdapr -n "TestApp" \
  --DatabaseProvider "InMemory" \
  --EnableAuthentication false \
  --EnableRedis false \
  --skipRestore true
```

## 📋 Tham số Template

### Cơ bản
| Tham số | Mô tả | Mặc định | Bắt buộc |
|---------|-------|----------|----------|
| `ProjectName` | Tên project (dùng cho namespace và tên file) | MyDaprApp | Không |
| `RootNamespace` | Root namespace cho project | MyDaprApp | Không |
| `CompanyName` | Tên công ty/tác giả | DX Software | Không |
| `ProjectDescription` | Mô tả project | A Dapr-based distributed application... | Không |
| `ProjectVersion` | Version ban đầu | 1.0.0 | Không |

### Framework & Database
| Tham số | Mô tả | Lựa chọn | Mặc định |
|---------|-------|----------|----------|
| `Framework` | Target framework | net10.0, net9.0, net8.0 | net10.0 |
| `DatabaseProvider` | Provider database | PostgreSQL, SqlServer, InMemory | PostgreSQL |

### Cổng mạng
| Tham số | Mô tả | Kiểu | Mặc định |
|---------|-------|------|----------|
| `HttpPort` | Cổng HTTP | integer | Auto-generated (5000-5299) |
| `HttpsPort` | Cổng HTTPS | integer | Auto-generated (7000-7299) |
| `DaprAppId` | Dapr App ID | string | my-dapr-app |
| `DaprHttpPort` | Dapr HTTP port | integer | 3500 |
| `DaprGrpcPort` | Dapr gRPC port | integer | 50001 |

### Tính năng
| Tham số | Mô tả | Kiểu | Mặc định |
|---------|-------|------|----------|
| `EnableSwagger` | Bật Swagger/OpenAPI | bool | true |
| `EnableRedis` | Bật Redis caching | bool | true |
| `EnableAuthentication` | Bật JWT authentication | bool | true |
| `EnableHealthChecks` | Bật ASP.NET Core health checks | bool | true |
| `EnableOpenTelemetry` | Bật OpenTelemetry observability | bool | true |
| `skipRestore` | Bỏ qua NuGet restore sau khi tạo | bool | false |

## 📁 Cấu trúc Project

```
MyDaprApp/
├── src/
│   ├── MyDaprApp.Api/           # ASP.NET Core Web API
│   │   ├── Controllers/         # API Controllers
│   │   ├── Extensions/          # Service extensions
│   │   ├── Application/         # Application logic
│   │   └── Program.cs
│   ├── MyDaprApp.Domain/        # Domain entities
│   │   └── Utilities/           # Domain utilities
│   └── MyDaprApp.Infrastructure/ # Data access
│       └── CheckoutDbContext.cs
├── dapr/                        # Dapr configuration
│   ├── components/              # Dapr components
│   └── configuration/           # Dapr config
├── .editorconfig               # Code style rules
├── .gitignore                  # Git ignore rules
├── docker-compose.yml          # Docker compose
└── MyDaprApp.sln              # Solution file
```

## ⚡ Tính năng có sẵn

### 🏗️ Kiến trúc
- **Clean Architecture** với Domain-Driven Design
- **CQRS** với MediatR
- **Repository Pattern** với Unit of Work
- **AutoMapper** cho object mapping

### 🛠️ Công nghệ
- **ASP.NET Core** - Web framework
- **Dapr** - Distributed application runtime
- **Entity Framework Core** - ORM
- **PostgreSQL/SQL Server** - Database
- **Redis** - Caching (tùy chọn)
- **Serilog** - Structured logging
- **OpenTelemetry** - Observability

### 👀 Monitoring & Observability
- Structured logging với Serilog
- OpenTelemetry tracing
- Health checks
- Swagger/OpenAPI documentation

### 🔐 Security
- JWT Authentication (tùy chọn)
- Authorization policies
- CORS configuration

## 🏃 Chạy ứng dụng

### Với Dapr
```bash
# Restore dependencies
dotnet restore

# Chạy với Dapr
dapr run --app-id my-dapr-app --app-port 5000 --dapr-http-port 3500 --dapr-grpc-port 50001 -- dotnet run --project src/MyDaprApp.Api

# Hoặc sử dụng PowerShell
dapr run --app-id my-dapr-app --app-port 5000 --dapr-http-port 3500 --dapr-grpc-port 50001 -- dotnet run --project src/MyDaprApp.Api
```

### Không có Dapr
```bash
dotnet run --project src/MyDaprApp.Api
```

### Sử dụng Docker Compose
```bash
docker-compose up -d
```

## ⚙️ Cấu hình

Chỉnh sửa `appsettings.json` và `appsettings.Development.json` trong project API để cấu hình:

- Connection strings (PostgreSQL, Redis)
- OpenTelemetry settings
- Swagger configuration
- Authentication settings
- Logging configuration

## 🧪 Testing Template

### 1. Cài đặt Template
```powershell
# Từ thư mục root của project
dotnet new install src/templates

# Kiểm tra template đã được cài đặt
dotnet new list | Select-String "dxdapr"
```

### 2. Test Template
```powershell
# Tạo thư mục test
New-Item -ItemType Directory -Path "test-output" -Force
Set-Location "test-output"

# Test với các tham số mặc định
dotnet new dxdapr -n "TestApp"

# Test với tham số tùy chỉnh
dotnet new dxdapr -n "MyECommerce" `
  --ProjectName "MyECommerce" `
  --RootNamespace "MyCompany.ECommerce" `
  --CompanyName "My Company Ltd" `
  --ProjectDescription "E-commerce platform" `
  --Framework "net10.0" `
  --DatabaseProvider "PostgreSQL" `
  --HttpPort 5001 `
  --HttpsPort 7001 `
  --EnableSwagger true `
  --EnableRedis true `
  --EnableAuthentication true

# Quay lại thư mục gốc
Set-Location ..
```

### 3. Build test project
```powershell
Set-Location "test-output/TestApp"
dotnet restore
dotnet build
Set-Location ../..
```

### 4. Gỡ Template (nếu cần)
```powershell
dotnet new uninstall src/templates
```

## ✔️ Kiểm tra Template

### Sau khi tạo project test, kiểm tra:

1. **Namespace đã được thay đổi** - Mở các file .cs và kiểm tra namespace
2. **Tên project** - File .csproj và assembly name
3. **Cổng mạng** - File appsettings.json và launchSettings.json  
4. **Các tính năng tùy chọn** - Code swagger, redis, authentication có được include không
5. **Assembly info** - Company name, description trong AssemblyInfo

### Template Parameters Validation

| Parameter | Default | Test Value | Expected Result |
|-----------|---------|------------|----------------|
| ProjectName | MyDaprApp | MyECommerce | Namespace, assembly name thay đổi |
| CompanyName | DX Software | My Company Ltd | AssemblyInfo.Company thay đổi |
| Framework | net10.0 | net10.0 | TargetFramework trong .csproj |
| HttpPort | Auto-gen | 5001 | launchSettings.json |
| HttpsPort | Auto-gen | 7001 | launchSettings.json |
| EnableSwagger | true | true | Swagger middleware enabled |

### Kiểm tra kết quả

#### 1. Namespace Replacement
Kiểm tra file bất kỳ trong project được tạo:
```csharp
namespace MyECommerce.Api.Controllers; // Thay vì DXProjectName.Api.Controllers
```

#### 2. Assembly Information
Kiểm tra trong file `.csproj`:
```xml
<PropertyGroup>
    <AssemblyTitle>MyECommerce.Api</AssemblyTitle>
    <AssemblyCompany>My Company Ltd</AssemblyCompany>
    <AssemblyDescription>E-commerce platform</AssemblyDescription>
</PropertyGroup>
```

#### 3. Configuration Files
Kiểm tra `launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:7001;http://localhost:5001"
}
```

#### 4. Feature Toggles
- Nếu `EnableSwagger: false` thì không có Swagger configuration
- Nếu `EnableRedis: false` thì không có Redis connection setup
- Nếu `EnableAuthentication: false` thì không có JWT Bearer configuration

## 🔧 Troubleshooting

### Template không được nhận diện
```powershell
# Clear template cache
dotnet new --debug:reinit

# Reinstall template
dotnet new uninstall src/templates
dotnet new install src/templates
```

### Build lỗi sau khi tạo project
```powershell
# Restore packages
dotnet restore

# Clean và rebuild
dotnet clean
dotnet build
```

### Unicode/Encoding issues
Đảm bảo tất cả files được lưu với UTF-8 encoding, đặc biệt:
- Template files (.cs, .json, .md)
- Configuration files (appsettings.json)
- Documentation files

## 📈 Migration từ Template Cũ

Nếu đang dùng template cũ:

1. **Uninstall template cũ:**
   ```bash
   dotnet new uninstall DX.Dapr.Template
   ```

2. **Install template mới:**
   ```bash
   dotnet new install src/templates
   ```

3. **Update commands:**
   ```bash
   # Cũ
   dotnet new DX -n MyApp --AuthorName "My Company"
   
   # Mới  
   dotnet new dxdapr -n MyApp --CompanyName "My Company"
   ```

## 💻 Phát triển

1. Tạo entities trong `Domain` project
2. Tạo repository interfaces trong `Domain` và implementations trong `Infrastructure`
3. Tạo commands/queries và handlers trong `Api/Application`
4. Tạo controllers trong `Api/Controllers`
5. Cấu hình dependency injection trong `Extensions/ApplicationBuilderExtensions.cs`

## 🏗️ Build System

### _build.csproj Summary

Workspace này sử dụng **Nuke Build** system cho automation và build pipeline:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace></RootNamespace>
    <NoWarn>CS0649;CS0169;CA1050;CA1822;CA2211;IDE1006</NoWarn>
    <NukeRootDirectory>..</NukeRootDirectory>
    <NukeScriptDirectory>..</NukeScriptDirectory>
    <NukeTelemetryVersion>1</NukeTelemetryVersion>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Nuke.Common" Version="9.0.4" />
  </ItemGroup>
</Project>
```

#### Đặc điểm chính:
- **Nuke.Common 9.0.4** - Framework để tạo build scripts
- **Target Framework**: .NET 8.0
- **Executable**: Console application để chạy build commands
- **Non-packable**: Không phải là package để distribute
- **Warning suppression**: Tắt các cảnh báo không cần thiết cho build scripts
- **Secure API key management** - Không hardcode secrets trong code

#### Sử dụng Build System:
```bash
# Build và pack template
dotnet run --project build -- Pack

# Publish với secure API key
dotnet run --project build -- Publish

# Clean build
dotnet run --project build -- Clean Compile
```

### 🔐 Secure Publishing

Build system được cấu hình để bảo mật API keys:

#### Environment Variable (Khuyến nghị)
```bash
# Thiết lập API key
export NUGET_API_KEY="your-api-key-here"

# Publish template
dotnet run --project build -- Publish
```

#### Command Line Parameter
```bash
dotnet run --project build -- Publish --nuget-api-key "your-api-key"
```

#### Features:
- ✅ **API Key Validation** - Kiểm tra API key trước khi publish
- ✅ **Masked Logging** - Hiển thị API key dưới dạng `***...` trong logs
- ✅ **Multiple Sources** - Hỗ trợ nhiều nguồn cấu hình API key
- ✅ **Environment Variables** - Ưu tiên environment variables
- ✅ **Skip Duplicates** - Tự động skip package đã tồn tại
- ✅ **Flexible Sources** - Cấu hình được NuGet source URL

Chi tiết về cấu hình bảo mật: [`build/build-secrets.md`](build/build-secrets.md)

## 📄 License

MIT License - xem file LICENSE để biết thêm chi tiết.

---

**Tác giả:** Võ Hoàng Vũ  
**Version:** 2.0  
**Created:** 2025  
**Template Identity:** DX.Dapr.Template.v2