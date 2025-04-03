using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Migrations;

using (var serviceProvider = CreateServices())
using (var scope = serviceProvider.CreateScope())
{
    UpdateDatabase(scope.ServiceProvider, args);
}

static ServiceProvider CreateServices()
{
    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddPostgres()
            .WithGlobalConnectionString("Host=localhost:5432;Database=portfolio;Username=admin;Password=admin")
            .ScanIn(typeof(Initial).Assembly).For.Migrations()
            .ConfigureGlobalProcessorOptions(opt =>
            {
                opt.ProviderSwitches = "Force Quote=false";
            }))
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .BuildServiceProvider(false);
}

static void UpdateDatabase(IServiceProvider serviceProvider, string[] args)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

    if (args.Length == 0)
        runner.MigrateUp();
    else
        runner.MigrateDown(long.Parse(args[0]));
}