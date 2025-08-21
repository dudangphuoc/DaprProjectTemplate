# ?? DX Dapr Template - H??ng d?n s? d?ng ??y ??

## ?? T?ng quan

DX Dapr Template l� m?t template .NET hi?n ??i ???c thi?t k? ?? t?o ra c�c ?ng d?ng ph�n t�n s? d?ng Dapr (Distributed Application Runtime) v?i Clean Architecture.

## ? Quick Start

### 1. C�i ??t Template
```bash
# T? th? m?c local
dotnet new install src/templates

# Ki?m tra ?� c�i ??t th�nh c�ng
dotnet new list dxdapr
```

### 2. T?o Project ??u ti�n
```bash
# T?o project c? b?n
dotnet new dxdapr -n MyFirstDaprApp

# Chuy?n v�o th? m?c project
cd MyFirstDaprApp

# Restore v� build
dotnet restore
dotnet build
```

### 3. Ch?y ?ng d?ng
```bash
# V?i Dapr (khuy?n ngh?)
dapr run --app-id my-first-dapr-app --app-port 5000 --dapr-http-port 3500 -- dotnet run --project src/MyFirstDaprApp.Api

# Ho?c ch?y tr?c ti?p
dotnet run --project src/MyFirstDaprApp.Api
```

## ?? C�c t�nh n?ng ch�nh

### ?? Clean Architecture
- **Domain Layer**: Entities, Value Objects, Domain Services
- **Application Layer**: Use Cases, CQRS v?i MediatR
- **Infrastructure Layer**: Data Access, External Services
- **Presentation Layer**: Web API Controllers

### ?? Dapr Integration
- **Service-to-Service Communication**: HTTP v� gRPC
- **State Management**: Key-value store v?i c�c state stores
- **Pub/Sub Messaging**: Event-driven communication
- **Secrets Management**: Secure configuration management

### ?? Modern .NET Stack
- **.NET 10/9/8**: Multi-framework support
- **Entity Framework Core**: Code-first database approach
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation
- **Serilog**: Structured logging

### ?? Observability Ready
- **OpenTelemetry**: Distributed tracing v� metrics
- **Health Checks**: Application health monitoring
- **Swagger/OpenAPI**: API documentation
- **Structured Logging**: JSON-formatted logs

### ?? Production Ready
- **JWT Authentication**: Secure API access
- **CORS Configuration**: Cross-origin resource sharing
- **Redis Caching**: Distributed caching (optional)
- **Docker Support**: Containerized deployment

## ?? T�y ch?nh Template

### Framework Selection
```bash
# .NET 10 (m?c ??nh)
dotnet new dxdapr -n MyApp --Framework net10.0

# .NET 8 LTS
dotnet new dxdapr -n MyApp --Framework net8.0
```

### Database Provider
```bash
# PostgreSQL (khuy?n ngh? cho production)
dotnet new dxdapr -n MyApp --DatabaseProvider PostgreSQL

# SQL Server
dotnet new dxdapr -n MyApp --DatabaseProvider SqlServer

# In-Memory (cho testing)
dotnet new dxdapr -n MyApp --DatabaseProvider InMemory
```

### Feature Toggles
```bash
# Minimal setup (ch? core features)
dotnet new dxdapr -n MyApp \
  --EnableSwagger false \
  --EnableRedis false \
  --EnableAuthentication false

# Full-featured setup
dotnet new dxdapr -n MyApp \
  --EnableSwagger true \
  --EnableRedis true \
  --EnableAuthentication true \
  --EnableHealthChecks true \
  --EnableOpenTelemetry true
```

### Custom Ports
```bash
dotnet new dxdapr -n MyApp \
  --HttpPort 5001 \
  --HttpsPort 7001 \
  --DaprHttpPort 3501 \
  --DaprGrpcPort 50002
```

## ?? C?u tr�c Project

```
MyDaprApp/
??? src/
?   ??? MyDaprApp.Api/              # ?? Web API Layer
?   ?   ??? Controllers/            # REST API endpoints
?   ?   ??? Extensions/             # DI configuration
?   ?   ??? Application/            # CQRS handlers
?   ?   ?   ??? Commands/           # Write operations
?   ?   ?   ??? Queries/            # Read operations
?   ?   ?   ??? AutoMapperProfiles/ # Object mapping
?   ?   ??? Properties/             # Launch settings
?   ?   ??? Program.cs              # Application entry point
?   ?
?   ??? MyDaprApp.Domain/           # ??? Domain Layer
?   ?   ??? Entities/               # Domain entities
?   ?   ??? ValueObjects/           # Value objects
?   ?   ??? Services/               # Domain services
?   ?   ??? Utilities/              # Domain utilities
?   ?
?   ??? MyDaprApp.Infrastructure/   # ?? Infrastructure Layer
?       ??? Persistence/            # Database context
?       ??? Repositories/           # Data access implementations
?       ??? Services/               # External service clients
?       ??? CheckoutDbContext.cs    # EF Core DbContext
?
??? .editorconfig                   # Code style rules
??? .gitignore                      # Git ignore patterns
??? .gitattributes                  # Git attributes
??? MyDaprApp.sln                   # Solution file
??? README.md                       # Project documentation
```

## ?? Deployment

### Local Development
```bash
# Start v?i Dapr
dapr run --app-id myapp --app-port 5000 --dapr-http-port 3500 -- dotnet run

# Access Swagger UI
# https://localhost:7000/swagger
```

### Docker
```bash
# Build Docker image
docker build -t myapp:latest .

# Run v?i Dapr
dapr run --app-id myapp --app-port 80 --dapr-http-port 3500 -- docker run -p 8080:80 myapp:latest
```

### Kubernetes
```bash
# Deploy v?i Dapr
kubectl apply -f k8s/deployment.yaml
```

## ?? Best Practices

### 1. **Domain-Driven Design**
- T?p trung v�o business logic trong Domain layer
- S? d?ng Rich Domain Models
- Implement Domain Events cho loose coupling

### 2. **CQRS Pattern**
- T�ch bi?t Read v� Write operations
- S? d?ng MediatR cho message handling
- Implement validation v?i FluentValidation

### 3. **Dapr Patterns**
- S? d?ng Service Invocation cho inter-service communication
- Implement Pub/Sub cho event-driven architecture
- Leverage State Management cho persistence

### 4. **Security**
- Always validate input data
- Use JWT for stateless authentication
- Implement proper authorization policies

### 5. **Monitoring**
- Enable structured logging v?i Serilog
- Use OpenTelemetry cho distributed tracing
- Implement comprehensive health checks

## ?? Troubleshooting

### Common Issues

#### Template kh�ng ???c t�m th?y
```bash
# Clear cache v� reinstall
dotnet new --debug:reinit
dotnet new uninstall src/templates
dotnet new install src/templates
```

#### Build errors sau khi t?o project
```bash
# Restore packages
dotnet restore

# Clear v� rebuild
dotnet clean
dotnet build
```

#### Dapr connection issues
```bash
# Ki?m tra Dapr ?� ???c c�i ??t
dapr --version

# Initialize Dapr
dapr init

# Ki?m tra Dapr components
dapr components -k
```

### Performance Tips
- Enable Response Caching cho read-heavy operations
- Use Redis cho distributed caching
- Implement database connection pooling
- Optimize Entity Framework queries

## ?? Resources

- [Dapr Documentation](https://docs.dapr.io/)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET 10 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)

## ?? Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## ?? License

This template is licensed under the MIT License. See [LICENSE](LICENSE) for details.