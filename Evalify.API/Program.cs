using Evalify.API;
using Evalify.Application;
using Evalify.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddDebug();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("Teacher"))
        await roleManager.CreateAsync(new IdentityRole("Teacher"));
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Evalify API v1");
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
        options.RoutePrefix = "swagger";
    });

    app.MapScalarApiReference(options =>
    {
        options.Title = "Evalify API";
        options.WithPreferredScheme("Bearer");
        options.WithHttpBearerAuthentication(bearer =>
        {
            bearer.Token = "your-token-here";
        });
    });
}

app.UseExceptionHandler();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
