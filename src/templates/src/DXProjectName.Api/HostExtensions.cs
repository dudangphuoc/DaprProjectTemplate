public static class HostExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        RegisterExceptionIdentifiers();

        builder.AddConfigurations()
            .AddDbContext()
            //.AddIdempotencyContext()
            //.AddTransactionalEvents()
            .AddRepositories()
            .AddServices()
            .AddAutoMapper()
            .AddFluentValidators()
            .AddBackgroundService()
            .AddMediatR()
            .AddOpenTelemetry()
            .AddSerilog()
            .AddHealthChecks()
            .AddRedis()
            .ConfigureSwaggerAndMvc();

        return builder.Build();
    }

    public static void ConfigurePipeline(this WebApplication app)
    {
        var swaggerEnabled = app.Configuration.GetValue<bool>("Swagger:Enabled");
        if (swaggerEnabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors(x => x
           .SetIsOriginAllowed(origin => true)
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials());

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health", new()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

        });
        app.MapControllers();
    }

    private static void RegisterExceptionIdentifiers()
    {
        ExceptionIdentifiers.Register(builder =>
        {
            builder.AddIdentifier(ExceptionCategories.Transient,
                new TransientHttpExceptionIdentifier());

            builder.AddIdentifier(ExceptionCategories.Transient,
                new TransientEFCoreExceptionIdentifier());

            builder.AddIdentifier(ExceptionCategories.UniqueViolation,
                new UniqueViolationEFCoreExceptionIdentifier());

            builder.AddIdentifier(ExceptionCategories.ConstraintViolation,
                new ConstraintViolationEFCoreExceptionIdentifier());
        });
    }
}
