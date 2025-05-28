Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
Copy csproj files
COPY ["LendTech.API/LendTech.API.csproj", "LendTech.API/"]
COPY ["LendTech.Application/LendTech.Application.csproj", "LendTech.Application/"]
COPY ["LendTech.Infrastructure/LendTech.Infrastructure.csproj", "LendTech.Infrastructure/"]
COPY ["LendTech.BackgroundTasks/LendTech.BackgroundTasks.csproj", "LendTech.BackgroundTasks/"]
COPY ["LendTech.Database/LendTech.Database.csproj", "LendTech.Database/"]
COPY ["LendTech.SharedKernel/LendTech.SharedKernel.csproj", "LendTech.SharedKernel/"]
Restore
RUN dotnet restore "LendTech.API/LendTech.API.csproj"
Copy everything else
COPY . .
Build
WORKDIR "/src/LendTech.API"
RUN dotnet build "LendTech.API.csproj" -c Release -o /app/build
Publish
FROM build AS publish
RUN dotnet publish "LendTech.API.csproj" -c Release -o /app/publish /p:UseAppHost=false
Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
Install culture data
RUN apt-get update && apt-get install -y locales
RUN locale-gen fa_IR.UTF-8
Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=fa_IR.UTF-8
ENV LC_ALL=fa_IR.UTF-8
Copy published app
COPY --from=publish /app/publish .
Create logs directory
RUN mkdir -p /app/logs
Expose port
EXPOSE 80
EXPOSE 443
Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 
CMD curl -f http://localhost/health || exit 1
Entry point
ENTRYPOINT ["dotnet", "LendTech.API.dll"]
