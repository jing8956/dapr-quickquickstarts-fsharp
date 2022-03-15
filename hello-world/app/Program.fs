open Microsoft.Extensions.Logging


#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

let port = 3000

let builder = WebApplication.CreateBuilder()
builder.WebHost.ConfigureKestrel(fun _ o -> o.ListenAnyIP(port))
builder.Logging.SetMinimumLevel(LogLevel.Warning).AddFilter("app", LogLevel.Information)
builder.Services.AddHttpClient()
builder.Services.AddControllers()

let app = builder.Build()
app.MapControllers()

app.Run()
