FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY /post-loop.fsx .
ENTRYPOINT ["dotnet", "fsi", "post-loop.fsx"]