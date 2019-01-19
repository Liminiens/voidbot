FROM microsoft/dotnet:2.2.100-sdk-stretch-arm32v7 AS build-dotnet
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY VoidBot/* ./VoidBot/
RUN dotnet restore
WORKDIR /app/VoidBot
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2.0-runtime-stretch-slim-arm32v7
WORKDIR /app
COPY --from=build-dotnet /app/VoidBot/out .
ENTRYPOINT ["dotnet", "VoidBot.dll"]