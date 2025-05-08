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
# 1) Copia o binário publicado
COPY --from=publish /app/publish .
# 2) Copia o certificado para o container
COPY aspnetapp.pfx /https/aspnetapp.pfx
# 3) Informa ao Kestrel onde achar o certificado e a senha
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=senha123!

EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "MinhaCarteira.API.dll"] 