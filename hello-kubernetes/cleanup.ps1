echo '清除 dapr 状态存储配置'
echo 'kubectl delete -f deploy/redis.yaml'
kubectl delete -f deploy/redis.yaml
pause

echo '删除 redis 集群'
echo 'helm uninstall redis'
helm uninstall redis
pause

echo '清除 helm repo'
echo 'helm repo remove bitnami'
helm repo remove bitnami
echo 'helm repo update'
helm repo update
pause

echo '在集群中清除 dapr'
echo 'dapr uninstall -k'
dapr uninstall -k
pause