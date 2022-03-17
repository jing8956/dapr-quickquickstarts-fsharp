# Hello World

构建一个基本的 Web 应用程序，通过调用 dapr 状态存储保存状态。

![Architecture Diagram](https://raw.githubusercontent.com/dapr/quickstarts/v1.0.0/hello-world/img/Architecture_Diagram.png)
PS：所有代码都使用 F# 替代

## 先决条件
- [Docker](https://docs.docker.com/)
- [.Net 6 SDK](https://dotnet.microsoft.com/download/)

## 步骤1：安装 Dapr
[教程链接](https://docs.dapr.io/getting-started/install-dapr/)

## 步骤2：理解代码
通过 Dapr 启动的程序会有一个 `DAPR_HTTP_PORT` 的环境变量，默认值 3500。应用程序通过这个端口与 Dapr 边车通信。
```fsharp
let daprPort =
    Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")
    |> function | null -> "3500" | v -> v
let stateStoreName = "statestore"
let stateUrl = $"http://localhost:{daprPort}/v1.0/state/{stateStoreName}"
```

`POST /neworder` 将收到的订单对象输出到日志，然后存入 Dapr 状态存储。
```fsharp
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
```

`GET /order` 将 Dapr 状态存储中存储的 order 读取出来
```fsharp
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
```

## 步骤3：用 Dapr 运行 FSharp App
使用 Dapr CLI 启动程序
```ps1
dapr run --app-id app --app-port 3000 --dapr-http-port 3500 dotnet run -- --project app/app.fsproj
```

看到成功消息：
```
Updating metadata for app command: dotnet run --project app/app.fsproj
You're up and running! Both Dapr and your app logs will appear here.
...
```

## 步骤4：向服务发送消息

使用 Dapr CLI 发送消息
```ps1
dapr invoke --app-id app --method neworder --data '{\"data\": { \"orderId\": \"42\" } }'
```

控制台收到日志
```
== APP == info: app.OrderController[0]
== APP ==       Got a new order! Order ID: 42
```

## 步骤5：确认成功持久化

使用 Dapr CLI 获取
```ps1
dapr invoke --app-id app --method order --verb GET
```

返回结果
```
{"orderId":42}
App invoked successfully
```

## 步骤6：通过跑另一个程序发送消息
发送消息的 Url
```fsharp
let daprPort =
    Environment.GetEnvironmentVariable("DAPR_HTTP_PORT ")
    |> Option.ofObj |> Option.defaultValue "3500"
let daprUrl = $"http://localhost:{daprPort}/v1.0/invoke/app/method/neworder"
```

运行
```ps1
dapr run --app-id post-loop dotnet fsi post-loop.fsx
```

控制台看到日志
```
== APP == info: app.OrderController[0]
== APP ==       Got a new order! Order ID: 1
== APP == info: app.OrderController[0]
== APP ==       Successfully persisted state.
== APP == info: app.OrderController[0]
== APP ==       Got a new order! Order ID: 2
== APP == info: app.OrderController[0]
== APP ==       Successfully persisted state.
== APP == info: app.OrderController[0]
== APP ==       Got a new order! Order ID: 3
== APP == info: app.OrderController[0]
== APP ==       Successfully persisted state.
```

## 步骤7：清理
在控制台按 `Ctrl+C` 或通过 Dapr CLI 停止程序
```
dapr stop --app-id app
dapr stop --app-id post-loop
```
