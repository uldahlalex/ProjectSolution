using System.ComponentModel.DataAnnotations;
using api.Etc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AppOptions>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var appOptions = new AppOptions();
    configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
    return appOptions;
});
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();

var app = builder.Build();
var appOptions = app.Services.GetRequiredService<AppOptions>();
//Here im just checking that I can get the "Db" connection string - it throws exception if not minimum 1 length
Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);
app.UseOpenApi();
app.UseSwaggerUi();
app.MapControllers();
app.GenerateApiClientsFromOpenApi("/../../client/src/generated-client.ts").GetAwaiter().GetResult();
app.Run();
