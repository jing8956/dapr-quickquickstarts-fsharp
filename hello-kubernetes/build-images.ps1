cd ..
Get-Content ./hello-kubernetes/dockerfiles/app | docker build -t dapriosamples-fsharp/app:latest -f - .
Get-Content ./hello-kubernetes/dockerfiles/post-loop | docker build -t dapriosamples-fsharp/post-loop:latest -f - .
cd hello-kubernetes