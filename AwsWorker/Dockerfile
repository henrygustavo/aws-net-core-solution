#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["AwsWorker/AwsWorker.csproj", "AwsWorker/"]
RUN dotnet restore "AwsWorker/AwsWorker.csproj"
COPY . .
WORKDIR "/src/AwsWorker"
RUN dotnet build "AwsWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AwsWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AwsWorker.dll"]