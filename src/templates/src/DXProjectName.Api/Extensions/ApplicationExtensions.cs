public static class ApplicationExtensions
{
    public static async Task InitializeAsync(this WebApplication app)
    {
        var config = app.Configuration.GetSection("Initialization");
        if (config.GetValue<bool>("MigrateDatabase"))
        {
            await app.MigrateDatabase<CheckoutDbContext>();
        }
    }
}
