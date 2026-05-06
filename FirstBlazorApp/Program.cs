using FirstBlazorApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddIdentityConfiguration();

builder.Services.AddAuthorizationConfiguration();

builder.Services.AddSessionServices();

var app = builder.Build();

app.UseConfiguredMiddlewares();

app.UseDevelopmentTools();

app.Run();