using Application.Domain.Portfolios;
using Application.Features.Portfolios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.TagActionsBy(api =>
        new[] { api.RelativePath?.Split('/').ElementAtOrDefault(1) }
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