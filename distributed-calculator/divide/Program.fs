#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

let args = System.Environment.GetCommandLineArgs()
let builder = WebApplication.CreateBuilder(args)
builder.Services.AddControllers()

let app = builder.Build()
app.MapControllers()
app.Run()
