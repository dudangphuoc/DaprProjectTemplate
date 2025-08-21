using DX.Application.FailedResponseHandling;
using DX.Application.SucceedResponseHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public static class ApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        // builder.Services
        //     .Configure<___Configuration>(configuration.GetSection("___Configuration"));
        return builder;
    }

    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        //// Cấu hình Serilog với Graylog sink hoặc các sink khác
        builder.Host.UseSerilog((context, config) =>
        {
            var uri = context.Configuration.GetValue<string>("SigNoz:Uri")!;
            var port = context.Configuration.GetValue<int>("SigNoz:Port")!;
            config.Enrich.FromLogContext()
                  .Enrich.WithMachineName()
                  .Enrich.WithEnvironmentName()
                  .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                  .Enrich.WithSpan(new SpanOptions
                  {
                      IncludeOperationName = true,
                      IncludeTags = true
                  })
                  .ReadFrom.Configuration(context.Configuration)
                  .WriteTo.Debug()
                  .WriteTo.Console()
                  .WriteTo.OpenTelemetry(options =>
                  {
                      options.Endpoint = $"http://{uri}:{port}";
                      options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
                      options.ResourceAttributes = new Dictionary<string, object>
                      {
                          ["service.name"] = "CoreServices",
                          ["environment"] = context.HostingEnvironment.EnvironmentName,
                          ["facility"] = $"{context.HostingEnvironment.EnvironmentName}-{context.HostingEnvironment.ApplicationName}",
                      };
                  }); ;
        });

        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
        var configuration = builder.Configuration;
        var openTelemetryOptions = builder.Configuration.GetSection("OpenTelemetryOptions").Get<OpenTelemetryOptions>() ?? throw new Exception("Can't find OpenTelemetryOptions");
        builder.Services.AddSingleton(openTelemetryOptions);
        GlobalUsings.OpenTelemetryOptions = openTelemetryOptions;

        // Note: Switch between Zipkin/OTLP/Console by setting UseTracingExporter in appsettings.json.
        var tracingExporter = (GlobalUsings.OpenTelemetryOptions.UseTracingExporter ?? "console")!.ToLowerInvariant();
        // Note: Switch between Console/OTLP by setting UseLogExporter in appsettings.json.
        GlobalUsings.LogExporter = (GlobalUsings.OpenTelemetryOptions.UseLogExporter ?? "console")!.ToLowerInvariant();
        // Note: Switch between Explicit/Exponential by setting HistogramAggregation in appsettings.json
        var histogramAggregation = (GlobalUsings.OpenTelemetryOptions.HistogramAggregation ?? "explicit")!.ToLowerInvariant();
        // Note: Switch between Prometheus/OTLP/Console by setting UseMetricsExporter in appsettings.json.
        GlobalUsings.MetricsExporter = (GlobalUsings.OpenTelemetryOptions.UseMetricsExporter ?? "console")!.ToLowerInvariant();
        string codeBase = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
        var serviceName = string.Format("{0}-{1}", builder.Environment.EnvironmentName, GlobalUsings.OpenTelemetryOptions.ServiceName!);
        GlobalUsings.ConfigureResource = r => r.AddService(serviceName: serviceName, serviceVersion: codeBase, serviceInstanceId: Environment.MachineName);
        builder.Services.AddSingleton<Instrumentation>();
        builder.Services.AddSingleton<AppMetricsReporter>();

        var compositeTextMapPropagator = new CompositeTextMapPropagator(new TextMapPropagator[]
        {
            new TraceContextPropagator(),
            new BaggagePropagator()
        });

        Sdk.SetDefaultTextMapPropagator(compositeTextMapPropagator);
        // Cấu hình OpenTelemetry với Jaeger exporter bật các instrumentations cần thiết
        builder.Services.AddOpenTelemetry()
           .ConfigureResource(GlobalUsings.ConfigureResource)
            .WithTracing(options =>
            {
                options
                  .AddSource("Mediator", "IntegrationEvent", "Subscriptions",
                    //ActivitySourceNames.RabbitMQEventSubChannel,
                    Instrumentation.ActivitySourceName
                  )
                  .SetErrorStatusOnException()
                  .AddHttpClientInstrumentation()
                  .AddAspNetCoreInstrumentation()
                  .AddEntityFrameworkCoreInstrumentation();

                options.AddSource("Mediator", "IntegrationEvent", "Subscriptions")
                    .SetErrorStatusOnException()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRabbitMQInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                    });

                options.AddOtlpExporter(otlpOptions =>
                {
                    var endpoint = openTelemetryOptions.Otlp.Endpoint ??= configuration.GetValue<string>("OpenTelemetryOptions:Otlp:Endpoint"); ;
                    otlpOptions.Endpoint = new Uri(endpoint);
                });
            });

        return builder;
    }

    public static WebApplicationBuilder AddMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(typeof(ApplicationBuilderExtensions).Assembly, typeof(CheckoutDbContext).Assembly);
        builder.Services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ActivityPipelineBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatingPipelineBehaviour<,>));

        return builder;
    }

    public static WebApplicationBuilder AddAutoMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(ApplicationBuilderExtensions).Assembly);

        return builder;
    }

    public static WebApplicationBuilder AddFluentValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(ApplicationBuilderExtensions).Assembly);

        return builder;
    }

    public static WebApplicationBuilder AddDbContext(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        string migrationAssembly = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions))!.GetName().Name!;
        var connectionString = configuration.GetConnectionString("PostgreSQL")!;
        builder.Services.AddDbContext<CheckoutDbContext>(ctx =>
        {
            ctx.UseNpgsql(connectionString, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.EnableRetryOnFailure();
            });
        });
        builder.Services.AddHealthChecks().AddDbContextCheck<CheckoutDbContext>();

        return builder;
    }

    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructureServices();
        return builder;
    }

    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services
            //.AddScoped<IExample1Repository, Example1Repository>()
            .AddDomainEventsAccessor<DomainEventsAccessor<CheckoutDbContext>>()
            //.AddEFCoreRepositories<AnswerRepository>()
            .AddUnitOfWork<UnitOfWork<CheckoutDbContext>>();

        return builder;
    }

    public static WebApplicationBuilder AddBackgroundService(this WebApplicationBuilder builder)
    {
        //builder.Services.AddHostedService<ExampleService>();
        return builder;
    }

    public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        return builder;
    }

    public static WebApplicationBuilder AddRedis(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var connectionString = configuration.GetConnectionString("Redis")!;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("Redis connection string is not configured. Skipping Redis setup.");
            return builder;
        }
        builder.Services.AddSingleton<IConnectionMultiplexer>((_) =>
        {
            return ConnectionMultiplexer.Connect(connectionString);
        });
        builder.Services.AddHealthChecks().AddRedis(connectionString);
        return builder;
    }


    public static WebApplicationBuilder AddDapr(this WebApplicationBuilder builder)
    {
        builder.Services.AddDaprClient();

        return builder;
    }
    public static WebApplicationBuilder AddEventBus(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        builder.Services.AddDaprEventBus((config) =>
        {
            config.AddHealthCheck(failureStatus: HealthStatus.Degraded);
        }
        ).AddIntegrationEventHandlers(Assembly.GetExecutingAssembly());
        return builder;
    }

    public static WebApplicationBuilder ConfigureSwaggerAndMvc(this WebApplicationBuilder builder)
    {

        var title = builder.Configuration.GetValue<string>("Swagger:Title");
        var version = builder.Configuration.GetValue<string>("Swagger:Version");
        var description = builder.Configuration.GetValue<string>("Swagger:Description");

        builder.Services.AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Title = title,
                    Version = version,
                    Description = description,
                    Contact = new OpenApiContact
                    {
                        Name = "DX Team",
                        Email = "graiden.gr@gmail.com",
                        Url = new Uri("http://hexbox.vn"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/license/mit/"),
                    },
                    TermsOfService = new Uri("https://hexbox.vn/terms-of-service"),

                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Sử dụng tiêu đề xác thực JWT với cơ chế Bearer. 
                                    Nhập 'Bearer' [khoảng trắng] và sau đó là token của bạn vào ô bên dưới.
                                    Ví dụ: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                //var applicationXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var infrastructureXml = $"{Assembly.GetAssembly(typeof(CheckoutDbContext)).GetName().Name}.xml";
                //var domainXml = $"{Assembly.GetAssembly(typeof(UnicodeExtensions)).GetName().Name}.xml";
                //var applicationPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                //var infrastructurePath = Path.Combine(AppContext.BaseDirectory, infrastructureXml);
                //var domainPath = Path.Combine(AppContext.BaseDirectory, domainXml);
                //c.IncludeXmlComments(applicationPath);
                //c.IncludeXmlComments(infrastructurePath);
                //c.IncludeXmlComments(domainPath);
            });
        builder.Services.AddMvcCore()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
        builder.AddAuthentication()
                 .AddAuthorization();
        builder.Services.AddCors();
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<FailedResponseFilter>();
            options.Filters.Add<SucceedResponseFilter>();
        }).AddDapr().ConfigureInvalidModelStateResponse();


        return builder;
    }

    private static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration.GetValue<string>("Auths:IdentityProvider");
                options.Audience = "vsp_resource";
                options.RefreshInterval = TimeSpan.FromSeconds(30);
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false
                };
            });
        return builder;
    }

    private static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
    {
        var polices = PoliceConstant.Polices();
        builder.Services.AddAuthorization(options =>
        {
            foreach (var policy in polices)
            {
                if (options.GetPolicy(policy.Name) != null)
                {
                    continue;
                }
                options.AddPolicy(policy.Name, builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole(policy.Roles);
                });
            }
        });

        return builder;
    }
}