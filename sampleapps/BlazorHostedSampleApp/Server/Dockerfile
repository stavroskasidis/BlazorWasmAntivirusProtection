#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Server/BlazorHostedSampleApp.Server.csproj", "Server/"]
COPY ["Client/BlazorHostedSampleApp.Client.csproj", "Client/"]
COPY ["Shared/BlazorHostedSampleApp.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/BlazorHostedSampleApp.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "BlazorHostedSampleApp.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorHostedSampleApp.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorHostedSampleApp.Server.dll"]