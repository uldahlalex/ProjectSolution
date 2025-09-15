using System.ComponentModel.DataAnnotations;
using api.Etc;
using dataccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AppOptions>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var appOptions = new AppOptions();
    configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
    return appOptions;
});
builder.Services.AddDbContext<MyDbContext>((services, options) =>
{
    options.UseNpgsql(services.GetRequiredService<AppOptions>().Db);
});
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddCors();
builder.Services.AddScoped<ILibraryService, LibraryService>();

var app = builder.Build();

var appOptions = app.Services.GetRequiredService<AppOptions>();
//Here im just checking that I can get the "Db" connection string - it throws exception if not minimum 1 length
Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);

app.UseOpenApi();
app.UseSwaggerUi();
app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(x => true));
app.MapControllers();
app.GenerateApiClientsFromOpenApi("/../../client/src/generated-client.ts").GetAwaiter().GetResult();
app.Run();
