open System
open System.Net.Http
open System.Net.Http.Json
open System.Threading

let daprPort =
    Environment.GetEnvironmentVariable("DAPR_HTTP_PORT ")
    |> Option.ofObj |> Option.defaultValue "3500"
let daprUrl = $"http://localhost:{daprPort}/v1.0/invoke/app/method/neworder"

let client = new HttpClient()
try
    let mutable n = 0
    while true do
        n <- n + 1
        let message = {| data = {| orderId = n |} |}

        try
            client.PostAsJsonAsync(daprUrl, message).Wait()
        with | e ->
            printfn "%s" e.Message

        Thread.Sleep(TimeSpan.FromSeconds(1.0))
finally
    client.Dispose()
