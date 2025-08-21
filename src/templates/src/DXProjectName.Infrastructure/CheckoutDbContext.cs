using DX.Infrastructure.EFCore;
using DX.Infrastructure.EFCore.Idempotency.Extensions;
using DX.Infrastructure.EFCore.Interceptors;
using DX.Infrastructure.EFCore.TransactionalEvents.Extensions;
using DX.SharedKernel.Runtime.Session;
using Microsoft.EntityFrameworkCore;

namespace DXProjectName.Infrastructure;

public class CheckoutDbContext : DXDbContext
{
    public CheckoutDbContext(DbContextOptions<CheckoutDbContext> options) : base(options)
    {
    }

    public CheckoutDbContext(DbContextOptions<CheckoutDbContext> options, ISession session) : base(options, session)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddTransactionalEventsModels();
        modelBuilder.AddIdempotentOperationModel();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CheckoutDbContext).Assembly);

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // Cấu hình để sử dụng Interceptor cho EF Core (Npgsql)
        optionsBuilder.AddInterceptors(new NpgsqlPessimisticLockCommandInterceptor());
    }

}
