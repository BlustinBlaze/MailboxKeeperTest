FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY src/API/API.csproj ./src/API/
RUN dotnet restore ./src/API/API.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish ./src/API/API.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .

# Change this if your API DLL has a different name
ENTRYPOINT ["dotnet", "API.dll"]
