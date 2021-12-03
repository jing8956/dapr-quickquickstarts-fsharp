#nowarn "20"

open System
open System.Net.Http
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

let def v ifNull = ifNull |> Option.ofObj |> Option.defaultValue v

let daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT ") |> def "3500"
let stateStoreName = "statestore"
let stateUri = $"http://localhost:{daprPort}/v1.0/state/{stateStoreName}/" |> Uri
let port = 3000

let builder = WebApplication.CreateBuilder()
builder.WebHost.ConfigureKestrel(fun _ o -> o.ListenAnyIP(port))
builder.Services.AddHttpClient("dapr").ConfigureHttpClient(fun client -> client.BaseAddress <- stateUri)
builder.Services.AddControllers()

let app = builder.Build()
app.MapControllers()

app.Run()
