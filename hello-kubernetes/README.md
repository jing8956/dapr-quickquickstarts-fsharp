# Hello Kubernetes

![Architecture Diagram](https://raw.githubusercontent.com/dapr/quickstarts/v1.0.0/hello-kubernetes/img/Architecture_Diagram.png)
PS：所有代码都使用 FSharp 替代

## 步骤1：在集群中部署 Dapr

```ps1
dapr init --kubernetes
```

## 步骤2：创建并配置状态存储

1. 按照[官方教程](https://docs.dapr.io/zh-hans/getting-started/configure-state-pubsub/)的步骤用 helm 部署 redis。
> PS：默认有 3 个 replaceSet，非生产使用不需要。
> ```
> helm install redis bitnami/redis --set replica.replicaCount=0
> ```
2. 将 `redis.yaml` 部署到集群
```ps1
kubectl apply -f ./deploy/redis.yaml
```

## 步骤3：部署带有 Dapr 边车的 app
生成镜像
```ps1
docker build -t dapriosamples-fsharp/app:latest app
```

```ps1
kubectl apply -f ./deploy/app.yaml
```

在 spec.template.metadata.annotations 中添加 dapr.io/enabled: "true" 启用 dapr
添加 dapr.io/app-id: "app" 设置 app id
添加 dapr.io/app-port: "3000" 设置 app port

获取暴露的端口
```ps1
kubectl get svc app
```

## 步骤4：通过端口验证服务

```ps1
curl http://localhost/ports
```

获得输出

```json
{"daprGrpcPort":"50001","daprPort":"3500"}
```

## 步骤5：部署带有 Dapr 变成的 post-loop
生成镜像
```ps1
docker build -t dapriosamples-fsharp/post-loop:latest post-loop
```

```ps1
kubectl apply -f ./deploy/post-loop.yaml
kubectl get pods --selector=app=post-loop -w
```

## 步骤6：观察消息
```ps1
kubectl logs --selector=app=app -c app --tail=-1
```

会看到类似这样的消息。
```
info: app.OrderController[0]
      Got a new order! Order ID: 1
info: app.OrderController[0]
      Successfully persisted state.
info: app.OrderController[0]
      Got a new order! Order ID: 2
info: app.OrderController[0]
      Successfully persisted state.
info: app.OrderController[0]
      Got a new order! Order ID: 3
info: app.OrderController[0]
      Successfully persisted state.
```

## 步骤7：确认持久化
```ps1
curl http://localhost/order
{"orderId":237}
```

## 步骤8：清理
```ps1
kubectl delete -f deploy
```
