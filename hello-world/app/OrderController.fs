namespace app

open System.Net.Mime
open System.Net.Http
open System.Net.Http.Json
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

[<ApiController>]
type OrderController(clientFactory: IHttpClientFactory, logger: ILogger<OrderController>) =
    inherit ControllerBase()

    let client = clientFactory.CreateClient("dapr")

    [<HttpGet("order")>]
    member x.Get() = 
        task { 
            let! str = client.GetStringAsync("order") 
            return x.Content(str, MediaTypeNames.Application.Json)
        }

    [<HttpPost("neworder")>]
    member x.Post(body: Map<string, Map<string, int>>) = 
        task {
            let data = body.["data"]
            let orderId = data.["orderId"]
            logger.LogInformation("Got a new order! Order ID: {OrderId}", orderId)

            let state = [ seq { "key", "order" |> box; "value", data |> box } |> Map ]

            let! response = client.PostAsJsonAsync("", state)
            if not response.IsSuccessStatusCode then failwith "Failed to persist state."

            logger.LogInformation("Successfully persisted state.")

            return x.Ok();
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
