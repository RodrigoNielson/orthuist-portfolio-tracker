using Application.Domain.Portfolios;
using Application.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.TagActionsBy(api =>
        [api.RelativePath?.Split('/').ElementAtOrDefault(1)]
    );
    c.DocInclusionPredicate((name, api) => true);
});

builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssembly(typeof(Portfolio).Assembly);
});

builder.Services.AddDbContext<PortfolioDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();