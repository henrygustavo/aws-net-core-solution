#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["AwsReceiver/AwsReceiver.csproj", "AwsReceiver/"]
RUN dotnet restore "AwsReceiver/AwsReceiver.csproj"
COPY . .
WORKDIR "/src/AwsReceiver"
RUN dotnet build "AwsReceiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AwsReceiver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AwsReceiver.dll"]