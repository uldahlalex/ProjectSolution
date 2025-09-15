using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AppOptions>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var appOptions = new AppOptions();
    configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
    return appOptions;
});

var app = builder.Build();
var appOptions = app.Services.GetRequiredService<AppOptions>();
//Here im just checking that I can get the "Db" connection string - it throws exception if not minimum 1 length
Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);
app.MapGet("/", () => "Hello World!");

app.Run();
