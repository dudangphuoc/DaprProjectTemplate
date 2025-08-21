var builder = WebApplication.CreateBuilder(args);
var app = builder.ConfigureServices();
app.ConfigurePipeline();
await app.InitializeAsync();
await app.RunAsync();