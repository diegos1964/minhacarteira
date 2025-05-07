FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MinhaCarteira.API/MinhaCarteira.API.csproj", "MinhaCarteira.API/"]
RUN dotnet restore "MinhaCarteira.API/MinhaCarteira.API.csproj"
COPY . .
WORKDIR "/src/MinhaCarteira.API"
RUN dotnet build "MinhaCarteira.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinhaCarteira.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "MinhaCarteira.API.dll"] 