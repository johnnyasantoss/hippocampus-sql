FROM microsoft/dotnet:1.1.1-sdk
WORKDIR /app
COPY . /app
CMD ["dotnet", "restore"]
CMD ["dotnet", "test", ".\tests\Hippocampus.SQL.Tests\Hippocampus.SQL.Tests.csproj"]