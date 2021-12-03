open System
open System.Net.Http
open System.Net.Http.Json
open System.Threading

let def v ifNull = ifNull |> Option.ofObj |> Option.defaultValue v
let daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT ") |> def "3500"
let daprUri = $"http://localhost:{daprPort}/v1.0/invoke/app/method/neworder" |> Uri

let inline post client message = HttpClientJsonExtensions.PostAsJsonAsync(client, "", message).Wait()

let rec loop n client : unit = 
    let message = {| data = {| orderId = n |} |}

    try
        post client message
    with | e -> printfn "%s" e.Message
    
    Thread.Sleep(TimeSpan.FromSeconds(1.0))
    loop (n + 1) client

using (new HttpClient(BaseAddress = daprUri)) (loop 1)
