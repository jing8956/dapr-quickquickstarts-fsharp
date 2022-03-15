#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 3000
# 消除警告，默认配置 http://+:80
ENV ASPNETCORE_URLS=
# 消除默认配置 json
ENV Logging__Console__FormatterName=

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["app.fsproj", "."]
RUN dotnet restore "./app.fsproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "app.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "app.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "app.dll"]