echo '在集群中安装 dapr'
echo 'dapr init -k --wait'
dapr init -k --wait
pause

echo '查看 dapr 状态'
echo 'dapr status -k'
dapr status -k
pause

echo '安装 redis 到集群'
echo 'helm repo add bitnami https://charts.bitnami.com/bitnami'
helm repo add bitnami https://charts.bitnami.com/bitnami
echo 'helm repo update'
helm repo update
echo 'helm install redis bitnami/redis'
helm install redis bitnami/redis
pause

echo '查看正在运行 redis 的容器'
echo 'kubectl get pods'
kubectl get pods
pause

echo '配置 dapr 状态存储'
echo 'kubectl apply -f deploy/redis.yaml'
kubectl apply -f deploy/redis.yaml
pause