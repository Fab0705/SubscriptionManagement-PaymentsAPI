using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Stripe;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Domain.Constants;
using SubscriptionManagement_PaymentsAPI.Infrastructure.Data;
using SubscriptionManagement_PaymentsAPI.Infrastructure.Data.Interceptors;
using SubscriptionManagement_PaymentsAPI.Infrastructure.Identity;
using SubscriptionManagement_PaymentsAPI.Infrastructure.Payments.Stripe;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SubscriptionManagement_PaymentsAPIDb");
        Guard.Against.Null(connectionString, message: "Connection string 'SubscriptionManagement_PaymentsAPIDb' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();


        builder.Services.AddScoped<IPaymentGatewayService, StripePaymentService>();

        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, SubscriptionManagement_PaymentsAPI.Infrastructure.Identity.IdentityService>();

        builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));
                options.AddPolicy(Policies.CanDelete, policy => policy.RequireRole(Roles.Administrator));
            });
    }
}
