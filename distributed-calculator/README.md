# Distributed calculator
- Add: 加
- Subtract: 减
- Multiply: 乘
- Divide: 除

![Architecture Diagram](https://raw.githubusercontent.com/dapr/quickstarts/v1.0.0/distributed-calculator/img/Architecture_Diagram.png)
PS:后端使用 F# 替代, React 不会只能照搬。

## 先决条件
- [Docker](https://docs.docker.com/)
- [.Net 6 SDK](https://dotnet.microsoft.com/download/)
- [Dapr CLI](https://github.com/dapr/cli)
- [Node.js and npm](https://nodejs.org/en/download/)

## 在本地运行
运行后端
```ps1
dapr run --app-id addapp --app-port 6000 --dapr-http-port 3503 dotnet run -- --project add --urls "http://localhost:6000"
dapr run --app-id subtractapp --app-port 7000 --dapr-http-port 3504 dotnet run -- --project subtract --urls "http://localhost:7000"
dapr run --app-id multiplyapp --app-port 5000 --dapr-http-port 3501 dotnet run -- --project multiply --urls "http://localhost:5000"
dapr run --app-id divideapp --app-port 4000 --dapr-http-port 3502 dotnet run -- --project divide --urls "http://localhost:4000"
```

运行 react-calculator
```ps1
cd react-calculator
npm install
npm run buildclient
dapr run --app-id frontendapp --app-port 8080 --dapr-http-port 3500 node server.js
```

打开 [http://localhost:8080/](http://localhost:8080/) 通过前端计算器计算。

通过前端代理 api 计算。
```ps1
curl -w "\n" -s 'http://localhost:8080/calculate/add' -H 'Content-Type: application/json' --data '{\"operandOne\":\"56\",\"operandTwo\":\"3\"}'
curl -w "\n" -s 'http://localhost:8080/calculate/subtract' -H 'Content-Type: application/json' --data '{\"operandOne\":\"52\",\"operandTwo\":\"34\"}'
curl -w "\n" -s 'http://localhost:8080/calculate/divide' -H 'Content-Type: application/json' --data '{\"operandOne\":\"144\",\"operandTwo\":\"12\"}'
curl -w "\n" -s 'http://localhost:8080/calculate/multiply' -H 'Content-Type: application/json' --data '{\"operandOne\":\"52\",\"operandTwo\":\"34\"}'
curl -w "\n" -s 'http://localhost:8080/persist' -H 'Content-Type: application/json' --data '[{\"key\":\"calculatorState\",\"value\":{\"total\":\"54\",\"next\":null,\"operation\":null}}]'
curl -s 'http://localhost:8080/state'
```

清理
```ps1
dapr stop --app-id addapp
dapr stop --app-id subtractapp
dapr stop --app-id divideapp
dapr stop --app-id multiplyapp
dapr stop --app-id frontendapp
```
## 在 k8s 运行
构建镜像
```ps1
docker build -t dapriosamples-fsharp/distributed-calculator-add:latest add
docker build -t dapriosamples-fsharp/distributed-calculator-subtract:latest subtract
docker build -t dapriosamples-fsharp/distributed-calculator-multiply:latest multiply
docker build -t dapriosamples-fsharp/distributed-calculator-divide:latest divide
```

部署到 k8s
```ps1
kubectl apply -f deploy
```

打开 [http://localhost/](http://localhost/) 通过前端计算器计算。

通过前端代理 api 计算。
```ps1
curl -w "\n" -s 'http://localhost/calculate/add' -H 'Content-Type: application/json' --data '{\"operandOne\":\"56\",\"operandTwo\":\"3\"}'
curl -w "\n" -s 'http://localhost/calculate/subtract' -H 'Content-Type: application/json' --data '{\"operandOne\":\"52\",\"operandTwo\":\"34\"}'
curl -w "\n" -s 'http://localhost/calculate/divide' -H 'Content-Type: application/json' --data '{\"operandOne\":\"144\",\"operandTwo\":\"12\"}'
curl -w "\n" -s 'http://localhost/calculate/multiply' -H 'Content-Type: application/json' --data '{\"operandOne\":\"52\",\"operandTwo\":\"34\"}'
curl -w "\n" -s 'http://localhost/persist' -H 'Content-Type: application/json' --data '[{\"key\":\"calculatorState\",\"value\":{\"total\":\"54\",\"next\":null,\"operation\":null}}]'
curl -s 'http://localhost/state'
```

清理
```ps1
kubectl delete -f deploy
```
