namespace app

open System
open System.Net.Mime
open System.Net.Http
open System.Net.Http.Json
open System.Runtime.CompilerServices
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

[<ApiController>]
type OrderController(client: HttpClient, logger: ILogger<OrderController>) =
    inherit ControllerBase()

    let daprPort =
        Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")
        |> function | null -> "3500" | v -> v
    let stateStoreName = "statestore"
    let stateUrl = $"http://localhost:{daprPort}/v1.0/state/{stateStoreName}"

    [<HttpGet("order")>]
    member x.Get() = 
        task {
            try
                let! response = client.GetAsync($"{stateUrl}/order")
                if not response.IsSuccessStatusCode then failwith "Could not get state."

                let! str = response.Content.ReadAsStringAsync()
                return x.Content(str, MediaTypeNames.Application.Json) :> IActionResult
            with | e ->
                logger.LogError(e, e.Message)
                return x.StatusCode(500, {| message = e.Message |})
        }

    [<HttpPost("neworder")>]
    member x.Post(body: {| data: {| orderId: int |} |}) =
        task {
            let data = body.data
            let orderId: int = data.orderId
            logger.LogInformation("Got a new order! Order ID: {OrderId}", orderId)

            let state = [{|
                key = "order"
                value = data
            |}]

            try
                let! response = client.PostAsJsonAsync(stateUrl, state)

                if not response.IsSuccessStatusCode then failwith "Failed to persist state."
                logger.LogInformation("Successfully persisted state.")

                return x.Ok() :> IActionResult
            with
            | e ->
                logger.LogError(e, e.Message)
                return x.StatusCode(500, {| message = e.Message |})
        }

    [<HttpDelete("order/{id}")>]
    member x.Delete(id: string) =
        task {
            let key = id
            logger.LogInformation("Invoke Delete for ID {Key}", key)

            let! response = client.DeleteAsync(id)
            if not response.IsSuccessStatusCode then failwith "Failed to delete state."

            logger.LogInformation("Successfully deleted state.")
            return x.Ok()
        }


#if hello_kubernetes
    [<HttpGet("ports")>]
    member _.Ports() =
        let daprPort = int <| System.Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")
        let daprGrpcPort = int <| System.Environment.GetEnvironmentVariable("DAPR_GRPC_PORT")
        {| DaprPort = daprPort; DaprGrpcPort = daprGrpcPort |}
#endif
