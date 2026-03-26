using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Domain.Common;
using SubscriptionManagement_PaymentsAPI.Domain.Entities;
using SubscriptionManagement_PaymentsAPI.Infrastructure.Identity;

namespace SubscriptionManagement_PaymentsAPI.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    private readonly ICurrentTenantService _currentTenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenantService) : base(options) { _currentTenantService = currentTenantService; }

    public Guid CurrentTenantId => _currentTenantService.TenantId;

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries().Where(e => typeof(IMustHaveTenant).IsAssignableFrom(e.Entity.GetType())))
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("TenantId").CurrentValue = _currentTenantService.TenantId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId);

        builder.Entity<Customer>()
            .HasIndex(c => c.Email);

        builder.Entity<Subscription>()
            .HasIndex(x => x.StripeSubscriptionId);

        builder.Entity<Invoice>()
            .HasIndex(x => x.StripeInvoiceId);

        builder.Entity<Subscription>()
            .Property(s => s.Status)
            .HasConversion<string>();


        foreach (var entityType in builder.Model.GetEntityTypes().Where(e => typeof(IMustHaveTenant).IsAssignableFrom(e.ClrType)))
        {
            builder.Entity(entityType.ClrType).Property<Guid>("TenantId");

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var propertyMethodInfo = typeof(EF).GetMethod("Property")!.MakeGenericMethod(typeof(Guid));
            var tenantIdProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("TenantId"));

            var currentTenantId = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));

            var body = Expression.Equal(tenantIdProperty, currentTenantId);
            var lambda = Expression.Lambda(body, parameter);

            builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
