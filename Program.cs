using EduBridge;
using EduBridge.Persistence.Seed;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDependencies(builder.Configuration);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        await RoleSeeder.SeedRolesAsync(services);
        await UserSeeder.SeedAdminAsync(services);
        await UserRoleSeeder.SeedUserRolesAsync(services);
    }

    if (app.Environment.IsDevelopment())
       { 
        app.MapOpenApi();
       app.MapScalarApiReference();
       }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseStaticFiles();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}